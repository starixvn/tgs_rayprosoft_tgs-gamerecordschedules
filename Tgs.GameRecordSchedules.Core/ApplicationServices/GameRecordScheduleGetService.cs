using MediatR;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices;

internal class GameRecordScheduleGetService : IRequestHandler<GameRecordScheduleGetRequest, GameRecordScheduleResponse?>
{
	private readonly IGameRecordSchedule _gameRecordSchedule;

	public GameRecordScheduleGetService(IGameRecordSchedule gameRecordSchedule)
	{
		_gameRecordSchedule = gameRecordSchedule ?? throw new ArgumentNullException(nameof(gameRecordSchedule));
	}

	public async Task<GameRecordScheduleResponse?> Handle(GameRecordScheduleGetRequest request, CancellationToken cancellationToken)
	{
		var gameRecordScheduleGet = await _gameRecordSchedule.GetAsync(
			request.GameRecordScheduleId,
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
