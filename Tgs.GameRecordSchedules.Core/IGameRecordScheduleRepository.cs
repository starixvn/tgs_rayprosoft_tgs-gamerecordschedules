using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core;

public interface IGameRecordScheduleRepository
{
    ValueTask<Guid> AddAsync(
        GameRecordScheduleAdd source,
        CancellationToken cancellationToken = default);

    ValueTask<GameRecordScheduleGet?> GetAsync(
        Guid gameRecordScheduleId,
        CancellationToken cancellationToken = default);

    ValueTask<GameRecordScheduleGet?> GetAsync(
        string gameProviderCode,
        string? brandCode,
        string? gameProviderAgentId,
        string? currencyCode,
        CancellationToken cancellationToken = default);

    ValueTask<Guid?> GetGameRecordScheduleIdAsync(
        string gameProviderCode,
        string? brandCode,
        string? gameProviderAgentId,
        string? currencyCode,
        CancellationToken cancellationToken = default);

    ValueTask InitDBAsync(
        CancellationToken cancellationToken = default);

    ValueTask UpdateAsync(
        GameRecordScheduleUpdate source,
        CancellationToken cancellationToken = default);
}
