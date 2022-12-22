using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tgs.GameRecordSchedules.Repositories.Models;

public record GameRecordSchedule
{
    [BsonId]
    public Guid GameRecordScheduleId { get; set; }

    [BsonElement("game_provider_id")]
    public Guid GameProviderId { get; init; }

    [BsonElement("game_provider_code")]
    public string GameProviderCode { get; init; } = default!;

    [BsonDefaultValue("")]
    [BsonElement("brand_code")]
    public string BrandCode { get; init; } = default!;

    [BsonDefaultValue("")]
    [BsonElement("game_provider_agent_id")]
    public string GameProviderAgentId { get; init; } = default!;

    [BsonDefaultValue("")]
    [BsonElement("currency_code")]
    public string CurrencyCode { get; set; } = default!;

    [BsonDefaultValue(0)]
    [BsonElement("status")]
    [BsonRepresentation(BsonType.Int32)]
    public int Status { get; set; }

    [BsonElement("config")]
    public string Config { get; set; } = default!;

    [BsonElement("created_on")]
    public DateTime CreatedOn { get; set; }

    [BsonElement("updated_on")]
    public DateTime UpdatedOn { get; set; }
}
