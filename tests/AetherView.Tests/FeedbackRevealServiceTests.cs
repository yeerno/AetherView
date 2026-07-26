using AetherView.App.Domain.Entities;
using AetherView.App.Domain.Enums;
using AetherView.App.Domain.Results;
using AetherView.App.Services.Feedback;

namespace AetherView.Tests;

public sealed class FeedbackRevealServiceTests
{
    private static readonly DateTimeOffset FeedbackAvailableAt =
        new(2026, 7, 26, 20, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Reveal_BeforeFeedbackAvailableAt_IsRejected()
    {
        TestClock clock = new(FeedbackAvailableAt.AddTicks(-1));
        ArvProject project = TestProjectFactory.CreateAwaitingFeedback(
            clock,
            FeedbackAvailableAt,
            out _);
        FeedbackRevealService service = new(clock);

        DomainResult<FeedbackResult> result = service.Reveal(
            project,
            ActualOutcome.OutcomeA);

        Assert.False(result.IsSuccess);
        Assert.Equal(DomainError.FeedbackNotAvailable, result.Error);
        Assert.Null(result.Value);
        Assert.Equal(ArvProjectStatus.AwaitingFeedback, project.Status);
    }

    [Fact]
    public void Reveal_WithoutActualOutcome_IsRejected()
    {
        TestClock clock = new(FeedbackAvailableAt);
        ArvProject project = TestProjectFactory.CreateAwaitingFeedback(
            clock,
            FeedbackAvailableAt,
            out _);
        FeedbackRevealService service = new(clock);

        DomainResult<FeedbackResult> result = service.Reveal(
            project,
            ActualOutcome.Unknown);

        Assert.False(result.IsSuccess);
        Assert.Equal(DomainError.ActualOutcomeRequired, result.Error);
        Assert.Null(result.Value);
        Assert.Equal(ArvProjectStatus.AwaitingFeedback, project.Status);
    }

    [Fact]
    public void Reveal_WhenFeedbackWasAlreadyRevealed_IsRejected()
    {
        TestClock clock = new(FeedbackAvailableAt);
        ArvProject project = TestProjectFactory.CreateAwaitingFeedback(
            clock,
            FeedbackAvailableAt,
            out _);
        FeedbackRevealService service = new(clock);
        DomainResult<FeedbackResult> firstResult = service.Reveal(
            project,
            ActualOutcome.OutcomeA);

        clock.UtcNow = clock.UtcNow.AddMinutes(1);
        DomainResult<FeedbackResult> repeatedResult = service.Reveal(
            project,
            ActualOutcome.OutcomeA);

        Assert.True(firstResult.IsSuccess);
        Assert.False(repeatedResult.IsSuccess);
        Assert.Equal(DomainError.FeedbackAlreadyRevealed, repeatedResult.Error);
        Assert.Null(repeatedResult.Value);
        Assert.Same(firstResult.Value, project.FeedbackResult);
    }

    [Fact]
    public void Reveal_ForOutcomeA_ReturnsImageAssignedToOutcomeA()
    {
        TestClock clock = new(FeedbackAvailableAt);
        ArvProject project = TestProjectFactory.CreateAwaitingFeedback(
            clock,
            FeedbackAvailableAt,
            out BlindAssignment assignment);
        FeedbackRevealService service = new(clock);

        DomainResult<FeedbackResult> result = service.Reveal(
            project,
            ActualOutcome.OutcomeA);

        FeedbackResult feedbackResult = Assert.IsType<FeedbackResult>(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Equal(assignment.OutcomeAImageId, feedbackResult.RevealedImageId);
        Assert.Equal(ActualOutcome.OutcomeA, feedbackResult.ActualOutcome);
    }

    [Fact]
    public void Reveal_ForOutcomeB_ReturnsImageAssignedToOutcomeB()
    {
        TestClock clock = new(FeedbackAvailableAt);
        ArvProject project = TestProjectFactory.CreateAwaitingFeedback(
            clock,
            FeedbackAvailableAt,
            out BlindAssignment assignment);
        FeedbackRevealService service = new(clock);

        DomainResult<FeedbackResult> result = service.Reveal(
            project,
            ActualOutcome.OutcomeB);

        FeedbackResult feedbackResult = Assert.IsType<FeedbackResult>(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Equal(assignment.OutcomeBImageId, feedbackResult.RevealedImageId);
        Assert.Equal(ActualOutcome.OutcomeB, feedbackResult.ActualOutcome);
    }

    [Fact]
    public void Reveal_ExactlyAtFeedbackAvailableAt_SucceedsAndRecordsTimestamp()
    {
        TestClock clock = new(FeedbackAvailableAt);
        ArvProject project = TestProjectFactory.CreateAwaitingFeedback(
            clock,
            FeedbackAvailableAt,
            out _);
        FeedbackRevealService service = new(clock);

        DomainResult<FeedbackResult> result = service.Reveal(
            project,
            ActualOutcome.OutcomeA);

        FeedbackResult feedbackResult = Assert.IsType<FeedbackResult>(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Equal(FeedbackAvailableAt, feedbackResult.RevealedAt);
        Assert.Equal(FeedbackAvailableAt, feedbackResult.AvailableAt);
        Assert.Equal(ArvProjectStatus.FeedbackRevealed, project.Status);
        Assert.Same(feedbackResult, project.FeedbackResult);
    }

    [Fact]
    public void Reveal_AfterFeedbackAvailableAt_Succeeds()
    {
        TestClock clock = new(FeedbackAvailableAt.AddHours(3));
        ArvProject project = TestProjectFactory.CreateAwaitingFeedback(
            clock,
            FeedbackAvailableAt,
            out _);
        FeedbackRevealService service = new(clock);

        DomainResult<FeedbackResult> result = service.Reveal(
            project,
            ActualOutcome.OutcomeB);

        FeedbackResult feedbackResult = Assert.IsType<FeedbackResult>(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Equal(clock.UtcNow, feedbackResult.RevealedAt);
    }

    [Fact]
    public void Reveal_AtSameInstantWithDifferentOffset_Succeeds()
    {
        DateTimeOffset availableAtWithOffset =
            new(2026, 7, 26, 22, 0, 0, TimeSpan.FromHours(2));
        TestClock clock = new(FeedbackAvailableAt);
        ArvProject project = TestProjectFactory.CreateAwaitingFeedback(
            clock,
            availableAtWithOffset,
            out _);
        FeedbackRevealService service = new(clock);

        DomainResult<FeedbackResult> result = service.Reveal(
            project,
            ActualOutcome.OutcomeA);

        Assert.True(result.IsSuccess);
        Assert.Equal(clock.UtcNow, result.Value?.RevealedAt);
    }

    [Theory]
    [InlineData(ActualOutcome.OutcomeA, true)]
    [InlineData(ActualOutcome.OutcomeB, false)]
    public void Reveal_RecordsPredictionAccuracy(
        ActualOutcome actualOutcome,
        bool expectedAccuracy)
    {
        TestClock clock = new(FeedbackAvailableAt);
        ArvProject project = TestProjectFactory.CreateAwaitingFeedback(
            clock,
            FeedbackAvailableAt,
            out _);
        FeedbackRevealService service = new(clock);

        DomainResult<FeedbackResult> result = service.Reveal(project, actualOutcome);

        Assert.Equal(expectedAccuracy, result.Value?.WasPredictionCorrect);
    }

    [Fact]
    public void Reveal_ForNoPrediction_DoesNotRecordAHitOrMiss()
    {
        TestClock clock = new(FeedbackAvailableAt);
        ArvProject project = TestProjectFactory.CreateAwaitingFeedback(
            clock,
            FeedbackAvailableAt,
            out _,
            PredictionDecision.NoPrediction);
        FeedbackRevealService service = new(clock);

        DomainResult<FeedbackResult> result = service.Reveal(
            project,
            ActualOutcome.OutcomeA);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value?.WasPredictionCorrect);
        Assert.Equal("scoring-v1", project.PredictionResult?.AlgorithmVersion);
    }
}
