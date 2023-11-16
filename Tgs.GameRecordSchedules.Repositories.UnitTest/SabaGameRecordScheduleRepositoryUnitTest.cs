using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using NSubstitute;
using Tgs.GameRecordSchedules.Core;
using Tgs.GameRecordSchedules.Repositories.Models;

namespace Tgs.GameRecordSchedules.Repositories.UnitTest;

public class SabaGameRecordScheduleRepositoryUnitTest
{
    [Fact]
    public async Task GetAsync()
    {
        var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
        var fakerSystemClock = Substitute.For<ISystemClock>();

        string operatorId = "tg";

        var repository = new SabaGameRecordScheduleRepository(
            fakerMongoDatabase,
            fakerSystemClock);

        _ = await repository.GetAsync(operatorId);

        var filter = Builders<SabaGameRecordSchedule>.Filter.Eq(x => x.OperatorId, operatorId);

        _ = fakerMongoDatabase
            .Received(1)
            .GetCollection<SabaGameRecordSchedule>(Arg.Is($"saba_game_record_schedules"))
            .FindAsync<SabaGameRecordSchedule>(
            filter: Arg.Is(filter));
    }

    [Fact]
    public async Task Merge()
    {
        var fakerMongoDatabase = Substitute.For<IMongoDatabase>();
        var fakerSystemClock = Substitute.For<ISystemClock>();

        string operatorId = "tg";
        long versionKey = 1000000;
        var dateTime = DateTimeOffset.UtcNow.DateTime;
        _ = fakerSystemClock.CurrentTime.Returns(dateTime);

        var repository = new SabaGameRecordScheduleRepository(
            fakerMongoDatabase,
            fakerSystemClock);

        await repository.MergeAsync(
            operatorId: operatorId,
            versionKey: versionKey);

        var filter = Builders<SabaGameRecordSchedule>.Filter.Eq(x => x.OperatorId, operatorId);

        var update = Builders<SabaGameRecordSchedule>.Update
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
            options: Arg.Is(options));
    }
}
