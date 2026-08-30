namespace XYUI.Avalonia.Controls;

public sealed partial class XYTab
{
    void Select() { IsSelected = true; Selected?.Invoke(this, EventArgs.Empty); }
    void RequestClose() => CloseRequested?.Invoke(this, EventArgs.Empty);
}
