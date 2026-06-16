using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

internal static class TranslationSolver
{
    public static bool TrySolve(
        TranslationDragAnchor anchor,
        TranslationRay currentRay,
        double pointerX,
        double pointerY,
        out Vector3d position)
    {
        position = anchor.ReferencePosition;
        if (anchor.Constraint == TranslationConstraint.GroundPlane)
            return TrySolveGround(anchor, currentRay, out position);

        if (!TranslationConstraintVector.TryGetAxis(anchor.Constraint, out var axis))
            return false;

        var distance = anchor.Mode == TranslationMappingMode.ScreenAxis
            ? anchor.ScreenAxis.DistanceFrom(anchor.PointerX, anchor.PointerY, pointerX, pointerY)
            : AxisDistance(anchor, currentRay, axis);
        if (!double.IsFinite(distance))
            return false;

        position = anchor.ReferencePosition + axis * distance;
        return double.IsFinite(position.X) && double.IsFinite(position.Y) && double.IsFinite(position.Z);
    }

    private static bool TrySolveGround(
        TranslationDragAnchor anchor,
        TranslationRay currentRay,
        out Vector3d position)
    {
        position = anchor.ReferencePosition;
        if (!TranslationRayPlane.TryIntersect(currentRay, anchor.ReferencePosition, Vector3d.UnitZ, out var hit))
            return false;

        var delta = hit - anchor.StartHit;
        position = anchor.ReferencePosition + new Vector3d(delta.X, delta.Y, 0);
        return true;
    }

    private static double AxisDistance(TranslationDragAnchor anchor, TranslationRay currentRay, Vector3d axis)
    {
        if (!TranslationRayPlane.TryIntersect(currentRay, anchor.ReferencePosition, anchor.PlaneNormal, out var hit))
            return double.NaN;

        return (hit - anchor.StartHit).Dot(axis);
    }
}
