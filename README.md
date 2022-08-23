# Smart Ac - Backend Practical Exercise

This is a fictional project for a fictional client, but please treat the project in a realistic fashion including any
communication with the team.

For a quick context, we have a simple backend service that works with SmartAC air conditioning units installed in
hundreds of thousands of homes around the country. The devices can register themselves and then immediately start
sending data about their environments.

Devices are manufactured with a unique secret that allows them to register later, and some devices are already added to
the database to make testing easier.

For in-deep context read the [project feature specs](docs/smartac-spec.md).

# Tasks for this Exercise

Before beginning, please familiarize yourself with the project and existing code, make sure that you can open the
project and run unit tests.

Read the [contributor guide](docs/contributor-guide.md) for commits, branch naming, and pull requests.

Also, read the requirements for all current tasks and the backlog so that you have context for the project and the work
you are doing. You will not be implementing all of the tasks listed, but it is a good idea to know what the future holds
for the project.

After reading the requirements and getting basically familiar with the project, a plan for the day might be something like:

* 1 hour - Your first contribution and making a plan
* 2-3 hours - Refactoring to clean but minimalist architecture
* 4-5 hours - Feature development and related tests

Of course, your times may vary and feel free to ask to extend time or talk about prioritization if the scope exceeds time allowed.

## 1. Your First Contribution

The team quickly setup a new project and took care of some of the tasks to get the project moving. They didn't quite get
it all done, so to start please:

- Fix and make sure all test are running and passing
- Fix and make sure all "TODO" comments get resolved

## 2. Make a Plan

Now that you are familiar with the code,and the tasks and features listed below, please provide a rough plan to the team
in Slack about how you are going to proceed for the remaining tasks. Let them know when you plan to work on them and
about when they should be ready to help with pull request reviews or to answer any of your questions.

## 3. Refactoring: Layering the Code

We now have a very good understanding of the feature list and that give us and idea on where this is going. The existing code was not layered very well (having all of the code in the controllers) and we would like you to take the lead on cleaning this up before development continues.

Find a minimalist set of changes to structure the code in a way that is scalable and maintainable for the known features
to be developed. The new architecture must be scalable, maintainable and should take advantage of all the performance
capabilities of ASP.NET Core.  A lot of architectures would work, but lets go for something quick and simple.

- Discuss in Slack any questions that you might have
- Implement the layered architecture and migrate current features to it
- ⚠️ Time is precious, don't forget there are other tasks and features to be built!

## 4. New Features

With the structure in place, we now want to implement new functionality into our project, the following is a
priority list of features we want your help on (those marked are already completed):

### Required Features

- [x]  [BE-DEV-1](docs/smartac-spec.md#be-dev-1) - A device can self-register with the server (_open endpoint, no auth_)
- [x]  [BE-DEV-2](docs/smartac-spec.md#be-dev-2) - A device will continually report its sensor readings to the server (_
  secure endpoint, requires auth_)
- [ ]  [BE-DEV-3](docs/smartac-spec.md#be-dev-3) - Received device data that is out of expected safe ranges should
  produce alerts (_internal logic_)
- [ ]  [BE-DEV-4](docs/smartac-spec.md#be-dev-4) - Device alerts should merge and not duplicate (_internal logic_)
- [ ]  [BE-DEV-5](docs/smartac-spec.md#be-dev-5) - Device alerts may self resolve (_internal logic_)
- [ ]  [BE-DEV-8](docs/smartac-spec.md#be-dev-8) - A device can read its alerts back from the system to display on its own user interface (_
  secure endpoint, requires auth_)

_**note:** a few of the above are already marked as completed and implementations exist in the current code._

### Project Backlog (out of scope for this iteration)

- [ ]  [BE-ADM-1](docs/smartac-spec.md#be-adm-1) - User Login (_open endpoint, no auth_)
- [ ]  [BE-ADM-3](docs/smartac-spec.md#be-adm-3) - List recently registered devices (_secure endpoint, requires auth_)
- [ ]  [BE-ADM-5](docs/smartac-spec.md#be-adm-5) - Aggregate sensor readings for a device by date range (_secure
  endpoint, requires auth_)
- [ ]  [BE-ADM-6](docs/smartac-spec.md#be-adm-6) - List alerts active in the system  (_secure endpoint, requires auth_)
- [ ]  [BE-DEV-6](docs/smartac-spec.md#be-dev-6) - Device sensor data that does not validate must be preserved (_
  internal logic_)
- [ ]  [BE-ADM-4](docs/smartac-spec.md#be-adm-4) - List sensor readings for a device by date range (_secure endpoint,
  requires auth_)
- [ ]  [BE-DEV-7](docs/smartac-spec.md#be-dev-7) - Devices sending a lot of invalid data should cause a new alert (_
  internal logic_)
- [ ]  [BE-ADM-11](docs/smartac-spec.md#be-adm-11) - Filter devices by registration date   (_secure endpoint, requires
  auth_)
- [ ]  [BE-ADM-7](docs/smartac-spec.md#be-adm-7) - Alerts can be marked viewed  (_secure endpoint, requires auth_)
- [ ]  [BE-ADM-8](docs/smartac-spec.md#be-adm-8) - Alerts can be marked ignored  (_secure endpoint, requires auth_)
- [ ]  [BE-ADM-9](docs/smartac-spec.md#be-adm-9) - Alert data can be listed along with sensor readings (_internal
  logic_)
- [ ]  [BE-ADM-2](docs/smartac-spec.md#be-adm-2) - User logout (_secure endpoint, requires auth_)
- [ ]  [BE-ADM-10](docs/smartac-spec.md#be-adm-10) - Search for a device by serial number (_secure endpoint, requires
  auth_)

_**note:** these are all backend API features and UI is not part of this backend project._

# Documentation

[Write your own documentation about decisions, assumptions or any other thing you feel is important here]
