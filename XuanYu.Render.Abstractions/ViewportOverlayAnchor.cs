namespace XuanYu.Render.Abstractions;

public enum ViewportOverlayAnchor
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center
}

public readonly record struct ViewportOverlayRect(
    double X, double Y, double Width, double Height)
{
    public double Right => X + Width;
    public double Bottom => Y + Height;
}

public readonly record struct ViewportOverlayLayoutRequest(
    double ViewportWidthDip,
    double ViewportHeightDip,
    double DesiredWidthDip,
    double DesiredHeightDip,
    double MarginXDip,
    double MarginYDip,
    ViewportOverlayAnchor Anchor);
