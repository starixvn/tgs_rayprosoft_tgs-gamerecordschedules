namespace Tgs.GameRecordSchedules
{
    public record RedisOptions
    {
        public string ConnectionString { get; set; } = "localhost";
    }
}
