using AetherView.App.Domain.Entities;
using AetherView.App.Domain.Enums;
using AetherView.App.Domain.Results;

namespace AetherView.App.Services.Feedback;

public interface IFeedbackRevealService
{
    DomainResult<FeedbackResult> Reveal(
        ArvProject project,
        ActualOutcome actualOutcome);
}
