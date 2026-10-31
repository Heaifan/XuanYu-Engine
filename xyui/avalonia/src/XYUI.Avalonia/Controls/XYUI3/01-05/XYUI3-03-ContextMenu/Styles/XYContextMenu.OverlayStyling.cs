using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYContextMenu
{
    bool _overlayStylesApplied;

    internal void ApplyOverlayStyling()
    {
        XyuiOverlayResourceBridge.Attach(this);
        if (!_overlayStylesApplied)
        {
            Styles.Add(XyuiComponentStyles.Create());
            _overlayStylesApplied = true;
        }
        ApplyStyling();
        Menu.ApplyOverlayStyling();
    }
}
