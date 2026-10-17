using static SilentNativeMethods;

internal sealed class SilentWinEventPump : IDisposable
{
    private readonly Action<SilentWindowEvent> _event;
    private readonly WinEventCallback _callback;
    private readonly ManualResetEventSlim _ready = new();
    private readonly Thread _thread;
    private Exception? _startupError;
    private uint _threadId;
    private IntPtr _showHook;
    private IntPtr _foregroundHook;
    private bool _disposed;

    public SilentWinEventPump(Action<SilentWindowEvent> callback)
    {
        _event = callback;
        _callback = OnEvent;
        _thread = new Thread(MessageLoop) { IsBackground = true, Name = "XYC WinEvent pump" };
        _thread.Start();
        _ready.Wait(TimeSpan.FromSeconds(5));
        if (!_ready.IsSet || _startupError is not null)
            throw new InvalidOperationException("WinEvent hook startup failed.", _startupError);
    }

    private void MessageLoop()
    {
        _threadId = GetCurrentThreadId();
        try
        {
            _ = PeekMessage(out _, IntPtr.Zero, 0, 0, 0);
            _showHook = SetWinEventHook(EventObjectShow, EventObjectShow, IntPtr.Zero,
                _callback, 0, 0, WineventOutOfContext);
            _foregroundHook = SetWinEventHook(EventSystemForeground, EventSystemForeground,
                IntPtr.Zero, _callback, 0, 0, WineventOutOfContext);
            if (_showHook == IntPtr.Zero || _foregroundHook == IntPtr.Zero)
                throw new InvalidOperationException("SetWinEventHook returned NULL.");
            _ready.Set();
            while (true)
            {
                var status = GetMessage(out var message, IntPtr.Zero, 0, 0);
                if (status == 0) break;
                if (status < 0) { ReportFailure(); break; }
                _ = TranslateMessage(ref message);
                _ = DispatchMessage(ref message);
            }
        }
        catch (Exception error) { _startupError = error; _ready.Set(); ReportFailure(); }
        finally { Unhook(); }
    }

    private void OnEvent(IntPtr hook, uint type, IntPtr hwnd, int objectId, int childId,
        uint eventThread, uint eventTime)
    {
        var foreground = type == EventSystemForeground;
        if (hwnd == IntPtr.Zero || (!foreground && (objectId != ObjectIdWindow || childId != ChildIdSelf))) return;
        try
        {
            if (GetWindowThreadProcessId(hwnd, out var pid) == 0 || pid == 0) return;
            var show = type == EventObjectShow;
            var top = foreground || GetAncestor(hwnd, GaRoot) == hwnd;
            _event(new((int)pid, show, top, show || IsWindowVisible(hwnd), hwnd, foreground));
        }
        catch { ReportFailure(); }
    }

    private void ReportFailure() => _event(new(0, false, false, false, IntPtr.Zero, false, true));
    private void Unhook()
    {
        if (_showHook != IntPtr.Zero) _ = UnhookWinEvent(_showHook);
        if (_foregroundHook != IntPtr.Zero) _ = UnhookWinEvent(_foregroundHook);
        _showHook = _foregroundHook = IntPtr.Zero;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_threadId != 0) _ = PostThreadMessage(_threadId, WmQuit, IntPtr.Zero, IntPtr.Zero);
        _thread.Join(TimeSpan.FromSeconds(2));
        if (!_thread.IsAlive) _ready.Dispose();
    }
}
