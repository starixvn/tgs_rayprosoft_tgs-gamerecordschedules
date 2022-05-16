using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecordSchedules.Client
{
    internal interface IGameRecordSchedulesGrpc
    {
        public ValueTask<GameRecordScheduleGetReply?> GameRecordScheduleAdd(GameRecordScheduleAddRequest request, CancellationToken cancellationToken = default);

        public ValueTask<GameRecordScheduleGetReply?> GameRecordScheduleGet(GameRecordScheduleGetRequest request, CancellationToken cancellationToken = default);

        public ValueTask<GameRecordScheduleGetReply?> GameRecordScheduleUpdate(GameRecordScheduleUpdateRequest request, CancellationToken cancellationToken = default);
    }
}
