namespace FluidWarfare.Render.Scene.Position;

/// <summary>
/// RenderScene 位置写入结果。
/// </summary>
public sealed record RenderObjectPositionWriteResult(
    bool IsSuccess,
    bool IsChanged,
    string Message,
    RenderScene? NewScene,
    RenderObjectPositionChange? Change);
