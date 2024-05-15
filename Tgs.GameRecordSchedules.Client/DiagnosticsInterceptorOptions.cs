using System.Diagnostics;

namespace Tgs.GameRecordSchedules.Client;

public class DiagnosticsInterceptorOptions
{
	public ActivitySource InterceptorActivitySource { get; set; } =
		new ActivitySource($"Tgs.GameRecordSchedules.Client.DiagnosticsInterceptor");
}
