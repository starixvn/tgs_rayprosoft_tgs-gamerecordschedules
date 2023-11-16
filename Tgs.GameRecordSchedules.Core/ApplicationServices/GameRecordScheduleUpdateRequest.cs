using MediatR;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices;

public record GameRecordScheduleUpdateRequest : IRequest<Unit>
{
    public Guid GameRecordScheduleId { get; set; }

    public string Config { get; set; } = default!;
}
