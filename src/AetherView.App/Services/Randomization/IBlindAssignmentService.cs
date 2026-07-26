using AetherView.App.Domain.Entities;
using AetherView.App.Domain.Results;

namespace AetherView.App.Services.Randomization;

public interface IBlindAssignmentService
{
    DomainResult<BlindAssignment> Create(
        Guid projectId,
        ImageAsset firstImage,
        ImageAsset secondImage);
}
