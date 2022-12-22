using Tgs.GameRecordSchedules;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordSchedules(
        this IServiceCollection services,
        Action<MongoDBOptions, IServiceProvider> mongoDBconfigure)
        => services
            .AddOptions<MongoDBOptions>()
            .Configure(mongoDBconfigure)
            .Services
            .AddSingleton<MongoDBConnectionManager>();
}
