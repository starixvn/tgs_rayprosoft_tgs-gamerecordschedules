using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecordSchedules.GrpcServices;

public class GameRecordSchedulesGrpcService : GameRecordSchedulesService.GameRecordSchedulesServiceBase
{
    private readonly IMediator _mediator;

    public GameRecordSchedulesGrpcService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public override async Task<GameRecordScheduleAddReply> GameRecordScheduleAdd(GameRecordScheduleAddRequest request, ServerCallContext context)
    {
        var gameRecordScheduleId = await _mediator.Send(new GameRecordSchedules.Core.ApplicationServices.GameRecordScheduleAddRequest()
        {
            GameProviderId = request.GameProviderId,
            GameProviderCode = request.GameProviderCode,
            GameProviderAgentId = request.GameProviderAgentId,
            BrandCode = request.BrandCode,
            CurrencyCode = request.CurrencyCode,
            Config = request.Config
        }).ConfigureAwait(false);

        return new GameRecordScheduleAddReply()
        {
            GameRecordScheduleId = gameRecordScheduleId,
        };
    }

    public override async Task<GameRecordScheduleGetReply> GameRecordScheduleGet(GameRecordScheduleGetRequest request, ServerCallContext context)
    {
        var response = await _mediator.Send(new GameRecordSchedules.Core.ApplicationServices.GameRecordScheduleGetGameProviderRequest()
        {
            GameProviderCode = request.GameProviderCode.ToUpper(),
            BrandCode = request.BrandCode,
            GameProviderAgentId = request.GameProviderAgentId,
            CurrencyCode = request.CurrencyCode
        }).ConfigureAwait(false);

        return new GameRecordScheduleGetReply()
        {
            Item = response == null ? null :
            new GameRecordScheduleReply()
            {
                GameRecordScheduleId = response.GameRecordScheduleId,
                GameProviderId = response.GameProviderId,
                GameProviderCode = response.GameProviderCode,
                BrandCode = response.BrandCode,
                GameProviderAgentId = response.GameProviderAgentId,
                CurrencyCode = response.CurrencyCode,
                Status = (GameRecordScheduleStatus)response.Status,
                Config = response.Config,
                CreatedOn = response.CreatedOn.ToTimestamp(),
                UpdatedOn = response.UpdatedOn.ToTimestamp()
            }
        };
    }

    public override async Task<GameRecordScheduleUpdateReply> GameRecordScheduleUpdate(GameRecordScheduleUpdateRequest request, ServerCallContext context)
    {
        _ = await _mediator.Send(new GameRecordSchedules.Core.ApplicationServices.GameRecordScheduleUpdateRequest()
        {
            GameRecordScheduleId = request.GameRecordScheduleId,
            Config = request.Config
        }).ConfigureAwait(false);

        return new GameRecordScheduleUpdateReply()
        {
            Success = true,
        };
    }
}
