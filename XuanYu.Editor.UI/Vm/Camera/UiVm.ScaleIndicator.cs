using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public string ScaleIndicatorText { get; private set; } = "";
    public double ScaleIndicatorWidthDip { get; private set; } = 80.0;
    public bool IsScaleIndicatorVisible { get; private set; }

    void UpdateScaleIndicator()
    {
        if (_lastViewport is not { } viewport)
        {
            SetScaleIndicator(false, "", 80.0);
            return;
        }
        var camera = new RenderCameraProjection(_camera.Position, _camera.Forward, _camera.Up,
            _camera.VerticalFovDegrees, _camera.NearPlane, _camera.FarPlane, _camera.Revision,
            _camera.Mode, _camera.OrthographicScale);
        if (!ViewportMetricScale.TryCreate(camera, viewport,
                MapSession.CurrentMap.Surface.BaseHeightMeters, out var metric))
        {
            SetScaleIndicator(false, "", 80.0);
            return;
        }
        var bar = ScaleIndicatorMetric.FromMetersPerDip(metric.MetersPerDip);
        SetScaleIndicator(bar.DistanceMeters > 0.0, bar.Label, bar.WidthDip);
    }

    void SetScaleIndicator(bool visible, string text, double width)
    {
        if (IsScaleIndicatorVisible == visible && ScaleIndicatorText == text &&
            System.Math.Abs(ScaleIndicatorWidthDip - width) < 0.01) return;
        IsScaleIndicatorVisible = visible;
        ScaleIndicatorText = text;
        ScaleIndicatorWidthDip = width;
        OnPropertyChanged(nameof(IsScaleIndicatorVisible));
        OnPropertyChanged(nameof(ScaleIndicatorText));
        OnPropertyChanged(nameof(ScaleIndicatorWidthDip));
    }
}
