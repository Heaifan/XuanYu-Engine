namespace XuanYu.Render.Abstractions;

public readonly record struct ScaleIndicatorOverlayProjection(
    bool Visible,
    string Label,
    double BarWidthDip)
{
    public const double CardWidthDip = 128.0;
    public const double CardHeightDip = 28.0;

    public static ScaleIndicatorOverlayProjection Hidden => new(false, "", 0.0);
}
