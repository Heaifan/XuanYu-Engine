using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translation.Plane;

/// <summary>
/// 平面平移求解器。XY/XZ/YZ/View 平面共用。
/// target = initialPosition + (currentHit - startHit)
/// </summary>
public static class PlaneTranslationSolver
{
    public static Vector3d Solve(
        PlaneTranslationAnchor anchor,
        Vector3d currentPlaneHit)
    {
        if (!anchor.IsValid || anchor.Mode == PlaneTranslationMode.Disabled)
            return anchor.InitialPosition;

        var delta = currentPlaneHit - anchor.StartHit;
        return anchor.InitialPosition + delta;
    }
}
