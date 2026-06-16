using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

public static class TranslationRayBuilder
{
    public static bool TryBuild(double pixelX, double pixelY, TranslationCameraSnapshot camera, out TranslationRay ray)
    {
        ray = default;
        if (!camera.IsValid ||
            pixelX < 0 || pixelX >= camera.ViewportWidth ||
            pixelY < 0 || pixelY >= camera.ViewportHeight)
            return false;

        var ndcX = 2.0 * pixelX / camera.ViewportWidth - 1.0;
        var ndcY = 2.0 * pixelY / camera.ViewportHeight - 1.0;

        if (!TranslationMatrix.TryUnproject(camera.InverseViewProjection, ndcX, ndcY, 0, out var nearWorld) ||
            !TranslationMatrix.TryUnproject(camera.InverseViewProjection, ndcX, ndcY, 1, out var farWorld))
            return false;

        var direction = farWorld - nearWorld;
        if (direction.Length < 1e-12)
            return false;

        ray = new TranslationRay(nearWorld, direction.Normalize());
        return true;
    }
}
