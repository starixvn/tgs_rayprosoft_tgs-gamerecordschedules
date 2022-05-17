using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using StackExchange.Redis;
using Tgs.GameRecordSchedules.Core;
using Tgs.GameRecordSchedules.Core.Models;

namespace Tgs.GameRecordSchedules.Repositories.UnitTest
{
    [TestClass]
    public class GameRecordScheduleRedisRepositoryUnitTest
    {
        [TestMethod]
        public async Task Add()
        {
            var fakerDatabase = Substitute.For<IDatabase>();
            var fakerGameRecordSchedule = Substitute.For<IGameRecordSchedule>();
            var fakerEexpiry = TimeSpan.FromSeconds(1);

            var data = new GameRecordScheduleAdd()
            {
                GameProviderId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                GameProviderCode = "KY",
                BrandCode = "Default",
                GameProviderAgentId = "72412",
                CurrencyCode = "CNY",
                Config = JsonSerializer.Serialize(new { })
            };

            var gameRecordScheduleId = Guid.NewGuid();

            var repository = new GameRecordScheduleRedisRepository(
                fakerDatabase,
                fakerGameRecordSchedule,
                fakerEexpiry);

            var gameRecordSchedule = new GameRecordScheduleGet()
            {
                GameRecordScheduleId = gameRecordScheduleId,
                GameProviderId = data.GameProviderId,
                GameProviderCode = data.GameProviderCode.ToUpper(),
                BrandCode = data.BrandCode,
                GameProviderAgentId = data.GameProviderAgentId,
                CurrencyCode = data.CurrencyCode,
                Status = GameRecordScheduleStatus.Enable,
                Config = data.Config,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };

            _ = fakerGameRecordSchedule
                .AddAsync(Arg.Any<GameRecordScheduleAdd>()).Returns(gameRecordScheduleId);

            _ = fakerGameRecordSchedule.GetAsync(gameRecordScheduleId).Returns(ValueTask.FromResult<GameRecordScheduleGet?>(gameRecordSchedule));

            _ = await repository.AddAsync(data).ConfigureAwait(false);

            _ = await fakerGameRecordSchedule.Received(1)
                .GetGameRecordScheduleIdAsync(
                gameRecordSchedule.GameProviderCode,
                gameRecordSchedule.BrandCode,
                gameRecordSchedule.GameProviderAgentId,
                gameRecordSchedule.CurrencyCode
                ).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetAsync()
        {
            var fakerDatabase = Substitute.For<IDatabase>();
            var fakerGameRecordSchedule = Substitute.For<IGameRecordSchedule>();
            var fakerEexpiry = TimeSpan.FromSeconds(1);

            var gameRecordScheduleId = Guid.NewGuid();

            var gameRecordSchedule = new GameRecordScheduleGet()
            {
                GameRecordScheduleId = gameRecordScheduleId,
                GameProviderId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                GameProviderCode = "KY",
                BrandCode = "Default",
                GameProviderAgentId = "72412",
                CurrencyCode = "CNY",
                Status = GameRecordScheduleStatus.Enable,
                Config = JsonSerializer.Serialize(new { }),
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };

            _ = fakerGameRecordSchedule
                .GetAsync(Arg.Is(gameRecordScheduleId)).Returns(gameRecordSchedule);

            var repository = new GameRecordScheduleRedisRepository(
                fakerDatabase,
                fakerGameRecordSchedule,
                fakerEexpiry);

            var actual = await repository.GetAsync(gameRecordScheduleId).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(gameRecordSchedule.GameRecordScheduleId, actual.GameRecordScheduleId);
        }

        [TestMethod]
        public async Task GetGameRecordScheduleIdAsync()
        {
            var fakerDatabase = Substitute.For<IDatabase>();
            var fakerGameRecordSchedule = Substitute.For<IGameRecordSchedule>();
            var fakerEexpiry = TimeSpan.FromSeconds(1);

            var gameProviderCode = "KY";
            var brandCode = "Default";
            var gameProviderAgentId = "72412";
            var currencyCode = "CNY";
            var gameRecordScheduleId = Guid.NewGuid();

            _ = fakerGameRecordSchedule
                .GetGameRecordScheduleIdAsync(
                Arg.Is(gameProviderCode),
                Arg.Is(brandCode),
                Arg.Is(gameProviderAgentId),
                Arg.Is(currencyCode)
                ).Returns(gameRecordScheduleId);

            var repository = new GameRecordScheduleRedisRepository(
                fakerDatabase,
                fakerGameRecordSchedule,
                fakerEexpiry);

            var actual = await repository.GetGameRecordScheduleIdAsync(
                gameProviderCode,
                brandCode,
                gameProviderAgentId,
                currencyCode).ConfigureAwait(false);

            Assert.IsNotNull(actual);
            Assert.AreEqual(gameRecordScheduleId, actual);
        }

        [TestMethod]
        public async Task UpdateAsync()
        {
            var fakerDatabase = Substitute.For<IDatabase>();
            var fakerGameRecordSchedule = Substitute.For<IGameRecordSchedule>();
            var fakerEexpiry = TimeSpan.FromSeconds(1);

            var gameRecordScheduleId = Guid.NewGuid();
            var data = new GameRecordScheduleUpdate()
            {
                GameRecordScheduleId = gameRecordScheduleId,
                Config = JsonSerializer.Serialize(new { })
            };

            var gameRecordSchedule = new GameRecordScheduleGet()
            {
                GameRecordScheduleId = gameRecordScheduleId,
                GameProviderId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                GameProviderCode = "KY",
                BrandCode = "Default",
                GameProviderAgentId = "72412",
                CurrencyCode = "CNY",
                Status = GameRecordScheduleStatus.Enable,
                Config = data.Config,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };

            _ = fakerGameRecordSchedule.GetAsync(Arg.Is(data.GameRecordScheduleId)).Returns(gameRecordSchedule);

            var repository = new GameRecordScheduleRedisRepository(
                fakerDatabase,
                fakerGameRecordSchedule,
                fakerEexpiry);

            await repository.UpdateAsync(data).ConfigureAwait(false);

            _ = fakerDatabase.Received(1)
                .StringSetAsync(
                Arg.Any<RedisKey>()
                , Arg.Any<RedisValue>()
                , expiry: fakerEexpiry,
                when: When.Always);
        }
    }
}
