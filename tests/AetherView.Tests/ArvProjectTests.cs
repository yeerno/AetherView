using AetherView.App.Domain.Entities;
using AetherView.App.Domain.Enums;
using AetherView.App.Domain.Results;

namespace AetherView.Tests;

public sealed class ArvProjectTests
{
    [Fact]
    public void StartTrials_FromDraft_CannotSkipProtocolLock()
    {
        DateTimeOffset now = new(2026, 7, 26, 12, 0, 0, TimeSpan.Zero);
        ArvProject project = TestProjectFactory.CreateDraft(now.AddHours(2));

        DomainResult result = project.StartTrials(now);

        Assert.False(result.IsSuccess);
        Assert.Equal(DomainError.InvalidProjectStatus, result.Error);
        Assert.Equal(ArvProjectStatus.Draft, project.Status);
    }
}
