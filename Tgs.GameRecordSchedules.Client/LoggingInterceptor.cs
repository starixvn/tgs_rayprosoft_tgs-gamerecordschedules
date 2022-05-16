using System.Diagnostics;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;

namespace Tgs.GameRecordSchedules.Client
{
    internal class LoggingInterceptor : Interceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var stopwatch = Stopwatch.StartNew();

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _logger.LogInformation("Timestamp:{timestamp} LogOn:{logOn}  MethodType:{methodType} Method:{method} Request:{request}", timestamp, DateTime.Now.ToString("o"), context.Method.Type, context.Method.Name, request);

            var response = continuation(request, context);

            stopwatch.Stop();
            _logger.LogInformation("Timestamp:{timestamp} LogOn:{logOn}  MethodType:{methodType} Method:{method} Spend:{spend:0.0000}ms Response:{response}", timestamp, DateTime.Now.ToString("o"), context.Method.Type, context.Method.Name, stopwatch.Elapsed.TotalMilliseconds, response);
            return response;
        }
    }
}
