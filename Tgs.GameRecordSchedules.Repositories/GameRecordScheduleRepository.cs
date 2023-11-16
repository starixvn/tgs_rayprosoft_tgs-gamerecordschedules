using MongoDB.Driver;

using Tgs.GameRecordSchedules.Core;
using Tgs.GameRecordSchedules.Core.Models;
using Tgs.GameRecordSchedules.Repositories.Models;
using Tgs.SerialNumberGenerator;
using ISystemClock = Tgs.GameRecordSchedules.Core.ISystemClock;

namespace Tgs.GameRecordSchedules.Repositories;

internal class GameRecordScheduleRepository : IGameRecordScheduleRepository
{
    private readonly IMongoDatabase _database;
    private readonly ISerialNumberGenerator _serialNumberGenerator;
    private readonly ISystemClock _systemClock;

    public GameRecordScheduleRepository(
        IMongoDatabase database,
        ISerialNumberGenerator serialNumberGenerator,
        ISystemClock systemClock)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async ValueTask<Guid> AddAsync(
        GameRecordScheduleAdd data,
        CancellationToken cancellationToken = default)
    {
        var collection = GetCollection();
        var gameRecordScheduleId = await _serialNumberGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);
        var dateTime = _systemClock.CurrentTime.DateTime;
        var document = new GameRecordSchedule()
        {
            GameRecordScheduleId = gameRecordScheduleId,
            GameProviderId = data.GameProviderId,
            GameProviderCode = data.GameProviderCode.ToUpper(),
            BrandCode = data.BrandCode,
            GameProviderAgentId = data.GameProviderAgentId,
            CurrencyCode = data.CurrencyCode,
            Status = (int)GameRecordScheduleStatus.Enable,
            Config = data.Config,
            CreatedOn = dateTime,
            UpdatedOn = dateTime
        };
        await collection.InsertOneAsync(document, cancellationToken: cancellationToken).ConfigureAwait(false);

        return gameRecordScheduleId;
    }

    public async ValueTask<GameRecordScheduleGet?> GetAsync(
        Guid gameRecordScheduleId,
        CancellationToken cancellationToken = default)
    {
        var collection = GetCollection();
        var builder = Builders<GameRecordSchedule>.Filter;
        var filter = builder.Eq(x => x.GameRecordScheduleId, gameRecordScheduleId);

        var result = (await collection
            .FindAsync<GameRecordSchedule>(filter, cancellationToken: cancellationToken)
            .ConfigureAwait(false)).FirstOrDefault(cancellationToken: cancellationToken);

        return result != null ?
            new GameRecordScheduleGet()
            {
                GameRecordScheduleId = result.GameRecordScheduleId,
                GameProviderId = result.GameProviderId,
                GameProviderCode = result.GameProviderCode,
                BrandCode = result.BrandCode,
                GameProviderAgentId = result.GameProviderAgentId,
                CurrencyCode = result.CurrencyCode,
                Status = (GameRecordScheduleStatus)result.Status,
                Config = result.Config,
                CreatedOn = result.CreatedOn,
                UpdatedOn = result.UpdatedOn
            } : null;
    }

    public async ValueTask<GameRecordScheduleGet?> GetAsync(
        string gameProviderCode,
        string? brandCode,
        string? gameProviderAgentId,
        string? currencyCode,
        CancellationToken cancellationToken = default)
    {
        var gameRecordScheduleId = await GetGameRecordScheduleIdAsync(
            gameProviderCode,
            brandCode,
            gameProviderAgentId,
            currencyCode,
            cancellationToken
            ).ConfigureAwait(false);

        return gameRecordScheduleId.HasValue ? await GetAsync(gameRecordScheduleId.Value, cancellationToken).ConfigureAwait(false) : null;
    }

    public async ValueTask<Guid?> GetGameRecordScheduleIdAsync(
        string gameProviderCode,
        string? brandCode,
        string? gameProviderAgentId,
        string? currencyCode,
        CancellationToken cancellationToken = default)
    {
        var collection = GetCollection();
        var builder = Builders<GameRecordSchedule>.Filter;
        var filter = builder.Eq(x => x.GameProviderCode, gameProviderCode) &
            builder.Eq(x => x.BrandCode, brandCode) &
            builder.Eq(x => x.GameProviderAgentId, gameProviderAgentId) &
            builder.Eq(x => x.CurrencyCode, currencyCode);

        return (await collection
            .FindAsync<GameRecordSchedule>(filter, cancellationToken: cancellationToken)
            .ConfigureAwait(false)).FirstOrDefault(cancellationToken: cancellationToken)?.GameRecordScheduleId;
    }

    public async ValueTask InitDBAsync(
        CancellationToken cancellationToken = default)
    {
        var collection = GetCollection();

        _ = await GameProviderCodeIndexAsync(
            collection: collection,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = await UpdatedOn_IndexAsync(
            collection: collection,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask UpdateAsync(
        GameRecordScheduleUpdate data,
        CancellationToken cancellationToken = default)
    {
        var collection = GetCollection();
        var dateTime = _systemClock.CurrentTime.DateTime;
        var builder = Builders<GameRecordSchedule>.Filter;
        var filter = builder.Eq(x => x.GameRecordScheduleId, data.GameRecordScheduleId);
        var update = Builders<GameRecordSchedule>.Update
            .Set(s => s.Config, data.Config)
            .Set(s => s.UpdatedOn, dateTime);
        _ = await collection
             .UpdateOneAsync(filter, update, cancellationToken: cancellationToken)
             .ConfigureAwait(false);
    }

    private static Task<string> GameProviderCodeIndexAsync(
        IMongoCollection<GameRecordSchedule> collection,
        CancellationToken cancellationToken)
    {
        var index = Builders<GameRecordSchedule>.IndexKeys
           .Ascending(m => m.GameProviderCode)
           .Ascending(m => m.BrandCode)
           .Ascending(m => m.GameProviderAgentId)
           .Ascending(m => m.CurrencyCode);

        var createIndexOptions = new CreateIndexOptions
        {
            Name = $"ix_search"
        };
        var createIndexModel = new CreateIndexModel<GameRecordSchedule>(index, createIndexOptions);

        return collection.Indexes.CreateOneAsync(
            model: createIndexModel,
            cancellationToken: cancellationToken);
    }

    private static Task<string> UpdatedOn_IndexAsync(
        IMongoCollection<GameRecordSchedule> collection,
        CancellationToken cancellationToken = default)
    {
        var index = Builders<GameRecordSchedule>.IndexKeys
            .Ascending(m => m.UpdatedOn);

        var createIndexOptions = new CreateIndexOptions
        {
            ExpireAfter = TimeSpan.FromDays(3),
            Name = $"ix_updated_on"
        };
        var createIndexModel = new CreateIndexModel<GameRecordSchedule>(index, createIndexOptions);

        return collection.Indexes.CreateOneAsync(
            model: createIndexModel,
            cancellationToken: cancellationToken);
    }

    private IMongoCollection<GameRecordSchedule> GetCollection()
            => _database.GetCollection<GameRecordSchedule>($"game_record_schedules");
}
