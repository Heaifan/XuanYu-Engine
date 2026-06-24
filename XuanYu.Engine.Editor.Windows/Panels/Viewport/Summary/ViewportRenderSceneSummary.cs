namespace XuanYu.Engine.Editor.Windows.Panels.Viewport;

/// <summary>
/// 视口 RenderScene 调试列表显示模型，保存多个渲染对象摘要。
/// 不依赖 Render、Engine 或 Project。
/// </summary>
public sealed record ViewportRenderSceneSummary(
    IReadOnlyList<ViewportRenderObjectSummary> Objects)
{
    public static ViewportRenderSceneSummary Empty { get; } =
        new ViewportRenderSceneSummary([]);
}
