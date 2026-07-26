# Aether View — Implementation Plan

## 1. Objective

Build a stable offline-first MVP of Aether View using .NET MAUI and .NET 10.

The MVP must support the complete basic ARV workflow:

1. Create a binary project.
2. Select one warm and one cold image.
3. Randomly assign images to outcomes.
4. Lock the protocol.
5. Record multiple trials.
6. Perform self-judging.
7. Calculate a prediction.
8. Schedule a feedback notification.
9. Enter the actual winner.
10. Reveal the winner's assigned image.
11. Store the result and update statistics.

The solution must remain intentionally small:

- one production MAUI project;
- one unit test project.

## 2. MVP boundaries

### Included

- local projects;
- binary outcomes;
- warm and cold image library;
- cryptographically secure outcome assignment;
- text notes;
- drawing canvas;
- configurable trial count;
- self-judging;
- configurable confidence values;
- prediction calculation;
- local feedback notifications;
- manual winner entry;
- full-screen feedback image;
- local SQLite persistence;
- basic statistics;
- dark and light theme;
- Android and Windows as initial development targets.

### Excluded from MVP

- automatic sports result retrieval;
- cloud synchronization;
- user accounts;
- social functions;
- bookmaker integration;
- automated betting;
- AI image analysis;
- AI drawing assessment;
- lunar and solar correlations;
- public rankings;
- real-time device synchronization;
- embedded commercial Hemi-Sync recordings.

## 3. Architecture

### Projects

```text
AetherView.App
AetherView.Tests
```

### Architecture style

Use a pragmatic MVVM architecture inside the MAUI project.

```text
UI -> ViewModel -> Service/Repository -> SQLite
                   |
                   -> Domain rules
```

The application does not need separate Domain, Application, and Infrastructure projects for the MVP. Separation is achieved through folders, namespaces, interfaces, and tests.

### Main folders

```text
src/AetherView.App/
├── Domain/
├── Data/
├── Services/
├── Features/
├── Controls/
├── Converters/
└── Resources/
```

### Dependency rules

- Pages may reference their ViewModels.
- ViewModels may reference domain types and service interfaces.
- Domain rules must not reference MAUI controls.
- Persistence code must not contain UI logic.
- Notification services must be accessed through an interface.
- Hidden assignments must only be accessed through a dedicated service.
- Tests should reference production code without starting a MAUI application.

## 4. Proposed domain model

### Entities

```text
ArvProject
ProtocolConfiguration
ImageAsset
BlindAssignment
ArvTrial
TrialDrawing
TrialJudgement
PredictionResult
FeedbackResult
AudioTrack
UserSettings
AuditEvent
```

### Important enums

```text
ArvProjectStatus
ImageTemperature
PredictionDecision
ActualOutcome
TrialStatus
AuditEventType
```

### Recommended project fields

```text
Id
Name
Question
OutcomeAName
OutcomeBName
EventStartsAt
FeedbackAvailableAt
PlannedTrialCount
Status
ProtocolVersion
ScoringAlgorithmVersion
CreatedAt
UpdatedAt
```

### Recommended image fields

```text
Id
StorageKey
OriginalDisplayName
Temperature
IsBuiltIn
IsEnabled
UsageCount
LastUsedAt
CreatedAt
```

`StorageKey` must not reveal whether the image is warm, cold, or assigned to a particular outcome.

### Recommended blind-assignment fields

```text
Id
ProjectId
WarmImageId
ColdImageId
OutcomeAImageId
OutcomeBImageId
CreatedAt
IsLocked
RevealedAt
```

The application must validate that:

- one image is warm;
- one image is cold;
- the two image identifiers differ;
- each outcome has exactly one image;
- the assignment cannot change after protocol lock.

## 5. Project state machine

Allowed transitions:

```text
Draft -> ProtocolLocked
ProtocolLocked -> TrialsInProgress
TrialsInProgress -> TrialsCompleted
TrialsCompleted -> JudgingInProgress
JudgingInProgress -> PredictionCalculated
PredictionCalculated -> AwaitingEvent
AwaitingEvent -> AwaitingFeedback
AwaitingFeedback -> FeedbackRevealed
FeedbackRevealed -> Completed
```

Exceptional transitions:

```text
Draft -> Cancelled
ProtocolLocked -> Invalidated
TrialsInProgress -> Invalidated
TrialsCompleted -> Invalidated
JudgingInProgress -> Invalidated
AwaitingEvent -> Cancelled
```

Every transition should be implemented through explicit domain methods or a state transition service.

Avoid directly assigning arbitrary status values from ViewModels.

## 6. Delivery stages

### Current implementation status

Last updated: 2026-07-26.

| Stage | Status | Summary |
| --- | --- | --- |
| Stage 0 — Repository bootstrap | In progress | The solution, MAUI project, xUnit project, SDK pinning, documentation, restore, build, and test flow exist. Central package management is still missing, application startup has not been manually verified, and Android clean builds report two SQLite native-library warnings. |
| Stage 1 — Application shell and navigation | Not started | Only the default MAUI template shell exists. Feature navigation, ViewModels, and application themes have not been implemented. |
| Stage 2 — Domain model and protocol rules | In progress | The initial domain foundation, secure blind assignment, the main happy-path transitions, clock abstraction, feedback reveal rule, and 19 domain tests are implemented. The remaining Stage 2 work is listed below. |
| Stages 3–10 | Not started | Persistence, project UI, image library, trials, judging, notifications, statistics, audio, and polish remain outside the current implementation. |

## Stage 0 — Repository bootstrap

Deliverables:

- solution and both projects;
- `global.json`;
- `Directory.Build.props`;
- `Directory.Packages.props`;
- `.editorconfig`;
- `.gitignore`;
- `README.md`;
- `PLAN.md`;
- `AGENTS.md`;
- baseline build and test commands.

Exit criteria:

- `dotnet build` succeeds;
- `dotnet test` succeeds;
- MAUI app starts on Windows or Android;
- repository has no generated build artifacts committed.

## Stage 1 — Application shell and navigation

Deliverables:

- dependency injection setup;
- Shell navigation;
- application theme;
- placeholder pages:
  - Dashboard;
  - Projects;
  - Project editor;
  - Session;
  - Judging;
  - Feedback;
  - Statistics;
  - Settings.

Exit criteria:

- navigation works without service locator calls;
- ViewModels are resolved through dependency injection;
- dark and light themes render correctly.

## Stage 2 — Domain model and protocol rules

Deliverables:

- entities and enums;
- project state transitions;
- protocol validation;
- secure image-to-outcome assignment;
- prediction decision model;
- feedback reveal rules;
- audit event model.

Implemented foundation:

- the required `ArvProjectStatus`, `ImageTemperature`, `PredictionDecision`, `ActualOutcome`, and `TrialStatus` enums;
- minimal `ArvProject`, `ImageAsset`, `BlindAssignment`, `ArvTrial`, `TrialJudgement`, `PredictionResult`, and `FeedbackResult` entities;
- constructor validation for required identifiers, values, outcome names, timestamps, and opaque image storage keys;
- `IClock` and `SystemClock`, with domain services depending on the injected clock;
- secure assignment using `RandomNumberGenerator`;
- validation that an assignment contains one warm and one cold image with distinct identifiers;
- order-independent image-category handling;
- immutable assignment data and protection against replacing a project assignment after protocol lock;
- internal visibility for the protected Outcome-to-image mapping so it is not exposed through the public project model;
- explicit result types and safe domain error codes for expected failures;
- the main project path from `Draft` through `AwaitingFeedback`, plus `FeedbackRevealed` and `Completed`;
- feedback validation for project status, availability time, confirmed binary outcome, one-time reveal, and valid locked assignment;
- resolution of the image assigned to the confirmed actual outcome;
- recording of reveal time and hit/miss or `NoPrediction` semantics;
- dependency injection registration for the clock, assignment, and feedback services;
- a platform-neutral `net10.0` target used by domain tests without starting the MAUI host.

Remaining Stage 2 work:

- complete and test every allowed and forbidden project status transition;
- add the exceptional `Cancelled` and `Invalidated` transition rules;
- decide and implement whether any transition timestamps must be monotonic;
- add the remaining domain models needed by this stage, including protocol configuration and audit events;
- complete protocol validation across project, trial, judging, prediction, and feedback state;
- finalize and implement the scoring formula, tie handling, signal threshold, and `InvalidSession` behavior;
- prevent silent recalculation with a different scoring algorithm version;
- add deterministic coverage that guards against a hard-coded assignment branch without claiming statistical proof of randomness;
- add the remaining time-zone and daylight-saving boundary tests where applicable.

Required tests:

- assignment always uses one warm and one cold image;
- both outcomes receive different images;
- assignment distribution is not hard-coded;
- assignment cannot change after lock;
- feedback cannot be revealed too early;
- feedback cannot be revealed without an actual outcome;
- winner receives the correctly assigned image;
- invalid status transitions are rejected.

Exit criteria:

- domain tests pass;
- no domain rule depends on a MAUI control.

## Stage 3 — SQLite persistence

Deliverables:

- `AppDbContext`;
- EF Core entity configurations;
- initial migration;
- database initialization;
- project repository;
- image repository;
- transaction handling for protocol lock and assignment;
- local paths for image and drawing data.

Exit criteria:

- app can create, close, reopen, and reload a project;
- assignment and project lock are stored atomically;
- database migrations are repeatable;
- deletion behavior is explicit.

## Stage 4 — Project creation

Deliverables:

- project list;
- project editor;
- binary outcomes;
- event and feedback date selection;
- trial count;
- validation messages;
- protocol review;
- lock protocol action.

Validation examples:

- Outcome A and Outcome B must be different.
- Feedback time must be later than event start.
- Trial count must remain inside the configured range.
- At least one enabled warm and one enabled cold image must exist.
- Locked protocol fields become read-only.

Exit criteria:

- a project can be created and locked;
- locking generates and saves a blind assignment;
- UI does not show the assignment.

## Stage 5 — Image library

Deliverables:

- bundled image import;
- warm/cold metadata;
- image enable/disable;
- usage counters;
- secure internal storage names;
- optional duplicate detection based on a content hash.

Rules:

- do not reveal outcome mapping in filenames;
- do not log assignment details during session or judging;
- do not show both feedback images before judging is complete;
- selection should avoid immediately repeating recently used images when possible.

Exit criteria:

- app can select one valid image from each category;
- missing or unreadable images are handled safely;
- the original asset remains unchanged.

## Stage 6 — ARV trials

Deliverables:

- session preparation screen;
- configurable timer;
- text notes;
- drawing canvas;
- autosave;
- trial counter;
- pause and resume;
- trial completion;
- recovery after application suspension.

Exit criteria:

- every completed trial has timestamps;
- notes survive app suspension;
- drawings reload correctly;
- the app cannot accidentally reveal target assignments.

## Stage 7 — Self-judging

Deliverables:

- trial-by-trial review;
- both images displayed without outcome labels;
- selected image;
- confidence from `0.1` to `4.0`;
- progress tracking;
- final confirmation;
- prediction calculation.

Before implementation, finalize:

- scoring formula;
- tie handling;
- `NoPrediction` threshold;
- whether judgments can be changed before final submission;
- whether low-confidence trials are included.

Exit criteria:

- all trials must be judged or explicitly skipped;
- final submission is explicit;
- the algorithm version is persisted;
- a prediction cannot be silently recalculated using another version.

## Stage 8 — Feedback scheduling and reveal

Deliverables:

- local notification permission handling;
- notification scheduling;
- notification cancellation and rescheduling;
- notification deep link to the correct project;
- `AwaitingFeedback` state;
- actual winner selection;
- irreversible confirmation step;
- full-screen winner image;
- reveal timestamp;
- hit/miss result.

Core rule:

```text
The notification makes feedback available.
It does not decide who won.
```

Feedback flow:

```text
Notification
  -> Open project
  -> Enter actual winner
  -> Confirm
  -> Resolve assigned image
  -> Display full-screen feedback
  -> Persist reveal
  -> Complete project
```

Exit criteria:

- notification is scheduled using the configured local time;
- opening the notification navigates to the correct project when supported;
- early reveal is blocked;
- the revealed image matches the actual winner;
- feedback can be revealed only once;
- timestamps use `DateTimeOffset`.

## Stage 9 — Basic statistics

Deliverables:

- total completed predictions;
- hits;
- misses;
- no-prediction count;
- hit rate;
- results by event type;
- results by trial count;
- recent project history.

Statistical labels should be descriptive and must not imply scientific validation.

Exit criteria:

- statistics can be reconstructed from stored projects;
- deleted or invalidated sessions are handled consistently;
- division by zero is handled.

## Stage 10 — Audio and polish

Deliverables:

- local audio library;
- user-provided audio import;
- playback controls;
- session timer integration;
- interruption handling;
- accessibility labels;
- responsive tablet layout;
- final empty, loading, and error states.

Do not bundle third-party commercial recordings unless licensing explicitly permits distribution.

## 7. Notification constraints

Mobile operating systems do not guarantee that arbitrary background code will execute at an exact time.

For the MVP:

- schedule a local notification;
- treat delivery time as best effort;
- validate feedback availability when the user opens the app;
- derive state from current time as well as stored notification metadata;
- reschedule notifications after project date changes;
- restore scheduled notifications after device restart where platform support requires it.

The domain rule remains authoritative even if a notification is delayed or not delivered.

## 8. Testing strategy

### Unit tests

Prioritize:

- protocol validation;
- assignment;
- state transitions;
- scoring;
- winner-image resolution;
- feedback time rules;
- hit/miss calculation;
- date handling.

### Integration tests

Use a temporary SQLite database for:

- migrations;
- repository operations;
- transaction rollback;
- relation constraints;
- project reload;
- feedback persistence.

### Manual platform tests

Test on at least:

- Windows;
- one physical Android device;
- one Android emulator.

Notification tests should include:

- permission denied;
- permission granted;
- app in foreground;
- app in background;
- app terminated;
- feedback time passed while device was offline;
- time zone changed;
- event rescheduled;
- notification tapped after feedback was already revealed.

## 9. Initial backlog

### Must have

- [x] Create solution.
- [x] Add MAUI project.
- [x] Add xUnit test project.
- [ ] Configure central package management.
- [x] Configure MVVM Toolkit.
- [x] Configure MAUI Community Toolkit.
- [ ] Configure EF Core SQLite beyond the current package references.
- [x] Add the initial domain model foundation.
- [ ] Complete the remaining domain model and protocol state machine.
- [ ] Add warm/cold image library selection; assignment of an already selected valid pair is complete.
- [x] Add secure blind assignment.
- [x] Add the clock abstraction and feedback reveal domain rule.
- [ ] Add project CRUD.
- [ ] Add trial notes.
- [ ] Add drawing canvas.
- [ ] Add judging.
- [ ] Add prediction calculation.
- [ ] Add local notification.
- [ ] Add manual winner entry.
- [ ] Add winner-image reveal UI and persistence; winner-image resolution in the domain is complete.
- [ ] Add basic statistics.
- [ ] Complete tests for all critical rules; 19 assignment, transition, timing, and reveal tests currently pass.

### Should have

- [ ] Audio playback.
- [ ] Autosave indicator.
- [ ] Trial recovery.
- [ ] Recently used image avoidance.
- [ ] Export to JSON.
- [ ] Export to CSV.
- [ ] Backup to a local archive.

### Could have

- [ ] Training mode.
- [ ] External sports result API.
- [ ] Cloud backup.
- [ ] Advanced charts.
- [ ] External environmental correlations.

## 10. Definition of done

A feature is complete when:

- behavior matches the approved protocol;
- error and empty states exist;
- relevant unit tests pass;
- database changes include a migration;
- no hidden assignment leaks through UI, logs, filenames, or exceptions;
- async operations do not block the UI thread;
- platform permissions are handled;
- public behavior is documented;
- `dotnet build` and `dotnet test` pass.
