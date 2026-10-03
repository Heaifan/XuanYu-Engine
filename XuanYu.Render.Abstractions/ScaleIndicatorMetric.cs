using System.Globalization;

namespace XuanYu.Render.Abstractions;

// 比例尺只消费地面端点测量结果，不参与命中、渲染事实或相机策略。
public readonly record struct ScaleIndicatorMetric(
    double DistanceMeters, double WidthDip, string Label)
{
    public const double FixedBarWidthDip = 104.0;
    public bool IsVisible => DistanceMeters > 0.0 && WidthDip > 0.0 && !string.IsNullOrWhiteSpace(Label);
    public const double HysteresisRatio = 0.95;

    public static ScaleIndicatorMetric FromMetersPerDip(double metersPerDip)
        => FromMetersPerDip(metersPerDip, 0.0);

    public static ScaleIndicatorMetric FromMetersPerDip(double metersPerDip, double previousDistanceMeters)
    {
        if (!double.IsFinite(metersPerDip) || metersPerDip <= 0.0)
            return new ScaleIndicatorMetric(0.0, 0.0, "");
        var rawDistance = metersPerDip * FixedBarWidthDip;
        var distance = LargestBarSpacingAtMost(rawDistance);
        var nextDistance = NextBarSpacing(distance);
        if (distance / metersPerDip < 80.0 && nextDistance / metersPerDip <= 160.0)
            distance = nextDistance;
        var next = NextBarSpacing(previousDistanceMeters);
        if (previousDistanceMeters >= 100.0 && next > previousDistanceMeters &&
            rawDistance >= previousDistanceMeters * HysteresisRatio && rawDistance < next)
            distance = previousDistanceMeters;
        var width = distance / metersPerDip;
        return new ScaleIndicatorMetric(distance, width, Format(distance));
    }

    static string Format(double meters) => meters >= 1000.0
        ? $"{(meters / 1000.0).ToString("0", CultureInfo.InvariantCulture)} km"
        : $"{meters.ToString("0", CultureInfo.InvariantCulture)} m";

    static double LargestBarSpacingAtMost(double value)
    {
        var decade = Math.Pow(10.0, Math.Floor(Math.Log10(value)));
        if (decade * 5.0 <= value) return decade * 5.0;
        if (decade * 2.0 <= value) return decade * 2.0;
        return decade;
    }

    static double NextBarSpacing(double spacing)
    {
        if (!double.IsFinite(spacing) || spacing <= 0.0) return 0.0;
        var decade = Math.Pow(10.0, Math.Floor(Math.Log10(spacing)));
        return spacing >= decade * 5.0 ? decade * 10.0
            : spacing >= decade * 2.0 ? decade * 5.0 : decade * 2.0;
    }
}
