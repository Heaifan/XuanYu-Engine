using System.Globalization;

namespace XuanYu.Render.Abstractions;

// MAP-A-R3-D2-F1-V3：比例尺只消费 ViewportMetricScale，不参与命中或渲染事实。
public readonly record struct ScaleIndicatorMetric(
    double DistanceMeters, double WidthDip, string Label)
{
    public const double FixedBarWidthDip = 104.0;
    public const double HysteresisRatio = 0.95;

    public static ScaleIndicatorMetric FromMetersPerDip(double metersPerDip)
        => FromMetersPerDip(metersPerDip, 0.0);

    public static ScaleIndicatorMetric FromMetersPerDip(double metersPerDip, double previousDistanceMeters)
    {
        if (!double.IsFinite(metersPerDip) || metersPerDip <= 0.0)
            return new ScaleIndicatorMetric(0.0, 0.0, "");
        var rawDistance = metersPerDip * FixedBarWidthDip;
        if (rawDistance < 100.0)
            return new ScaleIndicatorMetric(0.0, 0.0, "");
        var distance = ReferenceGridScale.LargestNiceSpacingAtMost(rawDistance);
        var next = ReferenceGridScale.NextNiceSpacing(previousDistanceMeters);
        if (previousDistanceMeters >= 100.0 && next > previousDistanceMeters &&
            rawDistance >= previousDistanceMeters * HysteresisRatio && rawDistance < next)
            distance = previousDistanceMeters;
        var width = distance / metersPerDip;
        return new ScaleIndicatorMetric(distance, width, Format(distance));
    }

    static string Format(double meters) => meters >= 1000.0
        ? $"{(meters / 1000.0).ToString("0", CultureInfo.InvariantCulture)} km"
        : $"{meters.ToString("0", CultureInfo.InvariantCulture)} m";
}
