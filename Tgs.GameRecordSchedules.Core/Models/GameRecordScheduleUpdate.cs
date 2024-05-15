namespace Tgs.GameRecordSchedules.Core.Models;

public record GameRecordScheduleUpdate
{
	public Guid GameRecordScheduleId { get; set; }

	public string Config { get; set; } = default!;
}
