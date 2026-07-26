using AetherView.App.Domain.Entities;
using AetherView.App.Domain.Enums;
using AetherView.App.Domain.Results;
using AetherView.App.Services.Randomization;

namespace AetherView.Tests;

internal static class TestProjectFactory
{
    public static ImageAsset CreateImage(
        ImageTemperature temperature,
        DateTimeOffset createdAt,
        Guid? id = null)
    {
        return new ImageAsset(
            id ?? Guid.NewGuid(),
            $"{Guid.NewGuid():D}.webp",
            temperature,
            createdAt);
    }

    public static ArvProject CreateDraft(
        DateTimeOffset feedbackAvailableAt,
        Guid? id = null)
    {
        return new ArvProject(
            id ?? Guid.NewGuid(),
            "Test project",
            "Which outcome will occur?",
            "Outcome A",
            "Outcome B",
            feedbackAvailableAt.AddHours(-1),
            feedbackAvailableAt,
            4,
            "protocol-v1",
            "scoring-v1",
            feedbackAvailableAt.AddHours(-2));
    }

    public static ArvProject CreateAwaitingFeedback(
        TestClock clock,
        DateTimeOffset feedbackAvailableAt,
        out BlindAssignment assignment,
        PredictionDecision predictionDecision = PredictionDecision.OutcomeA)
    {
        ArvProject project = CreateDraft(feedbackAvailableAt);
        ImageAsset warmImage = CreateImage(ImageTemperature.Warm, clock.UtcNow);
        ImageAsset coldImage = CreateImage(ImageTemperature.Cold, clock.UtcNow);
        BlindAssignmentService assignmentService = new(clock);
        DomainResult<BlindAssignment> assignmentResult = assignmentService.Create(
            project.Id,
            warmImage,
            coldImage);

        assignment = assignmentResult.Value
            ?? throw new InvalidOperationException("Test assignment creation failed.");

        EnsureSuccess(project.LockProtocol(assignment, clock.UtcNow));
        EnsureSuccess(project.StartTrials(clock.UtcNow));
        EnsureSuccess(project.CompleteTrials(clock.UtcNow));
        EnsureSuccess(project.StartJudging(clock.UtcNow));

        PredictionResult predictionResult = new(
            Guid.NewGuid(),
            project.Id,
            3.5m,
            2.0m,
            predictionDecision,
            1.5m,
            project.ScoringAlgorithmVersion,
            clock.UtcNow);

        EnsureSuccess(project.RecordPrediction(predictionResult, clock.UtcNow));
        EnsureSuccess(project.MarkAwaitingEvent(clock.UtcNow));
        EnsureSuccess(project.MarkAwaitingFeedback(clock.UtcNow));

        return project;
    }

    private static void EnsureSuccess(DomainResult result)
    {
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                $"Test project setup failed with domain error {result.Error}.");
        }
    }
}
