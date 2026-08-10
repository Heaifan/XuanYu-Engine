using System.Globalization;

namespace XuanYu.Render.Abstractions;

// MAP-A-R3-D2-F1-V3：比例尺只消费 ViewportMetricScale，不参与命中或渲染事实。
public readonly record struct ScaleIndicatorMetric(
    double DistanceMeters, double WidthDip, string Label)
{
    public static ScaleIndicatorMetric FromMetersPerDip(double metersPerDip)
    {
        if (!double.IsFinite(metersPerDip) || metersPerDip <= 0.0)
            return new ScaleIndicatorMetric(0.0, 0.0, "");
        var maxDistance = metersPerDip * 160.0;
        var distance = ReferenceGridScale.LargestNiceSpacingAtMost(maxDistance);
        var width = distance / metersPerDip;
        var next = ReferenceGridScale.NextNiceSpacing(distance);
        if (width < 80.0 && next / metersPerDip <= 160.0)
        {
            distance = next;
            width = distance / metersPerDip;
        }
        return new ScaleIndicatorMetric(distance, width, Format(distance));
    }

    static string Format(double meters) => meters >= 1000.0
        ? $"{(meters / 1000.0).ToString("0.##", CultureInfo.InvariantCulture)} km"
        : $"{meters.ToString("0.##", CultureInfo.InvariantCulture)} m";
}
