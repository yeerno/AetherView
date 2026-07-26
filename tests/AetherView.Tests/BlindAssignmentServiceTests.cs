using AetherView.App.Domain.Entities;
using AetherView.App.Domain.Enums;
using AetherView.App.Domain.Results;
using AetherView.App.Services.Randomization;

namespace AetherView.Tests;

public sealed class BlindAssignmentServiceTests
{
    private static readonly DateTimeOffset TestTime =
        new(2026, 7, 26, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithWarmAndColdImage_UsesOneImageOfEachTemperature()
    {
        TestClock clock = new(TestTime);
        ImageAsset warmImage = TestProjectFactory.CreateImage(
            ImageTemperature.Warm,
            TestTime);
        ImageAsset coldImage = TestProjectFactory.CreateImage(
            ImageTemperature.Cold,
            TestTime);
        BlindAssignmentService service = new(clock);

        DomainResult<BlindAssignment> result = service.Create(
            Guid.NewGuid(),
            warmImage,
            coldImage);

        BlindAssignment assignment = Assert.IsType<BlindAssignment>(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Equal(warmImage.Id, assignment.WarmImageId);
        Assert.Equal(coldImage.Id, assignment.ColdImageId);
    }

    [Theory]
    [InlineData(ImageTemperature.Warm)]
    [InlineData(ImageTemperature.Cold)]
    public void Create_WithImagesOfTheSameTemperature_IsRejected(
        ImageTemperature temperature)
    {
        TestClock clock = new(TestTime);
        ImageAsset firstImage = TestProjectFactory.CreateImage(temperature, TestTime);
        ImageAsset secondImage = TestProjectFactory.CreateImage(temperature, TestTime);
        BlindAssignmentService service = new(clock);

        DomainResult<BlindAssignment> result = service.Create(
            Guid.NewGuid(),
            firstImage,
            secondImage);

        Assert.False(result.IsSuccess);
        Assert.Equal(DomainError.InvalidImageSelection, result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Create_WithDuplicateImageIdentifiers_IsRejected()
    {
        TestClock clock = new(TestTime);
        Guid duplicateId = Guid.NewGuid();
        ImageAsset warmImage = TestProjectFactory.CreateImage(
            ImageTemperature.Warm,
            TestTime,
            duplicateId);
        ImageAsset coldImage = TestProjectFactory.CreateImage(
            ImageTemperature.Cold,
            TestTime,
            duplicateId);
        BlindAssignmentService service = new(clock);

        DomainResult<BlindAssignment> result = service.Create(
            Guid.NewGuid(),
            warmImage,
            coldImage);

        Assert.False(result.IsSuccess);
        Assert.Equal(DomainError.ImageIdentifiersMustBeDistinct, result.Error);
        Assert.Null(result.Value);
    }

    [Fact]
    public void Create_AssignsExactlyOneDistinctImageToEachOutcome()
    {
        TestClock clock = new(TestTime);
        ImageAsset warmImage = TestProjectFactory.CreateImage(
            ImageTemperature.Warm,
            TestTime);
        ImageAsset coldImage = TestProjectFactory.CreateImage(
            ImageTemperature.Cold,
            TestTime);
        BlindAssignmentService service = new(clock);

        DomainResult<BlindAssignment> result = service.Create(
            Guid.NewGuid(),
            warmImage,
            coldImage);

        BlindAssignment assignment = Assert.IsType<BlindAssignment>(result.Value);
        Guid[] assignedImageIds =
        [
            assignment.OutcomeAImageId,
            assignment.OutcomeBImageId
        ];

        Assert.True(result.IsSuccess);
        Assert.NotEqual(assignment.OutcomeAImageId, assignment.OutcomeBImageId);
        Assert.Contains(warmImage.Id, assignedImageIds);
        Assert.Contains(coldImage.Id, assignedImageIds);
    }

    [Fact]
    public void Create_AcceptsWarmAndColdImagesInEitherParameterOrder()
    {
        TestClock clock = new(TestTime);
        ImageAsset warmImage = TestProjectFactory.CreateImage(
            ImageTemperature.Warm,
            TestTime);
        ImageAsset coldImage = TestProjectFactory.CreateImage(
            ImageTemperature.Cold,
            TestTime);
        BlindAssignmentService service = new(clock);

        DomainResult<BlindAssignment> result = service.Create(
            Guid.NewGuid(),
            coldImage,
            warmImage);

        BlindAssignment assignment = Assert.IsType<BlindAssignment>(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Equal(warmImage.Id, assignment.WarmImageId);
        Assert.Equal(coldImage.Id, assignment.ColdImageId);
    }

    [Fact]
    public void LockProtocol_WhenAssignmentIsAlreadyLocked_CannotReplaceIt()
    {
        TestClock clock = new(TestTime);
        ArvProject project = TestProjectFactory.CreateDraft(TestTime.AddHours(2));
        BlindAssignmentService service = new(clock);
        BlindAssignment firstAssignment = CreateAssignment(service, project.Id);
        BlindAssignment replacementAssignment = CreateAssignment(service, project.Id);

        DomainResult firstLockResult = project.LockProtocol(firstAssignment, clock.UtcNow);
        DomainResult replacementResult = project.LockProtocol(
            replacementAssignment,
            clock.UtcNow);

        Assert.True(firstLockResult.IsSuccess);
        Assert.False(replacementResult.IsSuccess);
        Assert.Equal(DomainError.AssignmentLocked, replacementResult.Error);
        Assert.Same(firstAssignment, project.ProtectedAssignment);
    }

    private static BlindAssignment CreateAssignment(
        BlindAssignmentService service,
        Guid projectId)
    {
        ImageAsset warmImage = TestProjectFactory.CreateImage(
            ImageTemperature.Warm,
            TestTime);
        ImageAsset coldImage = TestProjectFactory.CreateImage(
            ImageTemperature.Cold,
            TestTime);

        DomainResult<BlindAssignment> result = service.Create(
            projectId,
            warmImage,
            coldImage);

        return result.Value
            ?? throw new InvalidOperationException("Test assignment creation failed.");
    }
}
