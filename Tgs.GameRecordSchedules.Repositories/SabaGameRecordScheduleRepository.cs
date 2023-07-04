using MongoDB.Driver;

using Tgs.GameRecordSchedules.Core;
using Tgs.GameRecordSchedules.Repositories.Models;

namespace Tgs.GameRecordSchedules.Repositories;

internal class SabaGameRecordScheduleRepository : ISabaGameRecordScheduleRepository
{
    private readonly IMongoDatabase _database;
    private readonly ISystemClock _systemClock;

    public SabaGameRecordScheduleRepository(
        IMongoDatabase database,
        ISystemClock systemClock)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async ValueTask<long> GetAsync(
        string operatorId,
        CancellationToken cancellationToken = default)
    {
        var collection = GetCollection();
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

    public async ValueTask InitDBAsync(CancellationToken cancellationToken = default)
    {
        var collection = GetCollection();

        await OperatorIdIndexAsync(
            collection: collection,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await UpdatedOn_IndexAsync(
            collection: collection,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask MergeAsync(
            string operatorId,
        long versionKey,
        CancellationToken cancellationToken = default)
    {
        var collection = GetCollection();
        var dateTime = _systemClock.CurrentTime.DateTime;
        var filter = Builders<SabaGameRecordSchedule>.Filter.Eq(x => x.OperatorId, operatorId);
        var update = Builders<SabaGameRecordSchedule>.Update
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

    private static Task OperatorIdIndexAsync(
        IMongoCollection<SabaGameRecordSchedule> collection,
        CancellationToken cancellationToken = default)
    {
        var index = Builders<SabaGameRecordSchedule>.IndexKeys
           .Ascending(m => m.OperatorId);

        var createIndexOptions = new CreateIndexOptions
        {
            Name = $"ix_search"
        };
        var createIndexModel = new CreateIndexModel<SabaGameRecordSchedule>(index, createIndexOptions);

        return collection.Indexes
            .CreateOneAsync(
            model: createIndexModel,
            cancellationToken: cancellationToken);
    }

    private static Task UpdatedOn_IndexAsync(
        IMongoCollection<SabaGameRecordSchedule> collection,
        CancellationToken cancellationToken = default)
    {
        var index = Builders<SabaGameRecordSchedule>.IndexKeys
            .Ascending(m => m.UpdatedOn);

        var createIndexOptions = new CreateIndexOptions
        {
            ExpireAfter = TimeSpan.FromDays(3),
            Name = $"ix_updated_on"
        };
        var createIndexModel = new CreateIndexModel<SabaGameRecordSchedule>(index, createIndexOptions);

        return collection.Indexes.CreateOneAsync(
            model: createIndexModel,
            cancellationToken: cancellationToken);
    }

    private IMongoCollection<SabaGameRecordSchedule> GetCollection()
            => _database.GetCollection<SabaGameRecordSchedule>($"saba_game_record_schedules".ToLower());
}
