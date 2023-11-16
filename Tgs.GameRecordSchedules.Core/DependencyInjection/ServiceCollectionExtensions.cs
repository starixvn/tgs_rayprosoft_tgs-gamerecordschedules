using Microsoft.Extensions.Internal;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordScheduleCore(
       this IServiceCollection services)
        => services
        .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()))
        .AddSingleton<ISystemClock, SystemClock>();
}
