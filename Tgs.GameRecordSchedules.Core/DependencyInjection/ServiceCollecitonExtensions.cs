using System.Reflection;
using MediatR;
using Tgs.GameRecordSchedules.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddGameRecordScheduleCore(
       this IServiceCollection services)
        => services
        .AddMediatR(Assembly.GetExecutingAssembly())
        .AddScoped<ISystemClock, SystemClock>();
}
