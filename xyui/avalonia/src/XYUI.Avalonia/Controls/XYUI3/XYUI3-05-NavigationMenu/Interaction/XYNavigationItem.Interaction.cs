using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationItem
{
    bool _pointerHooked;
    public event EventHandler? Selected;
    void HookInteraction()
    {
        if (_pointerHooked) return;
        PointerPressed += (_, _) => { IsSelected = true; Selected?.Invoke(this, EventArgs.Empty); };
        _pointerHooked = true;
    }
}
