using MediatR;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices;

internal class GameRecordScheduleUpdateService : IRequestHandler<GameRecordScheduleUpdateRequest>
{
	private readonly IGameRecordSchedule _gameRecordSchedule;

	public GameRecordScheduleUpdateService(IGameRecordSchedule gameRecordSchedule)
	{
		_gameRecordSchedule = gameRecordSchedule ?? throw new ArgumentNullException(nameof(gameRecordSchedule));
	}

	public async Task<Unit> Handle(GameRecordScheduleUpdateRequest request, CancellationToken cancellationToken)
	{
		var data = new GameRecordScheduleUpdate()
		{
			GameRecordScheduleId = request.GameRecordScheduleId,
			Config = request.Config
		};

		await _gameRecordSchedule.UpdateAsync(data, cancellationToken).ConfigureAwait(false);

		return Unit.Value;
	}
}
