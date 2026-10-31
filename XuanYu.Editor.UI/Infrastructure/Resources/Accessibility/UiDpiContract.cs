namespace XuanYu.Editor.UI;

public static class UiDpiContract
{
    public static readonly double[] SupportedScales = [1.0, 1.25, 1.5, 1.75, 2.0];
    public const double MinWindowWidthDip = 1280;
    public const double MinWindowHeightDip = 720;
    public const double RecommendedWindowWidthDip = 1440;
    public const double RecommendedWindowHeightDip = 900;
    public const double InspectorNarrowThresholdDip = 320;
    public const double MapFormWideThresholdDip = 360;

    public static double Physical(double dip, double scale) => dip * scale;
    public static bool IsSupported(double scale) => SupportedScales.Contains(scale);
    public static bool UsesDipThresholds => InspectorNarrowThresholdDip == 320
        && MapFormWideThresholdDip == 360;
}
