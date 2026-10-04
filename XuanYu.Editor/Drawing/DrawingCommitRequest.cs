using XuanYu.World.Map;

namespace XuanYu.Editor.Drawing;

public sealed record DrawingCommitRequest(
    DrawingPrimitiveKind PrimitiveKind,
    IReadOnlyList<MapPoint> Points);
