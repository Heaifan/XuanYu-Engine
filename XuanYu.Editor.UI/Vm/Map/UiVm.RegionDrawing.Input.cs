using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool TryPickRegionPoint(double x, double y, ViewportState viewport, out MapPoint point)
    {
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        return MapSurfacePicker.TryPick(MapSession.CurrentMap, projection, x, y, out point);
    }

    static bool IsInsideViewport(double x, double y, ViewportState viewport) =>
        x >= viewport.LogicalX && y >= viewport.LogicalY &&
        x <= viewport.LogicalX + viewport.LogicalWidth &&
        y <= viewport.LogicalY + viewport.LogicalHeight;
}
