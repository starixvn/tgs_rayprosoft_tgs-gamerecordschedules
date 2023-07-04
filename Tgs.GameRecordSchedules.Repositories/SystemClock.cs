using Tgs.GameRecordSchedules.Core;

namespace Tgs.GameRecordSchedules.Repositories;

internal class SystemClock : ISystemClock
{
    public DateTimeOffset CurrentTime { get => DateTimeOffset.UtcNow; }
}
