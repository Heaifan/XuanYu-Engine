using System.Diagnostics;

internal static class XycCliSessionRunner
{
    public static async Task<int> RunAsync(string appPath, string sessionId, int timeout,
        string? demPath, bool diagnosticWrite, bool silent, bool uiHost = false,
        string? evidenceDirectory = null)
    {
        var start = XycCliStartConfiguration.Create(appPath, sessionId, demPath,
            diagnosticWrite, silent, uiHost, evidenceDirectory);
        SilentProcessMonitor? monitor = null;
        if (silent)
        {
            try
            {
                monitor = SilentProcessMonitor.StartBeforeChild();
                Console.Error.WriteLine("XYC silent monitor phase=armed-before-start");
            }
            catch (Exception error)
            {
                Console.Error.WriteLine($"XYC silent monitor unavailable: {error.Message}");
                return 125;
            }
        }

        using var monitorLifetime = monitor;
        using var process = Process.Start(start);
        if (process is null) return 3;
        var exited = process.WaitForExitAsync();
        Console.Error.WriteLine($"XYC sessionId={sessionId}; childPid={process.Id}; silent={silent}; phase=started");
        if (monitor is not null)
        {
            try { monitor.Attach(process); }
            catch (Exception error)
            {
                Console.Error.WriteLine($"XYC silent monitor attach failed: {error.Message}");
                return await XycCliProcessBridge.KillChildAsync(process, exited, 125);
            }
        }
        var result = await XycCliSessionLifetime.RunAsync(process, exited, monitor, timeout);
        monitor?.Dispose();
        Console.Error.WriteLine($"XYC silent monitor phase=stopped; childPid={process.Id}");
        return result;
    }
}
