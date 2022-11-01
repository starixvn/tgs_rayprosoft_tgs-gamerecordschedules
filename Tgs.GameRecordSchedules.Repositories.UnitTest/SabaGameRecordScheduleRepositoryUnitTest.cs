using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using NSubstitute;
using Tgs.GameRecordSchedules.Repositories.Models;
using Tgs.SerialNumberGenerator;

namespace Tgs.GameRecordSchedules.Repositories.UnitTest;

[TestClass]
public class SabaGameRecordScheduleRepositoryUnitTest
{
	[TestMethod]
	public async Task Merge()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
		var fakerSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakerSystemClock = Substitute.For<ISystemClock>();

		string operatorId = "tg";
		long versionKey = 1000000;
		var dateTime = DateTimeOffset.UtcNow.DateTime;
		_ = fakerSystemClock.UtcNow.Returns(dateTime);

		var id = Guid.NewGuid();
		_ = fakerSerialNumberGenerator.GenerateAsync().Returns(id);

		var repository = new SabaGameRecordScheduleRepository(
			fakerMongoDatabase,
			fakerSerialNumberGenerator,
			fakerSystemClock);

		await repository
			.MergeAsync(
				operatorId,
				versionKey)
			.ConfigureAwait(false);

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

		_ = fakerMongoDatabase.Received(1)
			  .GetCollection<SabaGameRecordSchedule>(Arg.Is($"saba_game_record_schedules"))
			   .UpdateOneAsync(
					filter: Arg.Is(filter),
					update: Arg.Is(update),
					options: Arg.Is(options))
			 .ConfigureAwait(false);
	}

	[TestMethod]
	public async Task GetAsync()
	{
		var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
		var fakerSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakerSystemClock = Substitute.For<ISystemClock>();

		string operatorId = "tg";

		var repository = new SabaGameRecordScheduleRepository(
			fakerMongoDatabase,
			fakerSerialNumberGenerator,
			fakerSystemClock);

		_ = await repository.GetAsync(operatorId).ConfigureAwait(false);

		var filter = Builders<SabaGameRecordSchedule>.Filter.Eq(x => x.OperatorId, operatorId);

		_ = fakerMongoDatabase.Received(1)
			.GetCollection<SabaGameRecordSchedule>(Arg.Is($"saba_game_record_schedules"))
			.FindAsync<SabaGameRecordSchedule>(
				filter: Arg.Is(filter))
			.ConfigureAwait(false);
	}
}
