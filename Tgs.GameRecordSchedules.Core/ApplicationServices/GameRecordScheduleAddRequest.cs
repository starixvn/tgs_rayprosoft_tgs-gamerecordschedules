using MediatR;

namespace Tgs.GameRecordSchedules.Core.ApplicationServices;

public record GameRecordScheduleAddRequest : IRequest<Guid>
{
    public Guid GameProviderId { get; init; }
    public string GameProviderCode { get; init; } = default!;
    public string BrandCode { get; init; } = default!;
    public string GameProviderAgentId { get; init; } = default!;
    public string CurrencyCode { get; set; } = default!;
    public string Config { get; set; } = default!;
}
