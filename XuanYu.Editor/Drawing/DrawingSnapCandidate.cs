using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public enum DrawingSnapKind
{
    None,
    Vertex,
    Edge,
    Surface,
    Grid
}

public sealed record DrawingSnapCandidate(
    MapPoint CandidatePosition,
    DrawingSnapKind Kind,
    string? SourceIdentity,
    double? Distance,
    int? Priority,
    bool IsValid);
