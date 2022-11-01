using MediatR;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices;

internal class GameRecordScheduleAddService : IRequestHandler<GameRecordScheduleAddRequest, Guid>
{
	private readonly IGameRecordSchedule _gameRecordSchedule;

	public GameRecordScheduleAddService(IGameRecordSchedule gameRecordSchedule)
	{
		_gameRecordSchedule = gameRecordSchedule ?? throw new ArgumentNullException(nameof(gameRecordSchedule));
	}

	public async Task<Guid> Handle(GameRecordScheduleAddRequest request, CancellationToken cancellationToken)
	{
		var gameRecordScheduleId = await _gameRecordSchedule.GetGameRecordScheduleIdAsync(request.GameProviderCode, request.BrandCode, request.GameProviderAgentId, request.CurrencyCode, cancellationToken: cancellationToken).ConfigureAwait(false);

		if (gameRecordScheduleId.HasValue)
		{
			return gameRecordScheduleId.Value;
		}

		var data = new GameRecordScheduleAdd()
		{
			GameProviderId = request.GameProviderId,
			GameProviderCode = request.GameProviderCode,
			BrandCode = request.BrandCode,
			GameProviderAgentId = request.GameProviderAgentId,
			CurrencyCode = request.CurrencyCode,
			Config = request.Config
		};

		return await _gameRecordSchedule.AddAsync(data, cancellationToken).ConfigureAwait(false);
	}
}
