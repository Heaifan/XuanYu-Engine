using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Gizmo;

public sealed partial class MoveGizmoLayout
{
    static MoveGizmoPlane Plane(
        ViewProjectionState state, Vector3d origin, MoveGizmoAxis axis,
        Vector3d a, Vector3d b, double worldAxisLength)
    {
        var dipToWorld = worldAxisLength / MoveScreenSize.TargetScreenAxisDip;
        var offset = MoveScreenSize.PlaneOffsetDip * dipToWorld;
        var size = MoveScreenSize.PlaneArmLengthDip * dipToWorld;
        var padding = MoveScreenSize.PlaneHitPaddingDip * dipToWorld;
        var hitOffset = System.Math.Max(0, offset - padding);
        var hitSize = size + (2 * padding);
        return new MoveGizmoPlane(axis,
            Corner(state, origin, a, b, offset, size, 0, 0),
            Corner(state, origin, a, b, offset, size, 1, 0),
            Corner(state, origin, a, b, offset, size, 1, 1),
            Corner(state, origin, a, b, offset, size, 0, 1),
            Corner(state, origin, a, b, hitOffset, hitSize, 0, 0),
            Corner(state, origin, a, b, hitOffset, hitSize, 1, 0),
            Corner(state, origin, a, b, hitOffset, hitSize, 1, 1),
            Corner(state, origin, a, b, hitOffset, hitSize, 0, 1));
    }

    static ScreenPoint Corner(
        ViewProjectionState state, Vector3d origin, Vector3d a, Vector3d b,
        double offset, double size, int alongA, int alongB) =>
        state.ProjectWorldPoint(origin + (a * (offset + (size * alongA)))
            + (b * (offset + (size * alongB))));
}
