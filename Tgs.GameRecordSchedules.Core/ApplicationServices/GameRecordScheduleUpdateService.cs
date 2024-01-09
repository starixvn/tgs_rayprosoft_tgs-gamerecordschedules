using MediatR;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices;

internal class GameRecordScheduleUpdateService(
    IGameRecordScheduleRepository gameRecordSchedule) : IRequestHandler<GameRecordScheduleUpdateRequest, Unit>
{
    public async Task<Unit> Handle(GameRecordScheduleUpdateRequest request, CancellationToken cancellationToken)
    {
        var data = new GameRecordScheduleUpdate()
        {
            GameRecordScheduleId = request.GameRecordScheduleId,
            Config = request.Config
        };

        await gameRecordSchedule.UpdateAsync(data, cancellationToken).ConfigureAwait(false);

        return Unit.Value;
    }
}
