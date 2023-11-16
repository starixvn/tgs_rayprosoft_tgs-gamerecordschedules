using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecordSchedules.Client;

internal class SabaGameRecordSchedulesClient(SabaGameRecordSchedulesService.SabaGameRecordSchedulesServiceClient client) : ISabaGameRecordSchedulesClient
{
    private readonly SabaGameRecordSchedulesService.SabaGameRecordSchedulesServiceClient _client = client ?? throw new ArgumentNullException(nameof(client));

    public async ValueTask<long> GetAsync(string operatorId, CancellationToken cancellationToken = default)
    {
        var request = new GetRequest
        {
            OperatorId = operatorId
        };
        var reply = await _client
                .GetAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

        return reply.VersionKey;
    }

    public async ValueTask MergeAsync(string operatorId, long versionKey, CancellationToken cancellationToken = default)
    {
        var request = new MergeRequest
        {
            OperatorId = operatorId,
            VersionKey = versionKey
        };
        _ = await _client
                .MergeAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }
}
