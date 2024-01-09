using MediatR;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices;

internal class GameRecordScheduleGetGameProviderService(
    IGameRecordScheduleRepository gameRecordSchedule) : IRequestHandler<GameRecordScheduleGetGameProviderRequest, GameRecordScheduleResponse?>
{
    public async Task<GameRecordScheduleResponse?> Handle(GameRecordScheduleGetGameProviderRequest request, CancellationToken cancellationToken)
    {
        var gameRecordScheduleGet = await gameRecordSchedule.GetAsync(
            request.GameProviderCode,
            request.BrandCode,
            request.GameProviderAgentId,
            request.CurrencyCode,
            cancellationToken).ConfigureAwait(false);

        return gameRecordScheduleGet != null ?
            new GameRecordScheduleResponse()
            {
                GameRecordScheduleId = gameRecordScheduleGet.GameRecordScheduleId,
                GameProviderId = gameRecordScheduleGet.GameProviderId,
                GameProviderCode = gameRecordScheduleGet.GameProviderCode,
                BrandCode = gameRecordScheduleGet.BrandCode,
                GameProviderAgentId = gameRecordScheduleGet.GameProviderAgentId,
                CurrencyCode = gameRecordScheduleGet.CurrencyCode,
                Status = gameRecordScheduleGet.Status,
                Config = gameRecordScheduleGet.Config,
                CreatedOn = gameRecordScheduleGet.CreatedOn,
                UpdatedOn = gameRecordScheduleGet.UpdatedOn
            } : null;
    }
}
