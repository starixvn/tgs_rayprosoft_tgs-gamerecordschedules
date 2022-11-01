using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tgs.GameRecordSchedules.Repositories.Models;

public record SabaGameRecordSchedule
{
	[BsonElement("operator_id")]
	public string OperatorId { get; init; } = default!;

	[BsonElement("version_key")]
	public long VersionKey { get; set; } = default!;

	[BsonElement("updated_on")]
	public DateTime UpdatedOn { get; set; }
}
