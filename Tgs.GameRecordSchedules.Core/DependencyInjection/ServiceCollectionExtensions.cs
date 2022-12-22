using System.Reflection;
using MediatR;
using Microsoft.Extensions.Internal;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordScheduleCore(
       this IServiceCollection services)
        => services
        .AddMediatR(Assembly.GetExecutingAssembly())
        .AddSingleton<ISystemClock, SystemClock>();
}
