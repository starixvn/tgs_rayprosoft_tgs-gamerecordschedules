using MongoDB.Driver;
using Tgs.GameRecordSchedules.Core;
using Tgs.GameRecordSchedules.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordScheduleRepositories(
       this IServiceCollection services,
       Func<IServiceProvider, IMongoDatabase> mongoDatabaseResolver)
        => services
        .AddSerialNumberGenerator()
        .AddSingleton<ISystemClock, SystemClock>()
        .AddTransient<IGameRecordScheduleRepository>(sp => ActivatorUtilities.CreateInstance<GameRecordScheduleRepository>(sp, mongoDatabaseResolver(sp)))
        .AddTransient<ISabaGameRecordScheduleRepository>(sp => ActivatorUtilities.CreateInstance<SabaGameRecordScheduleRepository>(sp, mongoDatabaseResolver(sp)));
}
