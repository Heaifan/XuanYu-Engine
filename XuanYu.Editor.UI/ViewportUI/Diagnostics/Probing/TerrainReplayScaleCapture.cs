using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

internal static class TerrainReplayScaleCapture
{
    public static object Capture(UiVm vm, CameraState camera, ViewportState viewport)
    {
        var projection = new RenderCameraProjection(camera.Position, camera.Forward,
            camera.Up, camera.VerticalFovDegrees, camera.NearPlane, camera.FarPlane,
            camera.Revision, camera.Mode, camera.OrthographicScale);
        var valid = ScaleBarGroundProjection.TryCreate(projection, viewport,
            vm.MapSession.CurrentMap.Surface.BaseHeightMeters, out var ground);
        return new
        {
            Text = vm.ScaleIndicatorText,
            WidthDip = vm.ScaleIndicatorWidthDip,
            IsGroundProjectionValid = valid,
            MetersPerDip = valid ? ground.MetersPerDip : 0,
            EndpointDistanceMeters = valid ? ground.EndpointDistanceMeters : 0,
            ReferenceY = valid ? ground.ReferenceY : 0,
            CameraToPivotMeters = camera.Position.DistanceTo(vm.ObservationCenter)
        };
    }
}
