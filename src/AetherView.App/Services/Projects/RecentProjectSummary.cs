using AetherView.App.Domain.Enums;

namespace AetherView.App.Services.Projects;

public sealed record RecentProjectSummary(
    Guid Id,
    string Name,
    string Question,
    ArvProjectStatus Status,
    DateTimeOffset EventStartsAt,
    DateTimeOffset UpdatedAt);
