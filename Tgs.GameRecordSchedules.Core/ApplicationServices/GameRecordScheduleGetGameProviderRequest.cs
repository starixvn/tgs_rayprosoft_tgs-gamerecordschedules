using MediatR;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices;

public record GameRecordScheduleGetGameProviderRequest : IRequest<GameRecordScheduleResponse?>
{
	public string GameProviderCode { get; init; } = default!;
	public string? BrandCode { get; init; } = default!;
	public string? GameProviderAgentId { get; init; }
	public string? CurrencyCode { get; init; }
}
