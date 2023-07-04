using Tgs.GameRecordSchedules.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceInitializationExtensions
{
    public static async Task<WebApplication> InitialGameRecordSchedule(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IGameRecordScheduleRepository>();
            await repository
                .InitDBAsync()
                .ConfigureAwait(false);
        }
        return app;
    }

    public static async Task<WebApplication> InitialSabaGameRecordSchedule(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<ISabaGameRecordScheduleRepository>();
            await repository
                .InitDBAsync()
                .ConfigureAwait(false);
        }
        return app;
    }
}
