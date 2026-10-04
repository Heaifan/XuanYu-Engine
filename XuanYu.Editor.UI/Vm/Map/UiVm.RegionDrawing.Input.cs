using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool TryPickGroundPoint(double x, double y, ViewportState viewport, out GroundPickResult result)
    {
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        var terrain = TerrainWorld is null ? null : new TerrainWorldGroundSurface(TerrainWorld);
        return MapSurfacePicker.TryPickGround(MapSession.CurrentMap, projection, x, y, terrain, out result);
    }

    bool TryPickMapPoint(double x, double y, ViewportState viewport, out MapPoint point)
    {
        if (!TryPickGroundPoint(x, y, viewport, out var result))
        {
            point = default;
            return false;
        }
        point = result.WorldXY;
        return true;
    }

    static bool IsInsideViewport(double x, double y, ViewportState viewport) =>
        x >= viewport.LogicalX && y >= viewport.LogicalY &&
        x <= viewport.LogicalX + viewport.LogicalWidth &&
        y <= viewport.LogicalY + viewport.LogicalHeight;
}
