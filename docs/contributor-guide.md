# SmartAC Contributor Guide

Follow these standards when contributing to SmartAC codebase:

* Use meaningful branch names that follow patterns detailed below.
* Use pull requests (PR) to signal code is ready for review and to be merged to main.
* Use meaningful pull request names and descriptions that describe the work to both other developers and product management.
* Use stacked pull requests when you have PR's dependent on other PR's so that you can continue working ahead of PR reviews yet still keep each PR isolated. ([read more about stacked pull requests](https://blog.logrocket.com/using-stacked-pull-requests-in-github/))

## Branch Naming

```
feature/SOMETHING-123 
^-----^ ^-----------^
|       |
|       +-> Feature ID or short descriptive name with `-` word separation (use present tense)
|
+-------> Type: chore, docs, feature, fix, refactor, style, or test.
```

Examples of valid branch names:

* feature/BE-ADMIN-3
* chore/bump-mylib-version-3.4.1
* refactor/add-widget-layer

## Commits and Commit Messages

We commit meaningful units of work that keep cohesive changes together.  Commits do not include unrelated items to the commit message.
Commit messages are in present tense.  Commit messages might look like:

* "BE-ADMIN-3 add base model class for generic device error messages"
* "Add missing `FromBody` attribute to all controller methods"
* "Add integration tests for device error logging, including BE-ADMIN-3 and BE-ADMIN-7"

Commit messages can have multiple lines, but the first line or sentence should be the overall description.

```
Bump versions on all dependencies to current versions.
* FredLib 1.2.3
* MyLib 3.4.2
* OtherSoftLib 4.0.2 (includes security fix)
```

This could also defer the detail to the PR description instead, but is fine in both places.

## Pull Requests

When creating a pull request for a branch, the title of the pull request should be meaningful in a way that it quickly answers
the question of "what is this for?".  For example:

* FEATURE BE-DEV-12 Allow devices to log battery errors to server
* CHORE Update MyLib to version 3.4.1
* REFACTOR Add Widget Layer to the UI, removing WebComponents

The description would contain more details in proper Markdown, such as:

```
* Adds endpoint `POST api/v1/device/{deviceId}/error`
* Adds new custom type `DeviceErrorLogEntry` and examples for Swagger
* Adds basic tests (coverage might be weak here)
```

Pull requests are typically created early showing work in progress on a branch, and include the `[WIP]` prefix until complete
and ready for review.  For example `[WIP] CHORE Remove Newtonsoft` while in progress and then `CHORE Remove Newtonsoft` once
the work is finished and ready for review.

After a review, reply to any comments and resolve comments as they are considered or code is changed to reflect them.  Not
all comments need to be agreed with, and conversations and thoughtful responses are appropriate.  Not all review comments 
are actionable.

Pull requests are merged by the owner when reviews are complete. A reviewer might request minor changes or optional changes and
at the same time indicate a merge can happen, do not send these minor changes back for another review unless you have further
questions. A reviewer _might_ merge your pull requests in cases where they or others are dependent on the code and have a need
for the merge to happen earlier (i.e. pending release date).
