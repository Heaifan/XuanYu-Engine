namespace XuanYu.Render.Abstractions;

public static class ViewportOverlayLayoutResolver
{
    public static ViewportOverlayRect Resolve(ViewportOverlayLayoutRequest request)
    {
        var viewportWidth = NonNegative(request.ViewportWidthDip);
        var viewportHeight = NonNegative(request.ViewportHeightDip);
        var width = Math.Min(NonNegative(request.DesiredWidthDip), viewportWidth);
        var height = Math.Min(NonNegative(request.DesiredHeightDip), viewportHeight);
        var marginX = NonNegative(request.MarginXDip);
        var marginY = NonNegative(request.MarginYDip);
        var x = request.Anchor switch
        {
            ViewportOverlayAnchor.TopRight or ViewportOverlayAnchor.BottomRight
                => viewportWidth - marginX - width,
            ViewportOverlayAnchor.Center => (viewportWidth - width) * 0.5 + request.MarginXDip,
            _ => marginX
        };
        var y = request.Anchor switch
        {
            ViewportOverlayAnchor.BottomLeft or ViewportOverlayAnchor.BottomRight
                => viewportHeight - marginY - height,
            ViewportOverlayAnchor.Center => (viewportHeight - height) * 0.5 + request.MarginYDip,
            _ => marginY
        };
        return new ViewportOverlayRect(
            Math.Clamp(x, 0.0, viewportWidth - width),
            Math.Clamp(y, 0.0, viewportHeight - height),
            width,
            height);
    }

    static double NonNegative(double value) =>
        double.IsFinite(value) ? Math.Max(0.0, value) : 0.0;
}
