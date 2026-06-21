namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Resize;

/// <summary>渲染重绘的执行结果。Shell 根据 Action 决定后续 Apply 操作。</summary>
public sealed record Scene3dResizeRenderResult(
    Scene3dResizeAction Action,
    string? LogMessage,
    bool LogIsWarning,
    int NewRenderSeq)
{
    public static readonly Scene3dResizeRenderResult Skipped = new(Scene3dResizeAction.Skipped, null, false, 0);
    public static readonly Scene3dResizeRenderResult NotReady = new(Scene3dResizeAction.NotReady, null, false, 0);

    public static Scene3dResizeRenderResult Resized(string log) =>
        new(Scene3dResizeAction.Resized, log, false, 0);

    public static Scene3dResizeRenderResult Failure(string? log, int newSeq) =>
        new(Scene3dResizeAction.ClearFallbackAfterFailure, log, true, newSeq);

    public static Scene3dResizeRenderResult ClearOnly(int newSeq) =>
        new(Scene3dResizeAction.ClearFallback, null, false, newSeq);
}

/// <summary>渲染重绘后的后续操作类型。</summary>
public enum Scene3dResizeAction
{
    Skipped,                   // 后端不可用或渲染锁占用
    NotReady,                  // 尺寸无效
    Resized,                   // Session.Resize 成功
    ClearFallback,             // Session 未运行 → 只做 Clear
    ClearFallbackAfterFailure  // Resize 失败 → Stop Session + Clear
}
