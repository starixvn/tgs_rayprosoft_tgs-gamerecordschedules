using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecordSchedules.Client;

internal class SabaGameRecordSchedulesClient(
    SabaGameRecordSchedulesService.SabaGameRecordSchedulesServiceClient client) : ISabaGameRecordSchedulesClient
{
    public async ValueTask<long> GetAsync(string operatorId, CancellationToken cancellationToken = default)
    {
        var request = new GetRequest
        {
            OperatorId = operatorId
        };
        var reply = await client
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
        _ = await client
                .MergeAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }
}
