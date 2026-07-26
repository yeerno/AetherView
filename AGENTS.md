# AGENTS.md — Aether View

## Purpose

This file defines implementation rules for Codex and other coding agents working in the Aether View repository.

Read `README.md` and `PLAN.md` before making architectural changes.

## Product context

Aether View is an offline-first .NET MAUI application for conducting Associative Remote Viewing workflows.

The critical protocol property is blindness:

- outcome-to-image assignment must remain hidden during trials;
- assignment must remain hidden during judging;
- the winning image is revealed only after feedback is available and the actual winner is confirmed.

Treat any premature disclosure as a critical defect.

## Technology constraints

Use:

- .NET 10;
- .NET MAUI;
- C#;
- XAML;
- CommunityToolkit.Mvvm;
- CommunityToolkit.Maui;
- EF Core with SQLite;
- built-in dependency injection;
- `DateTimeOffset` for event timestamps.

The solution may contain at most two projects:

```text
AetherView.App
AetherView.Tests
```

Do not create additional projects unless the user explicitly changes this constraint.

## Architecture rules

Use a pragmatic MVVM architecture.

Production code stays in `AetherView.App` and is separated by folders and namespaces:

```text
Domain/
Data/
Services/
Features/
Controls/
Converters/
Resources/
```

Tests stay in `AetherView.Tests`.

Do not introduce:

- MediatR;
- a generic repository abstraction;
- an event bus;
- CQRS infrastructure;
- microservices;
- a backend;
- cloud synchronization;
- additional class-library projects;

unless a concrete requirement justifies the change and the user approves it.

## Dependency direction

Follow these rules:

- XAML pages bind to ViewModels.
- ViewModels depend on service interfaces and domain types.
- Domain rules do not depend on MAUI UI controls.
- SQLite code does not contain navigation or presentation logic.
- Platform-specific behavior is hidden behind interfaces.
- Hidden assignment access is isolated behind a dedicated service.
- Services are registered in `MauiProgram.cs`.
- Avoid static service locators.

## Feature structure

Prefer a feature folder:

```text
Features/Projects/
├── ProjectListPage.xaml
├── ProjectListPage.xaml.cs
├── ProjectListViewModel.cs
├── ProjectEditorPage.xaml
├── ProjectEditorPage.xaml.cs
└── ProjectEditorViewModel.cs
```

Keep code-behind limited to UI-specific behavior that cannot be expressed cleanly through binding.

Do not place business rules in code-behind.

## Domain rules

Model project status transitions explicitly.

Do not write:

```csharp
project.Status = ArvProjectStatus.Completed;
```

from arbitrary ViewModels.

Prefer:

```csharp
project.Complete(feedbackResult, clock.UtcNow);
```

or a dedicated domain service that validates the transition.

Critical rules:

1. A project has exactly two distinct outcomes.
2. A blind assignment uses one warm and one cold image.
3. Each outcome receives exactly one image.
4. Assignment uses cryptographically secure randomness.
5. Assignment becomes immutable after protocol lock.
6. Trials and judging must not expose outcome mapping.
7. Feedback cannot be revealed before `FeedbackAvailableAt`.
8. Feedback requires a confirmed actual outcome.
9. The resolved feedback image must match the actual winner.
10. Feedback reveal is recorded and cannot silently repeat.
11. Algorithm and protocol versions are persisted.
12. Invalid state transitions return an explicit failure.

## Randomness

Use `System.Security.Cryptography.RandomNumberGenerator`.

Do not use:

```csharp
new Random()
```

for blind assignment.

Randomness code must be unit tested for invariants, not for an exact random sequence.

Never claim that a small unit test statistically proves unbiased randomness.

## Date and time

Use `DateTimeOffset`.

Inject a clock abstraction:

```csharp
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
```

Avoid calling `DateTime.Now` or `DateTimeOffset.Now` throughout domain code.

Store timestamps in UTC where practical and retain the event's intended offset or time zone information when needed for display and rescheduling.

Test:

- feedback before availability;
- feedback at the exact availability time;
- feedback after availability;
- time zone changes;
- daylight-saving transitions where applicable.

## Blind assignment protection

Never expose hidden assignment through:

- page binding;
- ViewModel properties during trials or judging;
- navigation query parameters;
- filenames;
- debug labels;
- analytics;
- logs;
- exception messages;
- toast messages;
- test snapshots committed to the repository.

Do not name stored files:

```text
warm-outcome-a.jpg
cold-winner.jpg
```

Use opaque identifiers:

```text
f67da2f8-77f7-43b5-a55e-b9e84ee27688.webp
```

A category may exist in protected database metadata, but it must not leak through the session UI.

## Persistence

Use EF Core with SQLite.

Requirements:

- keep migrations in source control;
- use explicit entity configurations for non-trivial entities;
- add indexes for project status and feedback time;
- use transactions when locking a protocol and creating its assignment;
- configure delete behavior intentionally;
- do not run destructive schema recreation against a user database;
- do not use `EnsureCreated` after migrations are introduced;
- preserve user data during app upgrades.

Keep large binary drawing and image files outside normal database rows when practical. Store paths or opaque storage keys in SQLite.

## Notifications

Local notifications are reminders and are not a guaranteed exact-time execution mechanism.

When implementing notification behavior:

- request permission appropriately;
- handle permission denial;
- schedule by project identifier;
- support cancellation and rescheduling;
- verify current domain state when the notification is opened;
- do not reveal an image directly from notification payload data;
- do not place hidden assignment data in a notification;
- do not rely solely on a background callback to change project status.

The app must calculate whether feedback is available when opened.

## MVVM

Use `CommunityToolkit.Mvvm`.

Prefer:

```csharp
public partial class ProjectEditorViewModel : ObservableObject
{
    [ObservableProperty]
    private string name = string.Empty;

    [RelayCommand]
    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        // ...
    }
}
```

Guidelines:

- keep ViewModels focused on presentation state and orchestration;
- move protocol calculations into domain services;
- use `AsyncRelayCommand` through generated relay commands;
- prevent duplicate command execution;
- expose user-friendly validation errors;
- do not block with `.Result`, `.Wait()`, or `GetAwaiter().GetResult()`.

## XAML

Requirements:

- use compiled bindings where practical;
- set `x:DataType` on pages and data templates;
- use resource dictionaries for repeated styles;
- support light and dark themes;
- avoid hard-coded platform-specific dimensions;
- support phone and tablet layouts;
- include semantic descriptions for essential controls;
- keep the feedback image view visually focused.

Do not add Blazor Hybrid unless a specific screen demonstrates a clear need that XAML cannot satisfy economically.

## Error handling

Use explicit result types or well-defined exceptions at service boundaries.

User-facing errors should explain:

- what failed;
- whether data was saved;
- what the user can do next.

Do not expose:

- database SQL;
- physical file paths;
- stack traces;
- hidden image assignment;
- internal identifiers not needed by the user.

Log technical details locally only when logging is enabled and safe.

## Testing

Every critical domain behavior requires tests.

Minimum tests:

- valid assignment contains one warm and one cold image;
- both outcomes receive different images;
- insufficient image library fails;
- locked assignment cannot change;
- project cannot skip required statuses;
- reveal before feedback time fails;
- reveal without actual winner fails;
- Outcome A resolves its assigned image;
- Outcome B resolves its assigned image;
- feedback reveal timestamp is stored;
- repeated reveal is rejected or returns the existing immutable result;
- hit/miss is calculated correctly;
- `NoPrediction` is handled correctly;
- scoring algorithm version is persisted.

Use a temporary SQLite database for integration tests where persistence behavior matters.

Do not make tests depend on:

- current wall-clock time;
- network access;
- installed mobile emulators;
- a fixed random result.

## Package management

Prefer central package management through `Directory.Packages.props`.

Before adding a package:

1. Check whether .NET MAUI or the existing toolkit already provides the feature.
2. Verify that the package supports .NET 10 and target platforms.
3. Avoid abandoned packages.
4. Add only the minimum required package.
5. Document why it is needed.

Do not invent package versions. Use versions selected in the repository or explicitly verify current stable versions before updating them.

## Commands

Run from the repository root:

```powershell
dotnet restore
dotnet workload restore
dotnet build
dotnet test
```

Build Windows:

```powershell
dotnet build .\src\AetherView.App\AetherView.App.csproj `
  -f net10.0-windows10.0.19041.0
```

Build Android:

```powershell
dotnet build .\src\AetherView.App\AetherView.App.csproj `
  -f net10.0-android
```

## Change procedure

Before coding:

1. Read the relevant section of `PLAN.md`.
2. Identify affected protocol rules.
3. Inspect existing tests.
4. Keep the change inside the existing two-project structure.

After coding:

1. Format changed C# files.
2. Build the affected target.
3. Run tests.
4. Add or update tests.
5. Update documentation when behavior changes.
6. Review for blind-assignment leakage.
7. Report any build target that could not be tested.

## Definition of a safe change

A change is safe when:

- it preserves protocol blindness;
- it does not weaken feedback timing rules;
- it does not silently modify historical results;
- it does not add unnecessary architecture;
- it keeps offline workflows functional;
- it includes tests for changed domain behavior;
- it builds without introducing new warnings;
- it does not expose sensitive internal state.
