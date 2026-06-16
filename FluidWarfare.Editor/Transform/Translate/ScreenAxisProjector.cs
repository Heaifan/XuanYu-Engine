using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

internal static class ScreenAxisProjector
{
    public static bool TryCreate(
        Vector3d origin,
        Vector3d axis,
        TranslationCameraSnapshot camera,
        out ScreenAxisProjection projection)
    {
        projection = default;
        var sample = Math.Max(1.0, origin.DistanceTo(camera.Position) * 0.1);
        if (!TranslationMatrix.TryProject(camera.ViewProjection, origin,
                camera.ViewportWidth, camera.ViewportHeight, out var x0, out var y0) ||
            !TranslationMatrix.TryProject(camera.ViewProjection, origin + axis * sample,
                camera.ViewportWidth, camera.ViewportHeight, out var x1, out var y1))
            return false;

        var dx = x1 - x0;
        var dy = y1 - y0;
        var len = Math.Sqrt(dx * dx + dy * dy);
        if (len < 2.0)
            return false;

        projection = new ScreenAxisProjection(dx / len, dy / len, len / sample);
        return projection.IsValid;
    }
}
