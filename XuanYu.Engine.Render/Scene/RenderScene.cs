namespace FluidWarfare.Render.Scene;

/// <summary>
/// 保存一帧或当前状态下的可渲染对象集合。
/// </summary>
public sealed record RenderScene(
    IReadOnlyList<RenderObjectInfo> Objects)
{
    public static RenderScene Empty { get; } =
        new RenderScene([]);
}
