using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool MarkerPlacementPointerPressed(double x, double y, ViewportState viewport)
    {
        if (!BeginMarkerDrawing()) return IsMarkerPlacementTool;
        return MarkerDrawingPointerReleased(x, y, viewport);
    }
}
