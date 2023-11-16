using Grpc.Core;
using Tgs.GameRecordSchedules.Core;
using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecordSchedules.GrpcServices;

public class SabaGameRecordSchedulesGrpcService(ISabaGameRecordScheduleRepository repository) : SabaGameRecordSchedulesService.SabaGameRecordSchedulesServiceBase
{
    private readonly ISabaGameRecordScheduleRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public override async Task<GetReply> Get(GetRequest request, ServerCallContext context)
    {
        var versionKey = await _repository
                .GetAsync(
                    operatorId: request.OperatorId,
                    cancellationToken: context.CancellationToken)
                .ConfigureAwait(false);

        return new GetReply
        {
            VersionKey = versionKey
        };
    }

    public override async Task<BaseReply> Merge(MergeRequest request, ServerCallContext context)
    {
        await _repository
            .MergeAsync(
                operatorId: request.OperatorId,
                versionKey: request.VersionKey,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            Success = true
        };
    }
}
