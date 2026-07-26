namespace AetherView.App.Domain.Results;

public enum DomainError
{
    None,
    InvalidProjectIdentifier,
    InvalidImageSelection,
    ImageIdentifiersMustBeDistinct,
    AssignmentProjectMismatch,
    AssignmentLocked,
    InvalidPrediction,
    InvalidProjectStatus,
    InvalidTrialStatus,
    FeedbackNotAvailable,
    ActualOutcomeRequired,
    FeedbackAlreadyRevealed,
    InvalidAssignment
}
