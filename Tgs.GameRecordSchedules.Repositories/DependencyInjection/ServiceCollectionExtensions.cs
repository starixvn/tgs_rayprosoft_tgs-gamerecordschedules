using MongoDB.Driver;
using StackExchange.Redis;
using Tgs.GameRecordSchedules.Core;
using Tgs.GameRecordSchedules.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordScheduleRepositories(
       this IServiceCollection services,
       Func<IServiceProvider, IDatabase> redisDatabaseResolver,
       Func<IServiceProvider, IMongoDatabase> mongoDatabaseResolver)
        => services
        .AddSerialNumberGenerator()
        .AddTransient<IGameRecordSchedule>(sp => ActivatorUtilities.CreateInstance<GameRecordScheduleRepository>(sp, mongoDatabaseResolver(sp)))
        .Decorate<IGameRecordSchedule>((innerRepository, sp) => ActivatorUtilities.CreateInstance<GameRecordScheduleRedisRepository>(sp, innerRepository, redisDatabaseResolver(sp), TimeSpan.FromHours(1)));
}
