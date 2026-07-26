namespace AetherView.App.Services.Clock;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
