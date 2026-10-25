using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Render.Abstractions;

public static class ReferencePlaneHorizonCoverage
{
    public const double ParallelEpsilon = 0.001;

    public static double VisualGroundDistance(ViewProjectionState state, double height, double maxFiniteDistance)
    {
        var cameraHeight = System.Math.Abs(state.Camera.Position.Z - height);
        var pitch = System.Math.Abs(state.Camera.Forward.Z);
        var halfFov = state.Camera.VerticalFovDegrees * System.Math.PI / 360.0;
        var angularGroundScale = System.Math.Max(pitch, System.Math.Sin(halfFov) * 0.5);
        var geometryDistance = cameraHeight / System.Math.Max(angularGroundScale, 0.05);
        var horizonDistance = maxFiniteDistance + cameraHeight / angularGroundScale;
        var distance = System.Math.Max(cameraHeight, System.Math.Max(maxFiniteDistance, System.Math.Max(geometryDistance, horizonDistance)));
        return double.IsFinite(distance) ? distance : cameraHeight;
    }

    public static Vector3d Surrogate(WorldRay ray, double height, double visualDistance)
    {
        var horizontalLength = System.Math.Sqrt((ray.Direction.X * ray.Direction.X) + (ray.Direction.Y * ray.Direction.Y));
        if (!double.IsFinite(horizontalLength) || horizontalLength < ParallelEpsilon) return new Vector3d(ray.Origin.X, ray.Origin.Y, height);
        var x = ray.Origin.X + ray.Direction.X / horizontalLength * visualDistance;
        var y = ray.Origin.Y + ray.Direction.Y / horizontalLength * visualDistance;
        return new Vector3d(x, y, height);
    }
}
