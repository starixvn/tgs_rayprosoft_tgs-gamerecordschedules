namespace Tgs.GameRecordSchedules.Repositories;

public interface ISystemClock
{
    DateTimeOffset UtcNow { get; }
}
