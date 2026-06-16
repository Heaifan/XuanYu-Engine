using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

internal static class TranslationRayPlane
{
    public static bool TryIntersect(
        TranslationRay ray,
        Vector3d planePoint,
        Vector3d planeNormal,
        out Vector3d hit)
    {
        hit = default;
        var denom = ray.Direction.Dot(planeNormal);
        if (Math.Abs(denom) < 1e-10)
            return false;

        var t = (planePoint - ray.Origin).Dot(planeNormal) / denom;
        if (t <= 0 || !double.IsFinite(t))
            return false;

        hit = ray.At(t);
        return double.IsFinite(hit.X) && double.IsFinite(hit.Y) && double.IsFinite(hit.Z);
    }
}
