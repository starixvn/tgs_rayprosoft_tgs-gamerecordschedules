namespace Tgs.GameRecordSchedules
{
    public record MongoDBOptions
    {
        public string ConnectionString { get; set; } = "mongodb://localhost";
    }
}
