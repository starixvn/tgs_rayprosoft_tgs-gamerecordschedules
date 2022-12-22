using System.Diagnostics;

namespace Tgs.GameRecordSchedules;

public class DiagnosticsInterceptorOptions
{
    public ActivitySource InterceptorActivitySource { get; set; } =
        new ActivitySource($"Tgs.GameRecordSchedules.DiagnosticsInterceptor");
}
