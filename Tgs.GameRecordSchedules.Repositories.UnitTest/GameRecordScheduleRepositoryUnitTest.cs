using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using NSubstitute;
using Tgs.GameRecordSchedules.Core;
using Tgs.GameRecordSchedules.Core.Models;
using Tgs.GameRecordSchedules.Repositories.Models;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRecordSchedules.Repositories.UnitTest;

[TestClass]
public class GameRecordScheduleRepositoryUnitTest
{
	[TestMethod]
	public async Task Add()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
		var fakerSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakerSystemClock = Substitute.For<ISystemClock>();

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
		_ = fakerSerialNumberGenerator.GenerateAsync().Returns(gameRecordScheduleId);

		var dateTime = DateTimeOffset.UtcNow.DateTime;
		_ = fakerSystemClock.UtcNow.Returns(dateTime);

		var repository = new GameRecordScheduleRepository(
			fakerMongoDatabase,
			fakerSerialNumberGenerator,
			fakerSystemClock);

		_ = await repository.AddAsync(data).ConfigureAwait(false);

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

		_ = fakerMongoDatabase.Received(1)
			  .GetCollection<GameRecordSchedule>(Arg.Is($"game_record_schedules"))
			  .InsertOneAsync(Arg.Is(document));
	}

	[TestMethod]
	public async Task GetAsync()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
		var fakerSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakerSystemClock = Substitute.For<ISystemClock>();

		var gameRecordScheduleId = Guid.NewGuid();

		var repository = new GameRecordScheduleRepository(
			fakerMongoDatabase,
			fakerSerialNumberGenerator,
			fakerSystemClock);

		var builder = Builders<GameRecordSchedule>.Filter;
		var filter = builder.Eq(x => x.GameRecordScheduleId, gameRecordScheduleId);

		_ = await repository.GetAsync(gameRecordScheduleId).ConfigureAwait(false);

		_ = fakerMongoDatabase.Received(1)
			  .GetCollection<GameRecordSchedule>(Arg.Is($"game_record_schedules"))
			  .FindAsync(filter);
	}

	[TestMethod]
	public async Task GetGameRecordScheduleIdAsync()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
		var fakerSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakerSystemClock = Substitute.For<ISystemClock>();

		var gameProviderCode = "KY";
		var brandCode = "Default";
		var gameProviderAgentId = "72412";
		var currencyCode = "CNY";

		var builder = Builders<GameRecordSchedule>.Filter;
		var filter = builder.Eq(x => x.GameProviderCode, gameProviderCode) &
			builder.Eq(x => x.BrandCode, brandCode) &
			builder.Eq(x => x.GameProviderAgentId, gameProviderAgentId) &
			builder.Eq(x => x.CurrencyCode, currencyCode);

		var repository = new GameRecordScheduleRepository(
			fakerMongoDatabase,
			fakerSerialNumberGenerator,
			fakerSystemClock);

		_ = await repository.GetGameRecordScheduleIdAsync(gameProviderCode, brandCode, gameProviderAgentId, currencyCode).ConfigureAwait(false);

		_ = fakerMongoDatabase.Received(1)
			  .GetCollection<GameRecordSchedule>(Arg.Is($"game_record_schedules"))
			  .FindAsync(filter);
	}

	[TestMethod]
	public async Task UpdateAsync()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
		var fakerSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakerSystemClock = Substitute.For<ISystemClock>();

		var gameRecordScheduleId = Guid.NewGuid();

		var data = new GameRecordScheduleUpdate()
		{
			GameRecordScheduleId = gameRecordScheduleId,
			Config = JsonSerializer.Serialize(new { })
		};

		var builder = Builders<GameRecordSchedule>.Filter;
		var filter = builder.Eq(x => x.GameRecordScheduleId, gameRecordScheduleId);
		var update = Builders<GameRecordSchedule>.Update
			.Set(s => s.Config, data.Config)
			.Set(s => s.UpdatedOn, DateTime.UtcNow);

		var repository = new GameRecordScheduleRepository(
			fakerMongoDatabase,
			fakerSerialNumberGenerator,
			fakerSystemClock);

		await repository.UpdateAsync(data).ConfigureAwait(false);

		_ = fakerMongoDatabase.Received(1)
			  .GetCollection<GameRecordSchedule>(Arg.Is($"game_record_schedules"))
			  .UpdateOneAsync(filter, update);
	}
}
