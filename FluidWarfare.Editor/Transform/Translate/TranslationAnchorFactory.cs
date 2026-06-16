using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

internal static class TranslationAnchorFactory
{
    public static bool TryCreate(
        Vector3d referencePosition,
        TranslationConstraint constraint,
        TranslationRay startRay,
        double pointerX,
        double pointerY,
        TranslationCameraSnapshot camera,
        out TranslationDragAnchor anchor)
    {
        anchor = null!;
        if (!camera.IsValid)
            return false;

        if (constraint == TranslationConstraint.GroundPlane)
            return TryCreateGround(referencePosition, startRay, pointerX, pointerY, out anchor);

        return TranslationConstraintVector.TryGetAxis(constraint, out var axis) &&
               TryCreateAxis(referencePosition, constraint, axis, startRay, pointerX, pointerY, camera, out anchor);
    }

    private static bool TryCreateGround(
        Vector3d referencePosition,
        TranslationRay startRay,
        double pointerX,
        double pointerY,
        out TranslationDragAnchor anchor)
    {
        anchor = null!;
        if (!TranslationRayPlane.TryIntersect(startRay, referencePosition, Vector3d.UnitZ, out var hit))
            return false;

        anchor = new TranslationDragAnchor
        {
            Constraint = TranslationConstraint.GroundPlane,
            InitialPosition = referencePosition,
            ReferencePosition = referencePosition,
            PointerX = pointerX,
            PointerY = pointerY,
            StartHit = hit,
            PlaneNormal = Vector3d.UnitZ,
            ScreenAxis = default,
            Mode = TranslationMappingMode.PlaneIntersection
        };
        return true;
    }

    private static bool TryCreateAxis(
        Vector3d referencePosition,
        TranslationConstraint constraint,
        Vector3d axis,
        TranslationRay startRay,
        double pointerX,
        double pointerY,
        TranslationCameraSnapshot camera,
        out TranslationDragAnchor anchor)
    {
        anchor = null!;
        var view = (camera.Position - referencePosition).Normalize();
        var normal = view - axis * view.Dot(axis);
        if (normal.Length > 1e-8 &&
            TranslationRayPlane.TryIntersect(startRay, referencePosition, normal.Normalize(), out var hit))
        {
            anchor = Build(constraint, referencePosition, pointerX, pointerY, hit, normal.Normalize(), default,
                TranslationMappingMode.PlaneIntersection);
            return true;
        }

        if (!ScreenAxisProjector.TryCreate(referencePosition, axis, camera, out var screenAxis))
            return false;

        anchor = Build(constraint, referencePosition, pointerX, pointerY, referencePosition, Vector3d.Zero, screenAxis,
            TranslationMappingMode.ScreenAxis);
        return true;
    }

    private static TranslationDragAnchor Build(
        TranslationConstraint constraint,
        Vector3d referencePosition,
        double pointerX,
        double pointerY,
        Vector3d startHit,
        Vector3d normal,
        ScreenAxisProjection screenAxis,
        TranslationMappingMode mode) => new()
        {
            Constraint = constraint,
            InitialPosition = referencePosition,
            ReferencePosition = referencePosition,
            PointerX = pointerX,
            PointerY = pointerY,
            StartHit = startHit,
            PlaneNormal = normal,
            ScreenAxis = screenAxis,
            Mode = mode
        };
}
