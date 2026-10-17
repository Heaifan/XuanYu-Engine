using static SilentNativeMethods;

internal sealed class SilentTargetWindowProbe : IDisposable
{
    private readonly Action<SilentProcessViolation, int, IntPtr> _fail;
    private Timer? _foregroundTimer;
    private int _targetPid;

    public SilentTargetWindowProbe(Action<SilentProcessViolation, int, IntPtr> fail)
        => _fail = fail;

    public void Attach(int processId)
    {
        _targetPid = processId;
        EnumWindows(InspectWindow, IntPtr.Zero);
        PollForeground(null);
        _foregroundTimer = new Timer(PollForeground, null,
            TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(20));
    }

    private bool InspectWindow(IntPtr hwnd, IntPtr state)
    {
        if (GetWindowThreadProcessId(hwnd, out var pid) != 0 && pid == (uint)_targetPid)
        {
            var visible = IsWindowVisible(hwnd);
            Console.Error.WriteLine($"XYC silent window snapshot: pid={pid}; hwnd=0x{hwnd.ToInt64():X}; visible={visible}");
            if (visible) _fail(SilentProcessViolation.VisibleWindow, (int)pid, hwnd);
        }
        return true;
    }

    private void PollForeground(object? state)
    {
        try
        {
            var hwnd = GetForegroundWindow();
            if (hwnd != IntPtr.Zero && GetWindowThreadProcessId(hwnd, out var pid) != 0 &&
                pid == (uint)_targetPid)
                _fail(SilentProcessViolation.ForegroundFocus, (int)pid, hwnd);
        }
        catch { _fail(SilentProcessViolation.MonitorFailure, _targetPid, IntPtr.Zero); }
    }

    public void Dispose() => _foregroundTimer?.Dispose();
}
