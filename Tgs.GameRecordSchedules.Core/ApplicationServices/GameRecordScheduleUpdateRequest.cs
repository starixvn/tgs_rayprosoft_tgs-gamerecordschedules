using MediatR;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices;

public record GameRecordScheduleUpdateRequest : IRequest
{
	public Guid GameRecordScheduleId { get; set; }

	public string Config { get; set; } = default!;
}
