using System.Security.Cryptography;
using AetherView.App.Domain.Entities;
using AetherView.App.Domain.Enums;
using AetherView.App.Domain.Results;
using AetherView.App.Services.Clock;

namespace AetherView.App.Services.Randomization;

public sealed class BlindAssignmentService(IClock clock) : IBlindAssignmentService
{
    private readonly IClock clock = clock ?? throw new ArgumentNullException(nameof(clock));

    public DomainResult<BlindAssignment> Create(
        Guid projectId,
        ImageAsset firstImage,
        ImageAsset secondImage)
    {
        ArgumentNullException.ThrowIfNull(firstImage);
        ArgumentNullException.ThrowIfNull(secondImage);

        if (projectId == Guid.Empty)
        {
            return DomainResult<BlindAssignment>.Failure(DomainError.InvalidProjectIdentifier);
        }

        if (firstImage.Id == secondImage.Id)
        {
            return DomainResult<BlindAssignment>.Failure(
                DomainError.ImageIdentifiersMustBeDistinct);
        }

        if (firstImage.Temperature == secondImage.Temperature)
        {
            return DomainResult<BlindAssignment>.Failure(DomainError.InvalidImageSelection);
        }

        ImageAsset warmImage = firstImage.Temperature is ImageTemperature.Warm
            ? firstImage
            : secondImage;
        ImageAsset coldImage = firstImage.Temperature is ImageTemperature.Cold
            ? firstImage
            : secondImage;

        bool warmImageIsAssignedToOutcomeA = RandomNumberGenerator.GetInt32(2) is 0;
        Guid outcomeAImageId = warmImageIsAssignedToOutcomeA
            ? warmImage.Id
            : coldImage.Id;
        Guid outcomeBImageId = warmImageIsAssignedToOutcomeA
            ? coldImage.Id
            : warmImage.Id;

        BlindAssignment assignment = new(
            Guid.NewGuid(),
            projectId,
            warmImage.Id,
            coldImage.Id,
            outcomeAImageId,
            outcomeBImageId,
            clock.UtcNow);

        return DomainResult<BlindAssignment>.Success(assignment);
    }
}
