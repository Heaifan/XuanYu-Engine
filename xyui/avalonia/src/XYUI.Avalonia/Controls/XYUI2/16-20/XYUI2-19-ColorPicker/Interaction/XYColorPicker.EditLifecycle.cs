namespace XYUI.Avalonia.Controls;

public partial class XYColorPicker
{
    bool _cancelNextClose;

    public event EventHandler? EditStarted;
    public event EventHandler? EditCommitted;
    public event EventHandler? EditCanceled;

    void BeginEditLifecycle()
    {
        _cancelNextClose = false;
        EditStarted?.Invoke(this, EventArgs.Empty);
    }

    internal void CommitEditLifecycle()
    {
        if (!IsOpen) return;
        _cancelNextClose = false;
        IsOpen = false;
    }

    internal void CancelEditLifecycle()
    {
        if (!IsOpen) return;
        _cancelNextClose = true;
        IsOpen = false;
    }

    void CompleteEditLifecycle()
    {
        var canceled = _cancelNextClose;
        _cancelNextClose = false;
        if (canceled) EditCanceled?.Invoke(this, EventArgs.Empty);
        else EditCommitted?.Invoke(this, EventArgs.Empty);
    }
}
