using Microsoft.Extensions.Options;
using Tgs.GameRecordSchedules.Client;
using Tgs.GameRecordSchedules.Protos;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameRecordSchedulesClient(
            this IServiceCollection services,
            Action<GameRecordSchedulesClientOptions, IServiceProvider> configure)
            => services
                .AddOptions<GameRecordSchedulesClientOptions>()
                .Configure(configure)
                .Services
                .AddGrpcClient<GameRecordSchedulesService.GameRecordSchedulesServiceClient>((sp, configureClient) =>
                {
                    var options = sp.GetRequiredService<IOptions<GameRecordSchedulesClientOptions>>().Value;
                    configureClient.Address = options.BaseUrl;
                })
                .ConfigureChannel((sp, configureChannel) =>
                {
                    var handler = new SocketsHttpHandler
                    {
                        PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                        KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                        EnableMultipleHttp2Connections = true
                    };
                    configureChannel.HttpHandler = handler;
                })
                .AddInterceptor<LoggingInterceptor>()
                .Services
                .AddSingleton<DiagnosticsInterceptorOptions>()
                .AddSingleton<LoggingInterceptor>()
                .AddTransient<IGameRecordSchedulesGrpc, GameRecordSchedulesGrpc>();
    }
}
