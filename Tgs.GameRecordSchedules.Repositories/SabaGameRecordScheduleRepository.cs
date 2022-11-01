using Microsoft.Extensions.Internal;
using MongoDB.Bson;
using MongoDB.Driver;

using Tgs.GameRecordSchedules.Core;
using Tgs.GameRecordSchedules.Repositories.Models;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRecordSchedules.Repositories;

internal class SabaGameRecordScheduleRepository : ISabaGameRecordScheduleRepository
{
	private readonly IMongoDatabase _database;
	private readonly ISerialNumberGenerator _serialNumberGenerator;
	private readonly ISystemClock _systemClock;

	public SabaGameRecordScheduleRepository(
		IMongoDatabase database,
		ISerialNumberGenerator serialNumberGenerator,
		ISystemClock systemClock)
	{
		_database = database ?? throw new ArgumentNullException(nameof(database));
		_serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
		_systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
	}

	public async ValueTask<long> GetAsync(string operatorId, CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync().ConfigureAwait(false);
		var builder = Builders<SabaGameRecordSchedule>.Filter;
		var filter = builder.Eq(x => x.OperatorId, operatorId);

		var result = (await collection
			.FindAsync<SabaGameRecordSchedule>(
				filter: filter,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false))
			.FirstOrDefault(
				cancellationToken: cancellationToken);

		return result is null
			? 0
			: result.VersionKey;
	}

	public async ValueTask MergeAsync(string operatorId, long versionKey, CancellationToken cancellationToken = default)
	{
		var collection = await GetCollectionAsync().ConfigureAwait(false);
		var id = await _serialNumberGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);
		var dateTime = _systemClock.UtcNow.DateTime;
		var filter = Builders<SabaGameRecordSchedule>.Filter.Eq(x => x.OperatorId, operatorId);
		var update = Builders<SabaGameRecordSchedule>.Update
			.Set(s => s.Id, id)
			.Set(s => s.OperatorId, operatorId)
			.Set(s => s.VersionKey, versionKey)
			.Set(s => s.UpdatedOn, dateTime);
		var options = new UpdateOptions
		{
			IsUpsert = true
		};

		_ = await collection
			 .UpdateOneAsync(
				filter: filter,
				update: update,
				options: options,
				cancellationToken: cancellationToken)
			 .ConfigureAwait(false);
	}

	private static Task OperatorIdIndexAsync(IMongoCollection<SabaGameRecordSchedule> collection)
	{
		var index = Builders<SabaGameRecordSchedule>.IndexKeys
		   .Ascending(m => m.OperatorId);

		var createIndexOptions = new CreateIndexOptions
		{
			Name = $"ix_search"
		};
		var createIndexModel = new CreateIndexModel<SabaGameRecordSchedule>(index, createIndexOptions);

		return collection.Indexes.CreateOneAsync(createIndexModel);
	}

	private async ValueTask<bool> CollectionExistsAsync(string collectionName)
		=> await (await _database.ListCollectionsAsync(new ListCollectionsOptions { Filter = new BsonDocument("name", collectionName) }).ConfigureAwait(false)).AnyAsync().ConfigureAwait(false);

	private async Task<IMongoCollection<SabaGameRecordSchedule>> GetCollectionAsync()
	{
		var collection = _database.GetCollection<SabaGameRecordSchedule>($"saba_game_record_schedules".ToLower());
		if (collection.CollectionNamespace == null)
		{
			return collection;
		}

		if (!await CollectionExistsAsync(collection.CollectionNamespace.CollectionName.ToString()).ConfigureAwait(false))
		{
			await OperatorIdIndexAsync(collection).ConfigureAwait(false);
		}

		return collection;
	}
}
