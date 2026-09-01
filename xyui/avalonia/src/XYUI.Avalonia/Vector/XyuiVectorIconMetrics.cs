using Avalonia;

namespace XYUI.Avalonia.Vector;

public readonly record struct XyuiVectorIconMetrics(
    double LogicalViewport,
    Rect GeometryBounds,
    global::Avalonia.Vector OpticalOffset)
{
    public bool HasOpticalCorrection => OpticalOffset != new global::Avalonia.Vector(0, 0);
}
