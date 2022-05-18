using System.Text.Json;
using StackExchange.Redis;
using Tgs.GameRecordSchedules.Core;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Repositories
{
    internal class GameRecordScheduleRedisRepository : IGameRecordSchedule
    {
        private const string KeyGameRecordSchedule = "gamerecordschedule:gamerecordschedule:{0}";
        private const string KeyGameRecordScheduleId = "gamerecordschedule:gamerecordscheduleid:{0}:{1}:{2}:{3}";
        private readonly IDatabase _database;
        private readonly TimeSpan _expiry;
        private readonly IGameRecordSchedule _gameRecordSchedule;

        public GameRecordScheduleRedisRepository(
            IDatabase database,
            IGameRecordSchedule gameRecordSchedule,
            TimeSpan expiry)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _gameRecordSchedule = gameRecordSchedule ?? throw new ArgumentNullException(nameof(gameRecordSchedule));
            _expiry = expiry;
        }

        public async ValueTask<Guid> AddAsync(GameRecordScheduleAdd data, CancellationToken cancellationToken = default)
        {
            var gameRecordScheduleId = await _gameRecordSchedule.AddAsync(data, cancellationToken).ConfigureAwait(false);

            var gameRecordSchedule = await GetAsync(
                gameRecordScheduleId,
                cancellationToken).ConfigureAwait(false);

            if (gameRecordSchedule == null)
            {
                return gameRecordScheduleId;
            }

            _ = await GetGameRecordScheduleIdAsync(
                gameRecordSchedule.GameProviderCode,
                gameRecordSchedule.BrandCode,
                gameRecordSchedule.GameProviderAgentId,
                gameRecordSchedule.CurrencyCode,
                cancellationToken).ConfigureAwait(false);

            return gameRecordScheduleId;
        }

        public async ValueTask<GameRecordScheduleGet?> GetAsync(Guid gameRecordScheduleId, CancellationToken cancellationToken = default)
        {
            var key = string.Format(KeyGameRecordSchedule, gameRecordScheduleId);

            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var gameRecordScheduleGet = await _gameRecordSchedule.GetAsync(gameRecordScheduleId, cancellationToken).ConfigureAwait(false);

                if (gameRecordScheduleGet == null)
                {
                    return null;
                }
                _ = await CacheDataAsync(key, JsonSerializer.Serialize(gameRecordScheduleGet)).ConfigureAwait(false);

                return gameRecordScheduleGet;
            }

            return JsonSerializer.Deserialize<GameRecordScheduleGet>(await _database.StringGetAsync(key).ConfigureAwait(false));
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
            var key = string.Format(KeyGameRecordScheduleId, gameProviderCode, !string.IsNullOrEmpty(brandCode) ? brandCode : "brandcode", !string.IsNullOrEmpty(gameProviderAgentId) ? gameProviderAgentId : "gameprovideragentid", !string.IsNullOrEmpty(currencyCode) ? currencyCode : "currencycode");

            if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
            {
                var gameRecordScheduleId = await _gameRecordSchedule.GetGameRecordScheduleIdAsync(gameProviderCode, brandCode, gameProviderAgentId, currencyCode, cancellationToken).ConfigureAwait(false);

                if (gameRecordScheduleId == null || !gameRecordScheduleId.HasValue)
                {
                    return null;
                }
                _ = await CacheDataAsync(key, gameRecordScheduleId.Value.ToString()).ConfigureAwait(false);

                return gameRecordScheduleId;
            }

            return Guid.Parse(await _database.StringGetAsync(key).ConfigureAwait(false));
        }

        public async ValueTask UpdateAsync(GameRecordScheduleUpdate data, CancellationToken cancellationToken = default)
        {
            await _gameRecordSchedule.UpdateAsync(data, cancellationToken).ConfigureAwait(false);

            var gameRecordScheduleGet = await _gameRecordSchedule.GetAsync(data.GameRecordScheduleId, cancellationToken).ConfigureAwait(false);

            if (gameRecordScheduleGet is not null)
            {
                var key = string.Format(KeyGameRecordSchedule, data.GameRecordScheduleId);
                _ = await CacheDataAsync(key, JsonSerializer.Serialize(gameRecordScheduleGet)).ConfigureAwait(false);
            }
        }

        private Task<bool> CacheDataAsync(string redisKey, string redisValue)
                                            => _database.StringSetAsync(redisKey, redisValue, expiry: _expiry, when: When.Always);
    }
}
