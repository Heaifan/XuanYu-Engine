namespace XuanYu.Editor.UI;

public enum TreeGuideSegmentKind
{
    Blank,
    Full,
    Tee,
    Elbow
}

public sealed record TreeGuideSegment(int Depth, TreeGuideSegmentKind Kind);
