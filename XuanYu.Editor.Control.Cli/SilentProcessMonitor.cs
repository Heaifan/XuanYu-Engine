using System.Diagnostics;

public enum SilentProcessViolation { None, VisibleWindow, ForegroundFocus, MonitorFailure }

/// <summary>Owns the focused PID monitor and fails closed on any silent-contract violation.</summary>
internal sealed class SilentProcessMonitor : IDisposable
{
    private readonly SilentWindowEventWatcher _events;
    private readonly SilentTargetWindowProbe _probe;
    private readonly TaskCompletionSource<SilentProcessViolation> _violation =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private bool _disposed;

    private SilentProcessMonitor()
    {
        _events = new SilentWindowEventWatcher(OnWindowEvent);
        _probe = new SilentTargetWindowProbe(Fail);
    }

    public Task<SilentProcessViolation> Violation => _violation.Task;
    public static SilentProcessMonitor StartBeforeChild() => new();

    public void Attach(Process process)
    {
        _events.Attach(process.Id);
        _probe.Attach(process.Id);
        Console.Error.WriteLine($"XYC silent monitor phase=reconciled; childPid={process.Id}");
    }

    private void OnWindowEvent(SilentWindowEvent observed)
    {
        if (observed.MonitorFailure)
        {
            Fail(SilentProcessViolation.MonitorFailure, _events.TargetPid, IntPtr.Zero);
            return;
        }
        var decision = SilentProcessDecision.Evaluate(_events.TargetPid, observed.ProcessId,
            observed.IsShow, observed.IsTopLevel, observed.IsVisible, observed.IsForeground);
        if (decision != SilentProcessViolation.None)
            Fail(decision, observed.ProcessId, observed.Hwnd);
    }

    private void Fail(SilentProcessViolation reason, int processId, IntPtr hwnd)
    {
        if (_violation.TrySetResult(reason))
            Console.Error.WriteLine($"XYC silent monitor FAIL: {reason}; pid={processId}; hwnd=0x{hwnd.ToInt64():X}");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _probe.Dispose();
        _events.Dispose();
    }
}

public static class SilentProcessDecision
{
    public static SilentProcessViolation Evaluate(int targetPid, int eventPid, bool isShow,
        bool isTopLevel, bool isVisible, bool isForegroundOwner)
    {
        if (targetPid <= 0 || (eventPid != targetPid && !isForegroundOwner))
            return SilentProcessViolation.None;
        if (isForegroundOwner) return SilentProcessViolation.ForegroundFocus;
        return isShow && isTopLevel && isVisible
            ? SilentProcessViolation.VisibleWindow : SilentProcessViolation.None;
    }
}
