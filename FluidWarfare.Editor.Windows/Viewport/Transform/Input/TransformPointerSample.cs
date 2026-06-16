namespace FluidWarfare.Editor.Windows.Viewport.Transform.Input;

/// <summary>一次鼠标采样。</summary>
public readonly record struct TransformPointerSample(
    double X,
    double Y,
    int ButtonCode);
