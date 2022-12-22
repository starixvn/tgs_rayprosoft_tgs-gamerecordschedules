using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecordSchedules.Client;

public interface IGameRecordSchedulesClient
{
    public ValueTask<GameRecordScheduleAddReply> GameRecordScheduleAddAsync(GameRecordScheduleAddRequest request, CancellationToken cancellationToken = default);

    public ValueTask<GameRecordScheduleReply?> GameRecordScheduleGetAsync(GameRecordScheduleGetRequest request, CancellationToken cancellationToken = default);

    public ValueTask<GameRecordScheduleUpdateReply> GameRecordScheduleUpdateAsync(GameRecordScheduleUpdateRequest request, CancellationToken cancellationToken = default);
}
