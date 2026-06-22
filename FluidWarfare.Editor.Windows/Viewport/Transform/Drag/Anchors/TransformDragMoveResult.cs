namespace FluidWarfare.Editor.Windows.Viewport.Transform.Drag;

/// <summary>变换拖动移动结果。仅含 Handled 标志，无需位移动画相关字段。</summary>
public readonly record struct TransformDragMoveResult(bool Handled)
{
    public static readonly TransformDragMoveResult NotHandled = default;
    public static TransformDragMoveResult HandledResult => new(true);
}
