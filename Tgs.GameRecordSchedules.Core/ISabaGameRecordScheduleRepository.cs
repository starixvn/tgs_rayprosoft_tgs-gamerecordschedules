namespace Tgs.GameRecordSchedules.Core;

public interface ISabaGameRecordScheduleRepository
{
	ValueTask<long> GetAsync(
		string operatorId,
		CancellationToken cancellationToken = default);

	ValueTask InitDBAsync(
		CancellationToken cancellationToken = default);

	ValueTask MergeAsync(
			string operatorId,
		long versionKey,
		CancellationToken cancellationToken = default);
}
