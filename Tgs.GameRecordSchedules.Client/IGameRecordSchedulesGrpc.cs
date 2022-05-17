using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecordSchedules.Client
{
    public interface IGameRecordSchedulesGrpc
    {
        public ValueTask<GameRecordScheduleAddReply> GameRecordScheduleAddAsync(GameRecordScheduleAddRequest request, CancellationToken cancellationToken = default);

        public ValueTask<GameRecordScheduleGetReply> GameRecordScheduleGetAsync(GameRecordScheduleGetRequest request, CancellationToken cancellationToken = default);

        public ValueTask<GameRecordScheduleUpdateReply> GameRecordScheduleUpdateAsync(GameRecordScheduleUpdateRequest request, CancellationToken cancellationToken = default);
    }
}
