# SmartAC Proof of Concept - Specification

## Notes to Candidate:

This is a fictional project for a fictional client, but please treat the project in a realistic fashion including any communication with the team.  

**LICENSE: _All resources included in this project, and communication via Slack are to be treated as confidential and cannot be shared publicly outside of this exercise; either in their original form or as derivations.  You may retain a private copy for your own use only._**

## Project Summary

Our client is a manufacturer of smart air conditioning devices (SmartAC) that include either cellular modem or WIFI connectivity in order to report back their status at regular intervals to central servers for monitoring.  The broader project will include all the "bells and whistles" of  user and admin portals, facility management, and end-user registration; but we **are not addressing all of this for the PoC**.  The main goals of the PoC are centered around the concepts that:

* a SmartAC device can register itself with the server, unattended by any user, and in a secure fashion
* a SmartAC device can send regular sensor readings to the server in a secure fashion
* these registered SmartAC devices can be seen within a secured administrative web portal 
* these SmartAC device sensor readings can be viewed in an administrative web portal
	*	as tabular data
	*	as graphs
*	the administrative portal can monitor the devices for anomalous values and 
	*	create alerts that are seen by users of the administrative portal
	*	resolve alerts by users of the administrative portal

We have some flexibility in how to implement these features since the devices are not yet created, but we should stay within some constraints that are expected to exist in the device designs:

* Devices can talk **HTTPS** (and HTTP for local testing)
* Devices do not follow redirects (HTTP `301` or `302`)
* Device programming library has a built-in **JSON** serializer/deserializer.
* Devices can verify **JWT tokens**, and any other auth token will be treated as opaque.
* Devices treat numeric values **all** as decimals to two places (i.e. `1.00`, `2.10`, `12.01`).
* Devices know their own serial number burned into ROM and will never forget it.
* Devices know a shared registration secret burned into ROM and will never forget it.
* Devices can record and buffer data for up to about 4MB of data, and will typically send batches of data instead of individual elements of data.
* Devices cannot do much about errors returned from the server, they are Smart devices but dumb in that they have no ability to handle interaction to work around problems.
* Devices can hold other custom data of up to 2MB.  This data may be lost during long power outages (that exceed battery limits) or during firmware updates.  This is where they store things like auth tokens or state about what they have previously sent.

Therefore it is clear we should write a REST API that the device can talk to over HTTPS (or HTTP in development) using data in JSON.  The device memory sizes we can probably safely ignore on the server-side other than ensuring any auth token sent to the device is not excessively large.  Other than that, you can design the format of the JSON objects as needed.

## Features

Features fall into 3 categories, and the features are labelled with the following prefixes:

* Backend Device API (BE-DEV)
* Backend Admin API (BE-ADM)
* Frontend Admin UI (FE-ADM)

### FE-ADM

Refer to the [Wireframes](https://www.figma.com/file/uYhDDrxN5wq7r837wO8A7Q/Wireframes). Also the full details of the frontend tasks are not specified, just general descriptions.

### Backend Device API (BE-DEV)

The backend device API must allow communication with the devices (register, report status) as well as processing of the device data after it is received (validation, alerts).

#### BE-DEV-1

A device can self-register with the server (open endpoint, no auth)

> Using it's own serial-number and a shared secret (known only to the device and to the server) the device can send this information to the server and receive an auth token in response which is to be used in subsequent calls. The device will also report its firmware version during registration.

notes:  

* The server should have a fixed set of devices known to it for the purposes of testing and this PoC, some 5 to 10 devices with unique secrets each should be added to the database to be used.
* A device might register more than once if it forgets its token, if it receivers a 401 error, or after a firmware update.
* We should store for each registered device:
	* The serial number (alpha numeric 24-32 characters)
	* The date of its first registration (UTC)
	* The date of the most recent registration (UTC)
	* The most recent firmware version ([semantic versioning](https://semver.org/))

#### BE-DEV-2

A device will continually report its sensor readings to the server (secure endpoint, requires auth)

> A registered device will soon after and on a continuing basis report its sensor readings to the server.  This includes the **temperature**, **humidity**, **carbon monoxide**, and its overall **health status value**

notes:

* requires device auth 
* A device records a snapshot of all of its sensor readings  at a point in time (typically every minute).
* A device sends these readings in batches with each snapshot having its own original timestamp (UTC).
* All of the numeric readings follow the rule above of having two decimal places.  The units of measurement are not part of the data and are defined implicitly by the type of reading.  
	* **temperature**  in Celsius (-30.00 to 100.00),
	* **humidity** as percentage (0.00 to 100.00)
	* **carbon monoxide** as parts-per-million _PPM_ (0.00 to 100.00).
* If the numeric values are parseable as numbers, accept values even if outside of the expected ranges.  See BE-DEV-3 for handling of out-of-range values.
* The **health status value** is only one of the following known strings:  `OK`, `needs_filter`,   and `needs_service`
* Any HTTP code that is `2xx` will cause the device to interpret the call as successful and it will erase its buffer.  A return of `401` or `403` will cause it to re-register and resend the data after.  All other return codes will cause a resend of the same data later along with newly acquired data until buffer overflows and data is lost.
* We should store for each snapshot of readings:
	* The serial number of the device making the readings
	* The timestamp (UTC) the device recorded the reading
	* The timestamp (UTC) the server received the reading 
	* The health status value
	* The individual sensor readings (temp, humidity, carbon monoxide)

#### BE-DEV-3 

Received device data that is out of expected safe ranges should produce alerts.

> When data is received from a device, the data should be analyzed and determined if it is "out of range" and if so cause an alert to be created.  

notes:

* devices should have control returned to them as quickly as possible when sending sensor readings, so all alert processing should be done async.
* data that can generated alerts are:
	* OUT OF ACCEPTED RANGE (by sensor): Any values for `temperature`, `carbon monoxide`, or `humidty` that are out of the ranges specified in the requirements above (data is readable but is out of expected ranges).  
        * Use alert text similar to: _"Sensor xyz reported out of range value"_ (where `xyz` is the name of the sensor)
        * Each sensor is its own alert type and report individual alerts.
	    * A value being out of range below minimum value is same as the value being out of range above maximum value (i.e. temp of -1000 or 2000 both continue same alert).
    * DANGEROUS CO LEVELS: **Carbon Monoxide** at dangerous levels
        * Set the threshold value to a configurable 9.00 PPM and alert on any value higher. 
        * Use alert text similar to: _"CO value has exceeded danger limit"_.  
        * This alert is independent of alerts for out of range values and should report its own alert (i.e. a value of 200.0 is both a dangerous CO level alert and also an out of range value)
	* POOR HEALTH: **Health status value** that is any other string value other than `OK`.  
        * Use alert text: _"Device is reporting health problem: xyz"_ where `xyz` is the text of the health status enumeration.
* We should store in new database table(s), for each alert: 
	* Serial number of the device causing the alert
    * The type of alert (out of range temp, out of range CO, out of range humidity, dangerous CO, poor health)
	* The timestamp (UTC) of when the server created the alert
	* The timestamp (UTC) of the recorded datetime for the sensor data that caused (started) the alert
	* The timestamp (UTC) of the recorded datetime for the sensor data of the most recent reported value within this alert.
	* The textual alert (_examples above_)
	* The view state of an alert (`unviewed` vs. `viewed`)
	* The resolve state of the alert (`new`,  `resolved`, `ignored` )
* An alert provides a connection to the sensor readings that are related to the alert by the serial number and the recorded datetime range of the alert.  See BE-DEV-4 for more on alert merging.

#### BE-DEV-4

Device alerts should merge and not duplicate.

> A device that enters an alert state for a given topic, should not duplicate that same alert while the alert is still unresolved (or ignored).

notes:

* A device might have ongoing data values that would cause alerts.  For the same alert type, this expands or falls within the timeframe of a previously created alert and does create a new alert.
    * It is important to process batch of alerts individually in recorded datetime order to ensure they are merging with the correct alerts.  The same batch might both create and resolve an alert.
    * The view `viewed` / `unviewed` status does not affect merging (they merge)
    * The resolution `new` and `ignored` status does not affect merging (they merge)
    * The resolutions status of `resolved` followed by a new alert within (a configurable) 15 minutes merges into previous alert and sets the status back to `new` (but does not update viewed status), while outside that time range creates a new alert instead. This prevents many alerts for boundary values.
* The value causing the alert might differ from previous, but if the specific alert type is the same these are considered the same alert.  Examples:
	* A device reports dangerous CO PPM at 25, then 28, then 30, then 24 - are all considered the same alert of _"CO value has exceeded danger limit"_ and expand the time range of the same alert.
	* A device reports CO PPM at 25, which causes an alert.  Then there is a temperature of -500.00 which is out of range.  These are considered two different alerts and de not merge.
    * A device reports temperature 200, then 199, then 180, which causes a temperature out of range alert with merging causing the time range of the alert to span all of these sensor reports.
    * A device reports both a temperature of 200 and a humidity of 500 in the same report, this creates two distinct alerts.
* Information should be updated to the existing alert to encompass the new sensor values:
	* the timestamp (UTC) of the recorded datetime for the data of the latest reported value within this alert if it is higher than previous value.
    * any other value that makes sense for your implementation of alert tracking.

#### BE-DEV-5

Device alerts may self resolve.

> A device that was previously in an alert status but is no longer in an alert status should resolve its own alert.

notes:

* A device which has a current alert that was not resolved will mark its own value resolved when it no longer meets the criteria for the alert type that was created. For example:
    * A device reports CO PPM at 25 which creates an alert, then reports a value of 28 which extends the time range of the same current alert, and then later reports CO PPM of 5 which then marks the alert as `resolved`.
    * A device reports a health status if `needs_filter` creating an alert and then 1 hour later reports health status of `OK`, will resolve the alert.
    * A device in one batch of data reports a humidity reading of 200 and then a reading of 100, this will both create and resolve an alert.
* An alert marked `ignored` will be overridden by `resolved` once resolved.

#### BE-DEV-6

Device sensor data that does not validate must be preserved.

> A registered device might send data that is not parseable or otherwise readable, and this important information should be preserved in a log for later inspection and analysis.

notes:

* A device cannot respond to errors such as "400 - bad request" for invalid data.  Therefore the server should keep this data in a log (preferably the database) so that it can be later analyzed and maybe corrected if the problem is systematic and predictable (i.e. a firmware error causes temperatures to be reported with the "c" at the end of the numbers `10.02c` which prohibits valid parsing)  
* You can store this in any format, since it could be unclear what data was received in these cases.
* If the data is "too large" then truncate the data to the first 500 characters.
* We should store for each un-readable report:
	* serial number of the device sending the invalid data
	* the timestamp (UTC) when server received the data
	* the raw data that was received (up to 500 characters)

#### BE-DEV-7

Devices sending a lot of invalid data should cause a new alert.

> A device that has sent significant amounts of unreadable data should cause an alert.

notes:

* This is related to the previous requirement, and if a device has sent more than 500 items of invalid data since its last registration, it should cause an alert "Device sending unintelligible data".
* This alert self-resolves on a new device registration.
* This alert can be marked ignored by a user as can any alert.

#### BE-DEV-8

A device may read its own alerts.

> A device may read server-side created alerts, so that for the devices that have user interfaces, they can display the information to their users or maintenance crews.

notes:

* requires device auth
* alerts can be filtered by view state: `all`, `unviewed` (default), `viewed`
* alerts can be filtered by resolved state: `all`, `new` (default), `resolved`, `ignored`
* return alerts in order of newest to oldest
* return the data of the alert
  * and the high/low values of all numeric sensor readings within the datetime range of the alert regardless of alert type
  * and indicate which sensor (or health status) is at fault so that the alert text is not parsed to determine this
* results should be paged (default 50 per page)

### Backend Admin API (BE-ADM)

The backend administrative API is to allow a frontend to be created to visualize the activity of devices, and allow the viewing of alerts in order for them to have a human response.

#### BE-ADM-1

User Login (open endpoint).

> A user can log into the system and have an auth token

notes:

* For the PoC we can have one hard coded set of credentials and this API is more for providing the auth token for subsequent calls.

#### BE-ADM-2

User logout (secure endpoint, requires auth)

> A user can logout of the system

notes:

* requires auth

#### BE-ADM-3

List recently registered devices  (secure endpoint, requires auth)

> Devices may be listed by those most recently registered in the system.

notes:

* requires auth
* data should be paged.

#### BE-ADM-4

List sensor readings for a device by date range  (secure endpoint, requires auth)

> A unique device serial number has sensor readings organized in a time series, return those within the expected date range ordered by most recent to least recent.

notes:

* requires auth
* This includes the numeric sensor values (temperature, humidity, carbon monoxide) and also the health status textual value.  Only valid data is returned (none from BE-DEV-6 that is logging of invalid data)
* Absence of date range returns all data
* data should be paged.

#### BE-ADM-5

Aggregate sensor readings for a single device by date range  (secure endpoint, requires auth)

> A device has sensor readings organized in a time series, and for display in graphs they should be aggregated within a date range to allow for easy use in graphs or other display models.

notes:

* requires auth
* Each sensor (temperature, humidity, carbon monoxide) are each their own series of data.
* Since there are too many points to plot for large time periods (could have 1440 or more readings per day), the data must be broken into buckets that each aggregate the values that fall within the bucket sub range of time (i.e. 60 per hour need to become just one first/last/min/max/avg)
* each aggregation period should have the first value, last value, minimum, maximum, and average values for that aggregation period.
* Our DBA has requested that since there could be hundreds of thousands of rows for a device that we do as much as possible in SQL (or whatever the database language is), and do not pull all rows in the application server.  The bucketing rules below can be changed to something that is similar to make it easier in the database, use them only as a rough guideline.
* The input is a start/end date to get a date range.  The aggregation granularity should be something like the following (does not need to be exact) for the common values the UI selects:
	*   last 1 day - 24 buckets of 1 hours each
	*   last 7 days - 28 buckets of 6 hours each
	*   last 14 days - 28 buckets of 12 hours each
	*   last 21 days - 28 buckets of 16 hours each
	*   last 30 days - 30 buckets of 1 day each
	*   last 90 days - 30 buckets of 3 days each
	*   last 180 days - 30 buckets of 6 days each
* Smooth the aggregation granularity between those values (if not one of the fixed values above:  divide date range hours by 28, and if <24 then use that hours as size of bucket, otherwise divide date range days by 30, and use those days as size of bucket)
* The health status value is not a sensor and is not included in this data.
* data should not be paged, but should have a limit of 100 elements

#### BE-ADM-6

List alerts active in the system  (secure endpoint, requires auth)

> Active alerts in the system can be listed given their view status and resolved status.

notes:

* requires auth
* alerts can be filtered by view state: `all`, `unviewed` (default), `viewed`
* alerts can be filtered by resolved state: `all`, `new` (default), `resolved`, `ignored`
* return alerts in order of newest to oldest
* return the data of the alert
	* and the high/low values of all numeric sensor readings within the datetime range of the alert regardless of alert type
	* and indicate which sensor (or health status) is at fault so that the alert text is not parsed to determine this
* results should be paged (default 50 per page)

#### BE-ADM-7

Alerts can be marked viewed  (secure endpoint, requires auth)

> An alert can be marked viewed by setting view state in the alert to "viewed".

notes:

* requires auth

#### BE-ADM-8

Alerts can be marked ignored  (secure endpoint, requires auth)

> An alert can be marked ignored by setting resolve state to "ignored" unless the state is already "resolved" in which case it cannot be changed.

notes:

* requires auth

#### BE-ADM-9

Alert data can be listed along with sensor readings.

> When listing sensor readings [BE-ADM-4](#be-adm-4) it is convenient to know if the sensor reading is related to an alert, and therefore to have that alert data.

notes:

* requires auth
* Because an alert has a start/end timestamp related to the data that caused the alert, we can enhance the endpoint that returns sensor data for a device (see requirement 2 above) to include a reference to an alert that was caused or supported by each specific sensor reading.  
* The data can be a reference to the alert key (therefore another endpoint must be added to retrieve those alerts individually), or the alert data can be nested into the sensor reading data for easy viewing.
 
#### BE-ADM-10

Search for a device by serial number   (secure endpoint, requires auth)

> A device may be found by its unique serial number.  This includes primarily devices that are registered, and as a second part of the response those that have not been registered yet.

notes:

* requires auth

#### BE-ADM-11

Filter devices by registration date   (secure endpoint, requires auth)

> Devices may be filtered by a range of dates in which they were registered, ordered by most recently registered to least recent for those found.

notes:

* requires auth
