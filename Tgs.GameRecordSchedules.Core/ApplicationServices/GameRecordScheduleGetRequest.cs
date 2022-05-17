using MediatR;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices
{
    public record GameRecordScheduleGetRequest : IRequest<GameRecordScheduleResponse?>
    {
        public Guid GameRecordScheduleId { get; set; }
    }
}
