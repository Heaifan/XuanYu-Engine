using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

public readonly record struct CameraDepthPrecisionSample(
    double Distance, double Near, double Far, double Ratio,
    double DepthValue, double AdjacentDepth, double DepthUlp,
    double WorldSpaceResolutionMeters, double PrecisionImprovementFactor,
    bool IsFinite);

public static class CameraDepthPrecisionDiagnostic
{
    public static double ForwardDepth(double distance, double near, double far)
    {
        Validate(distance, near, far);
        return far / (far - near) - (far * near) / ((far - near) * distance);
    }

    public static CameraDepthPrecisionSample Measure(double distance, double far, double near)
    {
        Validate(distance, near, far);
        var depth = (float)ForwardDepth(distance, near, far);
        var adjacent = depth >= 1.0f ? float.BitDecrement(depth) : float.BitIncrement(depth);
        var resolution = WorldInterval(depth, adjacent, near, far);
        var baseline = Resolution(distance, far, 0.05);
        var finite = double.IsFinite(depth) && double.IsFinite(adjacent) &&
            double.IsFinite(resolution) && resolution > 0.0;
        return new(distance, near, far, far / near, depth, adjacent,
            Math.Abs(adjacent - depth), resolution, baseline / resolution, finite);
    }

    public static CameraDepthPrecisionSample Create(CameraState camera, Vector3d center) =>
        Measure(camera.Position.DistanceTo(center), camera.FarPlane, camera.NearPlane);

    public static string DebugText(CameraDepthPrecisionSample sample) =>
        $"[DEBUG][CameraDepthPrecision] Distance={sample.Distance:0.###}m " +
        $"Near={sample.Near:0.###}m Far={sample.Far:0.###}m Ratio={sample.Ratio:0.###}:1 " +
        $"Depth={sample.DepthValue:R} DepthULP={sample.DepthUlp:R} " +
        $"WorldResolution={sample.WorldSpaceResolutionMeters:R}m";

    static double Resolution(double distance, double far, double near)
    {
        var depth = (float)ForwardDepth(distance, near, far);
        return WorldInterval(depth, AdjacentDepth(distance, near, far), near, far);
    }

    static float AdjacentDepth(double distance, double near, double far)
    {
        var depth = (float)ForwardDepth(distance, near, far);
        return depth >= 1.0f ? float.BitDecrement(depth) : float.BitIncrement(depth);
    }

    static double WorldInterval(float depth, float adjacent, double near, double far)
    {
        var d1 = far - depth * (far - near);
        var d2 = far - adjacent * (far - near);
        return Math.Abs(far * near * (adjacent - depth) * (far - near) / (d1 * d2));
    }

    static void Validate(double distance, double near, double far)
    {
        if (!double.IsFinite(distance) || distance <= 0.0 || !double.IsFinite(near) ||
            near <= 0.0 || !double.IsFinite(far) || far <= near || distance > far)
            throw new ArgumentOutOfRangeException();
    }
}
