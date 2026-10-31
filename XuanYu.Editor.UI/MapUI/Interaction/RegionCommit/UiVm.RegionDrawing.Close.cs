using XuanYu.Core.Gizmo;
using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool IsRegionClosePointer(double x, double y, ViewportState viewport)
    {
        if (_regionDrawing.Draft is not { CanClose: true } draft) return false;
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        if (!projection.TryProjectWorldPoint(new(
                draft.Vertices[0].X, draft.Vertices[0].Y,
                MapSession.CurrentMap.Surface.BaseHeightMeters), out var first)) return false;
        var distance = Math.Sqrt(Math.Pow(first.X - x, 2) + Math.Pow(first.Y - y, 2));
        if (distance > ScaleGizmoScreenSize.CenterHitRadiusDip) return false;
        _regionDrawing.UpdatePointer(draft.Vertices[0], closeCandidate: true);
        RaiseRegionDrawingBindings();
        return true;
    }
}
