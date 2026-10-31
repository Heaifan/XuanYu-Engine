using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenu
{
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (TopLevel.GetTopLevel(this) is not { } root) return;
        if (root.GetType().Name == "PopupRoot") XyuiOverlayResourceBridge.AttachPopupRoot(root); else XyuiOverlayResourceBridge.Attach(root);
        if (_overlayStylesApplied) Dispatcher.UIThread.Post(RefreshOverlayResources, DispatcherPriority.Loaded);
    }

    void RefreshOverlayResources()
    {
        if (TopLevel.GetTopLevel(this) is not { } root) return;
        XyuiOverlayResourceBridge.Attach(root);
        ApplyStyling();
        foreach (var item in Items) item.ApplyStyling();
    }
}
