using AetherView.App.Services.Clock;

namespace AetherView.Tests;

internal sealed class TestClock(DateTimeOffset utcNow) : IClock
{
    public DateTimeOffset UtcNow { get; set; } = utcNow;
}
