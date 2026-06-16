using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

internal static class TranslationConstraintVector
{
    public static bool TryGetAxis(TranslationConstraint constraint, out Vector3d axis)
    {
        axis = constraint switch
        {
            TranslationConstraint.X => Vector3d.UnitX,
            TranslationConstraint.Y => Vector3d.UnitY,
            TranslationConstraint.Z => Vector3d.UnitZ,
            _ => Vector3d.Zero
        };
        return !axis.IsZero;
    }
}
