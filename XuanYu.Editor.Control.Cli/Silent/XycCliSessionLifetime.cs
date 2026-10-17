using System.Diagnostics;

internal static class XycCliSessionLifetime
{
    public static async Task<int> RunAsync(Process process, Task exited,
        SilentProcessMonitor? monitor, int timeoutSeconds)
    {
        var output = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
        var closed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var pump = XycCliProcessBridge.PumpOutputAsync(process, output, closed, monitor is not null);
        var input = Task.Run(() => XycCliProcessBridge.PumpInputAsync(Console.In, process.StandardInput));
        var deadline = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
        var trigger = monitor is null
            ? await Task.WhenAny(input, closed.Task, exited, deadline)
            : await Task.WhenAny(input, closed.Task, exited, deadline, monitor.Violation);
        var violation = await FindViolationAsync(process, exited, monitor);
        if (violation is not null) return violation.Value;
        if (trigger == deadline) return await XycCliProcessBridge.KillChildAsync(process, exited);

        process.StandardInput.Close();
        var pumpResult = await WaitStageAsync(pump, deadline, process, exited, monitor);
        if (pumpResult is not null) return pumpResult.Value;
        var exitResult = await WaitStageAsync(exited, deadline, process, exited, monitor);
        if (exitResult is not null) return exitResult.Value;
        Console.Error.WriteLine($"XYC silent monitor phase=exited; childPid={process.Id}; exitCode={process.ExitCode}");
        return process.ExitCode;
    }

    private static async Task<int?> FindViolationAsync(Process process, Task exited,
        SilentProcessMonitor? monitor)
    {
        if (monitor?.Violation.IsCompletedSuccessfully != true) return null;
        var reason = await monitor.Violation;
        Console.Error.WriteLine($"XYC silent monitor phase=terminating; childPid={process.Id}; reason={reason}");
        return await XycCliProcessBridge.KillChildAsync(process, exited, 125);
    }

    private static async Task<int?> WaitStageAsync(Task stage, Task deadline,
        Process process, Task exited, SilentProcessMonitor? monitor)
    {
        var violation = monitor?.Violation;
        var trigger = violation is null ? await Task.WhenAny(stage, deadline)
            : await Task.WhenAny(stage, deadline, violation);
        if (violation?.IsCompletedSuccessfully == true)
        {
            var reason = await violation;
            Console.Error.WriteLine($"XYC silent monitor phase=terminating; childPid={process.Id}; reason={reason}");
            return process.HasExited ? 125 : await XycCliProcessBridge.KillChildAsync(process, exited, 125);
        }
        if (trigger == deadline) return await XycCliProcessBridge.KillChildAsync(process, exited);
        await stage;
        if (violation?.IsCompletedSuccessfully == true)
        {
            var reason = await violation;
            Console.Error.WriteLine($"XYC silent monitor phase=violation-after-stage; childPid={process.Id}; reason={reason}");
            return process.HasExited ? 125 : await XycCliProcessBridge.KillChildAsync(process, exited, 125);
        }
        return null;
    }
}
