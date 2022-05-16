using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecordSchedule.Services
{
    public class GameRecordSchedulesGrpcService : GameRecordSchedulesService.GameRecordSchedulesServiceBase
    {
        private readonly ILogger<GameRecordSchedulesGrpcService> _logger;
        private readonly IMediator _mediator;

        public GameRecordSchedulesGrpcService(
            ILogger<GameRecordSchedulesGrpcService> logger,
            IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override async Task<GameRecordScheduleGetReply?> GameRecordScheduleAdd(GameRecordScheduleAddRequest request, ServerCallContext context)
        {
            _logger.LogInformation("RecordTime:{DateTime.Now.ToString(\"o\")} | ActionName:{actionname} | Request:{request}",
                DateTime.Now.ToString("o"),
                "Add",
                request);

            var gameRecordScheduleId = await _mediator.Send(new GameRecordSchedules.Core.ApplicationServices.GameRecordScheduleAddRequest()
            {
                GameProviderId = request.GameProviderId,
                GameProviderCode = request.GameProviderCode,
                GameProviderAgentId = request.GameProviderAgentId,
                BrandCode = request.BrandCode,
                CurrencyCode = request.CurrencyCode,
                Config = request.Config
            }).ConfigureAwait(false);

            var response = await _mediator.Send(new GameRecordSchedules.Core.ApplicationServices.GameRecordScheduleGetRequest()
            {
                GameRecordScheduleId = gameRecordScheduleId
            }).ConfigureAwait(false);

            return response is not null ?
                new GameRecordScheduleGetReply()
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
                    UpdatedOn = response.UpdatedOn.ToTimestamp(),
                } : null;
        }

        public override async Task<GameRecordScheduleGetReply?> GameRecordScheduleGet(GameRecordScheduleGetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("RecordTime:{DateTime.Now.ToString(\"o\")} | ActionName:{actionname} | Request:{request}",
                DateTime.Now.ToString("o"),
                "Get",
                request);

            var response = await _mediator.Send(new GameRecordSchedules.Core.ApplicationServices.GameRecordScheduleGetGameProviderRequest()
            {
                GameProviderCode = request.GameProviderCode.ToUpper(),
                BrandCode = request.BrandCode,
                GameProviderAgentId = request.GameProviderAgentId,
                CurrencyCode = request.CurrencyCode
            }).ConfigureAwait(false);

            return response is not null ?
                new GameRecordScheduleGetReply()
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
                    UpdatedOn = response.UpdatedOn.ToTimestamp(),
                } : null;
        }

        public override async Task<GameRecordScheduleGetReply?> GameRecordScheduleUpdate(GameRecordScheduleUpdateRequest request, ServerCallContext context)
        {
            _logger.LogInformation("RecordTime:{DateTime.Now.ToString(\"o\")} | ActionName:{actionname} | Request:{request}",
                DateTime.Now.ToString("o"),
                "Update",
                request);

            _ = await _mediator.Send(new GameRecordSchedules.Core.ApplicationServices.GameRecordScheduleUpdateRequest()
            {
                GameRecordScheduleId = request.GameRecordScheduleId,
                Config = request.Config
            }).ConfigureAwait(false);

            var response = await _mediator.Send(new GameRecordSchedules.Core.ApplicationServices.GameRecordScheduleGetRequest()
            {
                GameRecordScheduleId = request.GameRecordScheduleId
            }).ConfigureAwait(false);

            return response is not null ?
                new GameRecordScheduleGetReply()
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
                    UpdatedOn = response.UpdatedOn.ToTimestamp(),
                } : null;
        }
    }
}
