namespace FluidWarfare.Editor.Windows.Panels.Viewport;

/// <summary>
/// 视口 RenderScene 调试列表中的单个渲染对象显示摘要。
/// 只保存显示文本，不依赖 Render、Engine 或 Project。
/// </summary>
public sealed record ViewportRenderObjectSummary(
    string DisplayName,
    string VisualKindText,
    string PositionText,
    string? SourcePath);
