namespace Tgs.GameRecordSchedules.Repositories;

internal class SystemClock : ISystemClock
{
    DateTimeOffset ISystemClock.UtcNow { get => DateTimeOffset.UtcNow; }

    public DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;
}
