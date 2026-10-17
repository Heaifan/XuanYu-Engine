internal sealed class SilentWindowEventWatcher : IDisposable
{
    private const int MaxJournal = 4096;
    private readonly object _gate = new();
    private readonly Queue<SilentWindowEvent> _early = new();
    private readonly Action<SilentWindowEvent> _targetEvent;
    private readonly SilentWinEventPump _pump;
    private int _targetPid;
    private bool _overflowed;

    public SilentWindowEventWatcher(Action<SilentWindowEvent> targetEvent)
    {
        _targetEvent = targetEvent;
        _pump = new SilentWinEventPump(OnEvent);
    }

    public int TargetPid => Volatile.Read(ref _targetPid);

    public void Attach(int processId)
    {
        List<SilentWindowEvent> pending = [];
        lock (_gate)
        {
            _targetPid = processId;
            while (_early.TryDequeue(out var item)) pending.Add(item);
        }
        if (_overflowed)
            _targetEvent(new(0, false, false, false, IntPtr.Zero, false, true));
        foreach (var item in pending)
            if (item.MonitorFailure || item.ProcessId == processId) _targetEvent(item);
    }

    private void OnEvent(SilentWindowEvent item)
    {
        lock (_gate)
        {
            if (_targetPid == 0)
            {
                if (_early.Count >= MaxJournal) _overflowed = true;
                else _early.Enqueue(item);
                return;
            }
            if (!item.MonitorFailure && item.ProcessId != _targetPid) return;
        }
        _targetEvent(item);
    }

    public void Dispose() => _pump.Dispose();
}

internal readonly record struct SilentWindowEvent(int ProcessId, bool IsShow,
    bool IsTopLevel, bool IsVisible, IntPtr Hwnd, bool IsForeground, bool MonitorFailure = false);
