# Aether View

**Aether View** is a cross-platform application for conducting and documenting **Associative Remote Viewing (ARV)** sessions.

The application is built with **.NET MAUI and .NET 10** and is intended to run on Android, iOS, Windows, and macOS. Its initial implementation focuses on a simple, offline-first workflow for binary predictions, session notes, drawings, self-judging, scheduled feedback, and local statistics.

> Aether View is an experimental and training tool. It does not guarantee accurate predictions and is not an automated betting system or financial advisory product.

## Product goal

Aether View helps a user conduct a repeatable ARV protocol without assistance from another person.

The core workflow is:

1. Create a binary event, for example a sports match.
2. Define Outcome A and Outcome B.
3. Randomly assign one warm image and one cold image to the outcomes.
4. Keep the assignment hidden during the session and judging.
5. Complete the configured number of trials.
6. Judge the trials and calculate the prediction.
7. Wait until the scheduled feedback time.
8. Enter the actual winner.
9. Display the image assigned to the winning outcome.
10. Save the complete result and update statistics.

## Technology

- .NET 10
- .NET MAUI
- C#
- XAML
- MVVM
- CommunityToolkit.Mvvm
- CommunityToolkit.Maui
- SQLite
- Entity Framework Core
- Local notifications
- SecureStorage
- Optional drawing canvas
- Optional local audio playback

## Supported platforms

The solution is intended to support:

- Android
- Windows
- iOS
- macOS through Mac Catalyst

Building iOS and Mac Catalyst targets requires macOS and the appropriate Apple tooling.

## Solution structure

The solution intentionally contains no more than two projects:

```text
AetherView/
├── AetherView.slnx
├── global.json
├── Directory.Build.props
├── Directory.Packages.props
├── README.md
├── PLAN.md
├── AGENTS.md
├── src/
│   └── AetherView.App/
│       ├── AetherView.App.csproj
│       ├── App.xaml
│       ├── AppShell.xaml
│       ├── Domain/
│       ├── Data/
│       ├── Services/
│       ├── Features/
│       ├── Controls/
│       ├── Converters/
│       ├── Resources/
│       └── Platforms/
└── tests/
    └── AetherView.Tests/
        └── AetherView.Tests.csproj
```

### AetherView.App

Contains all production code:

- MAUI UI;
- domain models and rules;
- SQLite persistence;
- notification scheduling;
- image selection and blind assignment;
- session and judging workflows;
- feedback reveal;
- statistics.

### AetherView.Tests

Contains unit tests for code that does not require a running MAUI host:

- random assignment rules;
- status transitions;
- scoring;
- `NoPrediction` rules;
- feedback availability;
- winner-image selection;
- date and time handling;
- protocol validation.

## Internal application structure

The production project uses feature-oriented folders while retaining a simple MVVM architecture.

```text
AetherView.App/
├── Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── ValueObjects/
│   └── Rules/
├── Data/
│   ├── AppDbContext.cs
│   ├── Configurations/
│   ├── Migrations/
│   └── Repositories/
├── Services/
│   ├── Audio/
│   ├── Clock/
│   ├── Images/
│   ├── Notifications/
│   ├── Randomization/
│   └── Storage/
├── Features/
│   ├── Dashboard/
│   ├── Projects/
│   ├── Session/
│   ├── Judging/
│   ├── Feedback/
│   ├── Statistics/
│   └── Settings/
└── Resources/
    ├── Images/
    ├── Raw/
    └── Styles/
```

Each feature can contain its own page, ViewModel, and feature-specific models:

```text
Features/Feedback/
├── FeedbackPage.xaml
├── FeedbackPage.xaml.cs
├── FeedbackViewModel.cs
└── FeedbackNavigationParameters.cs
```

## Key domain concepts

### Project

Represents one binary prediction event.

Example:

```text
Question: Who will win the match?
Outcome A: Player A
Outcome B: Player B
Event start: 2026-07-26 18:00 +02:00
Feedback available: 2026-07-26 21:30 +02:00
Planned trials: 12
```

### ImageAsset

Represents one image stored in the local image library.

The initial library is already divided into:

- `Warm`
- `Cold`

An image should use an opaque identifier and must not have a filename that reveals its assigned outcome.

### BlindAssignment

Stores the hidden mapping between both outcomes and selected images.

Example:

```text
Outcome A -> Warm image
Outcome B -> Cold image
```

or:

```text
Outcome A -> Cold image
Outcome B -> Warm image
```

The assignment is generated using cryptographically secure randomness and becomes immutable when the protocol is locked.

### Trial

Represents one perception attempt and may contain:

- start and end time;
- text notes;
- drawing path or drawing file;
- completion status;
- optional session duration.

### Judgement

Represents the user's assessment of a trial:

- selected image;
- confidence from `0.1` to `4.0`;
- timestamp;
- optional notes.

### PredictionResult

Stores:

- raw points for each image;
- selected outcome;
- signal difference;
- algorithm version;
- optional `NoPrediction` decision.

### FeedbackResult

Stores:

- actual event result;
- winning outcome;
- image shown as feedback;
- scheduled feedback time;
- actual reveal time;
- whether the prediction was correct.

## Project status

Suggested project lifecycle:

```text
Draft
  -> ProtocolLocked
  -> TrialsInProgress
  -> TrialsCompleted
  -> JudgingInProgress
  -> PredictionCalculated
  -> AwaitingEvent
  -> AwaitingFeedback
  -> FeedbackRevealed
  -> Completed
```

Additional terminal states:

```text
Cancelled
Invalidated
```

Status transitions must be enforced by domain rules, not only by hiding buttons in the UI.

## Feedback notification workflow

The MVP uses a local notification.

1. The user defines `FeedbackAvailableAt`.
2. The application schedules a local notification.
3. At the scheduled time, the notification informs the user that feedback is available.
4. The user opens the project.
5. The user selects the actual winner.
6. The application asks for confirmation.
7. The application reveals the image assigned to the winning outcome.
8. The reveal timestamp and shown image are stored.
9. The application updates project statistics.

A scheduled time does not automatically determine the winner. In the MVP, the actual result is entered manually.

Suggested notification text:

```text
Aether View feedback is ready.
Enter the final result and reveal the winner's image.
```

## Time handling

Use `DateTimeOffset` for event and feedback timestamps.

Do not store event dates as local `DateTime` values without an offset. Sports events may occur in another time zone, and daylight-saving transitions must not alter the intended feedback time.

Recommended fields:

```csharp
DateTimeOffset EventStartsAt
DateTimeOffset FeedbackAvailableAt
DateTimeOffset? NotificationOpenedAt
DateTimeOffset? FeedbackRevealedAt
```

## Blindness and data protection

During trials and judging:

- the UI must not receive outcome-to-image mapping;
- ViewModels must not expose hidden assignments;
- filenames must not contain `warm`, `cold`, `outcome-a`, or `outcome-b`;
- logs must not print hidden assignments;
- navigation parameters must not include hidden mapping;
- image assignment must not be visible in debug labels;
- feedback cannot be revealed before its configured time.

The database may contain the hidden assignment, but access to it must be isolated behind a dedicated service.

## Offline-first approach

The MVP works without a backend.

Stored locally:

- projects;
- trials;
- notes;
- drawings;
- image metadata;
- assignments;
- judgments;
- results;
- settings;
- notification metadata.

Cloud synchronization and remote result APIs are outside the MVP.

## Getting started

### Requirements

Install:

- .NET 10 SDK;
- .NET MAUI workload;
- Android SDK for Android development;
- Windows App SDK requirements for Windows development;
- Xcode on macOS for iOS and Mac Catalyst development.

Verify the SDK:

```powershell
dotnet --info
dotnet --list-sdks
```

Install or repair the MAUI workload on Windows:

```powershell
dotnet workload install maui
dotnet workload list
```

Restore workloads declared by the project:

```powershell
dotnet workload restore
```

### Restore and build

From the solution root:

```powershell
dotnet restore
dotnet build
dotnet test
```

### Run on Windows

```powershell
dotnet build .\src\AetherView.App\AetherView.App.csproj `
  -t:Run `
  -f net10.0-windows10.0.19041.0
```

### Build Android

```powershell
dotnet build .\src\AetherView.App\AetherView.App.csproj `
  -f net10.0-android
```

To run Android from the CLI, start an emulator or connect a device first, then use the target or tooling available in the installed MAUI workload.

## Development principles

- Keep the solution limited to two projects.
- Keep domain rules independent from pages and controls.
- Prefer interfaces for platform-specific services.
- Do not introduce a mediator, event bus, or generic repository without a demonstrated need.
- Use asynchronous I/O.
- Use cancellation tokens in operations that can take noticeable time.
- Do not expose blind assignments before feedback reveal.
- Add tests for every change to scoring, randomization, status transitions, and reveal rules.
- Keep database migrations in source control.
- Treat notifications as reminders, not guaranteed exact-time background execution.
- Do not add online dependencies to an offline feature.

## Documentation

- [PLAN.md](PLAN.md) describes delivery stages and the MVP backlog.
- [AGENTS.md](AGENTS.md) contains rules for Codex and other coding agents.
