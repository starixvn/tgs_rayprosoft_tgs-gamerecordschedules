using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecordSchedules.Client;

internal class GameRecordSchedulesClient(GameRecordSchedulesService.GameRecordSchedulesServiceClient client) : IGameRecordSchedulesClient
{
    private readonly GameRecordSchedulesService.GameRecordSchedulesServiceClient _client = client ?? throw new ArgumentNullException(nameof(client));

    public async ValueTask<GameRecordScheduleAddReply> GameRecordScheduleAddAsync(GameRecordScheduleAddRequest request, CancellationToken cancellationToken = default)
        => await _client.GameRecordScheduleAddAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);

    public async ValueTask<GameRecordScheduleReply?> GameRecordScheduleGetAsync(GameRecordScheduleGetRequest request, CancellationToken cancellationToken = default)
    {
        var reply = await _client.GameRecordScheduleGetAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        return reply.Item;
    }

    public async ValueTask<GameRecordScheduleUpdateReply> GameRecordScheduleUpdateAsync(GameRecordScheduleUpdateRequest request, CancellationToken cancellationToken = default)
    {
        return await _client.GameRecordScheduleUpdateAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
