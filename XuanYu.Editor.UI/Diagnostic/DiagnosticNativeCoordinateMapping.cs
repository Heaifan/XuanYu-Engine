using Avalonia;

namespace XuanYu.Editor.UI;

internal static class DiagnosticNativeCoordinateMapping
{
    public static Point ToLayer(PixelPoint screen, PixelPoint layerOrigin, double scaling)
    {
        var scale = scaling > 0 ? scaling : 1;
        return new Point((screen.X - layerOrigin.X) / scale,
            (screen.Y - layerOrigin.Y) / scale);
    }
}
