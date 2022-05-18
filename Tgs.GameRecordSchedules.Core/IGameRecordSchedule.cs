using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core
{
    public interface IGameRecordSchedule
    {
        public ValueTask<Guid> AddAsync(GameRecordScheduleAdd data, CancellationToken cancellationToken = default);

        public ValueTask<GameRecordScheduleGet?> GetAsync(Guid gameRecordScheduleId, CancellationToken cancellationToken = default);

        public ValueTask<GameRecordScheduleGet?> GetAsync(string gameProviderCode, string brandCode, string? gameProviderAgentId, string? currencyCode, CancellationToken cancellationToken = default);

        public ValueTask<Guid?> GetGameRecordScheduleIdAsync(string gameProviderCode, string brandCode, string? gameProviderAgentId, string? currencyCode, CancellationToken cancellationToken = default);

        public ValueTask UpdateAsync(GameRecordScheduleUpdate data, CancellationToken cancellationToken = default);
    }
}
