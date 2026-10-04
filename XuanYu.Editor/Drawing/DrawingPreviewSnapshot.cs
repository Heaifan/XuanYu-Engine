using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public enum DrawingPreviewHoverState
{
    None,
    Hovered,
    Acceptable,
    Rejected
}

public sealed record DrawingPreviewSegment(MapPoint Start, MapPoint End);

public sealed record DrawingCloseCandidate(bool IsCandidate);

public sealed record DrawingPreviewSnapshot(
    MapPoint? CursorPosition,
    DrawingPreviewSegment? Segment,
    DrawingCloseCandidate CloseCandidate,
    DrawingPreviewHoverState HoverState,
    bool IsValid);
