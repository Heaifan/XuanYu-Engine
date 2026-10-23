using XuanYu.Core.Space;

namespace XuanYu.Core.Diagnostics;

public static partial class TerrainReplayOptions
{
    public static CameraState ApplyCamera(CameraState camera, TerrainReplayVariant variant) =>
        ApplyCamera(camera, variant, FarPlaneOverride);

    public static CameraState ApplyCamera(CameraState camera, TerrainReplayVariant variant,
        double? farPlaneOverride)
    {
        var near = variant == TerrainReplayVariant.ReducedNearPlane
            ? global::System.Math.Max(0.01, camera.NearPlane * 0.5) : camera.NearPlane;
        var far = variant == TerrainReplayVariant.ExpandedFarPlane ? camera.FarPlane * 2.0
            : farPlaneOverride is > 0 && farPlaneOverride > camera.NearPlane
                ? farPlaneOverride.Value : camera.FarPlane;
        if (near == camera.NearPlane && far == camera.FarPlane) return camera;
        return new(camera.Position, camera.Forward, camera.Up, camera.VerticalFovDegrees,
            near, far, camera.Revision, camera.Mode, camera.OrthographicScale);
    }
}
