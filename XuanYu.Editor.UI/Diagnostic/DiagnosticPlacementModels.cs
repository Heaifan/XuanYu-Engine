using Avalonia;

namespace XuanYu.Editor.UI.Diagnostic;

public enum DiagnosticPlacementTargetKind
{
    SmallControl, LargeSurface, Viewport, PointerAnchored
}

public enum DiagnosticPlacementKind
{
    Right, Left, Bottom, Top, RightBottom, RightTop,
    LeftBottom, LeftTop, Fallback
}

public sealed record DiagnosticPlacementRequest(
    DiagnosticPlacementTargetKind TargetKind,
    Rect TargetBounds,
    Point Pointer,
    Size CardSize,
    Rect AvailableBounds,
    double SafeDistance = 8);

public sealed record DiagnosticPlacementResult(
    Rect CardBounds,
    DiagnosticPlacementKind Placement,
    bool WasClamped,
    bool UsedFallback,
    bool DoesNotFit);
