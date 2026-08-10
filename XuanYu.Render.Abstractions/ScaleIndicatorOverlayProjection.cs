namespace XuanYu.Render.Abstractions;

public readonly record struct ScaleIndicatorOverlayProjection(
    bool Visible,
    string Label,
    double BarWidthDip)
{
    public static ScaleIndicatorOverlayProjection Hidden => new(false, "", 0.0);
}
