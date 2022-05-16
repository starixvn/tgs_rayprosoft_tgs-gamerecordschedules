using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecordSchedules.Client
{
    internal class GameRecordSchedulesGrpc : IGameRecordSchedulesGrpc
    {
        private readonly GameRecordSchedulesService.GameRecordSchedulesServiceClient _client;

        public GameRecordSchedulesGrpc(GameRecordSchedulesService.GameRecordSchedulesServiceClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<GameRecordScheduleGetReply?> GameRecordScheduleAdd(GameRecordScheduleAddRequest request, CancellationToken cancellationToken = default)
        {
            return await _client.GameRecordScheduleAddAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask<GameRecordScheduleGetReply?> GameRecordScheduleGet(GameRecordScheduleGetRequest request, CancellationToken cancellationToken = default)
        {
            return await _client.GameRecordScheduleGetAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask<GameRecordScheduleGetReply?> GameRecordScheduleUpdate(GameRecordScheduleUpdateRequest request, CancellationToken cancellationToken = default)
        {
            return await _client.GameRecordScheduleUpdateAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
