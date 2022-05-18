using MongoDB.Bson;
using MongoDB.Driver;

using Tgs.GameRecordSchedules.Core;
using Tgs.GameRecordSchedules.Core.Models;
using Tgs.GameRecordSchedules.Repositories.Models;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRecordSchedules.Repositories
{
    internal class GameRecordScheduleRepository : IGameRecordSchedule
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

        public async ValueTask<Guid> AddAsync(GameRecordScheduleAdd data, CancellationToken cancellationToken = default)
        {
            var collection = await GetCollectionAsync().ConfigureAwait(false);
            var gameRecordScheduleId = await _serialNumberGenerator.GenerateAsync().ConfigureAwait(false);
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

        public async ValueTask<GameRecordScheduleGet?> GetAsync(Guid gameRecordScheduleId, CancellationToken cancellationToken = default)
        {
            var collection = await GetCollectionAsync().ConfigureAwait(false);
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

        public async ValueTask<GameRecordScheduleGet?> GetAsync(string gameProviderCode, string? brandCode, string? gameProviderAgentId, string? currencyCode, CancellationToken cancellationToken = default)
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

        public async ValueTask<Guid?> GetGameRecordScheduleIdAsync(string gameProviderCode, string? brandCode, string? gameProviderAgentId, string? currencyCode, CancellationToken cancellationToken = default)
        {
            var collection = await GetCollectionAsync().ConfigureAwait(false);
            var builder = Builders<GameRecordSchedule>.Filter;
            var filter = builder.Eq(x => x.GameProviderCode, gameProviderCode) &
                builder.Eq(x => x.BrandCode, brandCode) &
                builder.Eq(x => x.GameProviderAgentId, gameProviderAgentId) &
                builder.Eq(x => x.CurrencyCode, currencyCode);

            return (await collection
                .FindAsync<GameRecordSchedule>(filter, cancellationToken: cancellationToken)
                .ConfigureAwait(false)).FirstOrDefault(cancellationToken: cancellationToken)?.GameRecordScheduleId;
        }

        public async ValueTask UpdateAsync(GameRecordScheduleUpdate data, CancellationToken cancellationToken = default)
        {
            var collection = await GetCollectionAsync().ConfigureAwait(false);
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

        private static Task GameProviderCodeIndexAsync(IMongoCollection<GameRecordSchedule> collection)
        {
            var index = Builders<GameRecordSchedule>.IndexKeys
               .Ascending(m => m.GameProviderCode)
               .Ascending(m => m.BrandCode)
               .Ascending(m => m.GameProviderAgentId)
               .Ascending(m => m.CurrencyCode);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"ix_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}_gameProvider_code_brand_code_game_provider_agent_id_currency_code"
            };
            var createIndexModel = new CreateIndexModel<GameRecordSchedule>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(createIndexModel);
        }

        private static Task PrimaryKeyIndexAsync(IMongoCollection<GameRecordSchedule> collection)
        {
            var index = Builders<GameRecordSchedule>.IndexKeys
               .Ascending(m => m.GameRecordScheduleId);

            var createIndexOptions = new CreateIndexOptions
            {
                Name = $"pk_{collection.CollectionNamespace.CollectionName.ToString().ToLower()}"
            };
            var createIndexModel = new CreateIndexModel<GameRecordSchedule>(index, createIndexOptions);

            return collection.Indexes.CreateOneAsync(createIndexModel);
        }

        private async ValueTask<bool> CollectionExistsAsync(string collectionName)
            => await (await _database.ListCollectionsAsync(new ListCollectionsOptions { Filter = new BsonDocument("name", collectionName) }).ConfigureAwait(false)).AnyAsync().ConfigureAwait(false);

        private async Task<IMongoCollection<GameRecordSchedule>> GetCollectionAsync()
        {
            var collection = _database.GetCollection<GameRecordSchedule>($"{nameof(GameRecordSchedule)}s");
            if (collection.CollectionNamespace == null)
            {
                return collection;
            }

            if (!await CollectionExistsAsync(collection.CollectionNamespace.CollectionName.ToString()).ConfigureAwait(false))
            {
                await PrimaryKeyIndexAsync(collection).ConfigureAwait(false);
                await GameProviderCodeIndexAsync(collection).ConfigureAwait(false);
            }

            return collection;
        }
    }
}
