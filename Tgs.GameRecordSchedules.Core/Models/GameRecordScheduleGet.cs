namespace Tgs.GameRecordSchedules.Core.Models
{
    public record GameRecordScheduleGet
    {
        public Guid GameRecordScheduleId { get; set; }
        public Guid GameProviderId { get; init; }
        public string GameProviderCode { get; init; } = default!;
        public string BrandCode { get; init; } = default!;
        public string GameProviderAgentId { get; init; } = default!;
        public string CurrencyCode { get; set; } = default!;
        public GameRecordScheduleStatus Status { get; set; }
        public string Config { get; set; } = default!;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
