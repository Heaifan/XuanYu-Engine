using System.Globalization;

namespace XuanYu.Render.Abstractions;

// MAP-A-R3-D2-F1-V3：比例尺只消费 ViewportMetricScale，不参与命中或渲染事实。
public readonly record struct ScaleIndicatorMetric(
    double DistanceMeters, double WidthDip, string Label)
{
    public const double FixedBarWidthDip = 104.0;

    public static ScaleIndicatorMetric FromMetersPerDip(double metersPerDip)
    {
        if (!double.IsFinite(metersPerDip) || metersPerDip <= 0.0)
            return new ScaleIndicatorMetric(0.0, 0.0, "");
        var distance = metersPerDip * FixedBarWidthDip;
        return new ScaleIndicatorMetric(distance, FixedBarWidthDip, Format(distance));
    }

    static string Format(double meters) => meters >= 1000.0
        ? $"{(meters / 1000.0).ToString("0.##", CultureInfo.InvariantCulture)} km"
        : $"{meters.ToString("0.##", CultureInfo.InvariantCulture)} m";
}
