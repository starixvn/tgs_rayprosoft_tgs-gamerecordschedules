namespace Tgs.GameRecordSchedules.Client;

public interface ISabaGameRecordSchedulesClient
{
	public ValueTask<long> GetAsync(string operatorId, CancellationToken cancellationToken = default);

	public ValueTask MergeAsync(string operatorId, long versionKey, CancellationToken cancellationToken = default);
}
