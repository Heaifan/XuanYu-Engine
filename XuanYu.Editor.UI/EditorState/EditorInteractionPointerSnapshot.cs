namespace XuanYu.Editor.UI;

public readonly record struct EditorInteractionPointerSnapshot(
    long PointerId,
    double StartX,
    double StartY,
    double CurrentX,
    double CurrentY,
    int PreviewCount)
{
    public static EditorInteractionPointerSnapshot Empty { get; } =
        new(0, 0, 0, 0, 0, 0);

    public bool IsEmpty => PointerId == 0;
    public double DeltaX => CurrentX - StartX;
    public double DeltaY => CurrentY - StartY;

    public EditorInteractionPointerSnapshot MoveTo(long pointerId, double x, double y)
    {
        if (pointerId != PointerId) return this;
        return this with { CurrentX = x, CurrentY = y, PreviewCount = PreviewCount + 1 };
    }

    public string Summary => IsEmpty
        ? "无"
        : $"指针={PointerId} 起点={StartX:F0},{StartY:F0} 当前={CurrentX:F0},{CurrentY:F0} 位移={DeltaX:F0},{DeltaY:F0} 预览={PreviewCount}";
}
