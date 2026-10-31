using XuanYu.Editor.Camera;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    int _lastFarDiagnosticBucket = int.MinValue;
    bool _farDistanceLimitReported;

    void TraceFarDolly(CameraFrameResult result)
    {
        if (result.Camera.Position.DistanceTo(result.ObservationCenter) < CameraNavigation.MaxDistanceMeters)
            _farDistanceLimitReported = false;
        if (result.Camera.Position.DistanceTo(result.ObservationCenter) >= CameraNavigation.MaxDistanceMeters &&
            !_farDistanceLimitReported)
        {
            _farDistanceLimitReported = true;
            _logBus.Warning(EditorLogSource.Input, EditorLogCategory.Command, "相机距离已到工作上限",
                $"上限={CameraNavigation.MaxDistanceMeters:g}m；已停止继续拉远。");
        }
        var data = CameraFarProjectionDiagnostic.Create(result.Camera,
            result.ObservationCenter, _lastViewport);
        var bucket = data.Distance <= 0.0 ? int.MinValue : (int)System.Math.Floor(System.Math.Log10(data.Distance));
        if (bucket < 4 || bucket == _lastFarDiagnosticBucket) return;
        _lastFarDiagnosticBucket = bucket;
        var metric = data.MetricValid
            ? $"有效；X={data.MetersPerDipX:g6} m/DIP；Y={data.MetersPerDipY:g6} m/DIP"
            : "无效";
        _logBus.Warning(EditorLogSource.Input, EditorLogCategory.Command, "极远相机诊断",
            $"距离={data.Distance:g6}m；Near={result.Camera.NearPlane:g6}m；Far={result.Camera.FarPlane:g6}m；" +
            $"Metric={metric}；中心射线 t={data.CenterRayT:g6}m；位置={result.Camera.Position}；" +
            $"观察中心={result.ObservationCenter}。未构建 ViewProjection。");
    }
}
