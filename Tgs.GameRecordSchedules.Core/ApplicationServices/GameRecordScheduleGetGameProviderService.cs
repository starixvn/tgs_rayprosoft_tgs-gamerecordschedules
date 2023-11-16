using MediatR;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices;

internal class GameRecordScheduleGetGameProviderService : IRequestHandler<GameRecordScheduleGetGameProviderRequest, GameRecordScheduleResponse?>
{
    private readonly IGameRecordScheduleRepository _gameRecordSchedule;

    public GameRecordScheduleGetGameProviderService(IGameRecordScheduleRepository gameRecordSchedule)
    {
        _gameRecordSchedule = gameRecordSchedule ?? throw new ArgumentNullException(nameof(gameRecordSchedule));
    }

    public async Task<GameRecordScheduleResponse?> Handle(GameRecordScheduleGetGameProviderRequest request, CancellationToken cancellationToken)
    {
        var gameRecordScheduleGet = await _gameRecordSchedule.GetAsync(
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
