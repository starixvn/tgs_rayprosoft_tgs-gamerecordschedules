using MediatR;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices;

internal class GameRecordScheduleUpdateService(IGameRecordScheduleRepository gameRecordSchedule) : IRequestHandler<GameRecordScheduleUpdateRequest, Unit>
{
    private readonly IGameRecordScheduleRepository _gameRecordSchedule = gameRecordSchedule ?? throw new ArgumentNullException(nameof(gameRecordSchedule));

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
