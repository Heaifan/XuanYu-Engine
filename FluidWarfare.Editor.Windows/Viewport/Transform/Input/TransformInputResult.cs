namespace FluidWarfare.Editor.Windows.Viewport.Transform.Input;

/// <summary>一次变换操作的结果。</summary>
public readonly record struct TransformInputResult(
    bool Handled,
    bool Started,
    bool Ended,
    double PreviewX,
    double PreviewY,
    double PreviewZ);
