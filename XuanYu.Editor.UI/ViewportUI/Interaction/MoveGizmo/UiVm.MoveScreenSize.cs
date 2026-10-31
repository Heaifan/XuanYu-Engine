using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    double ComputeMoveGizmoWorldAxisLength(Vector3d origin)
    {
        if (_lastViewport is not { } viewport) return MoveGizmoLayout.AxisLength;
        return MoveScreenSize.ComputeWorldAxisLength(_camera, viewport, origin);
    }
}
