namespace XuanYu.Engine.Editor.Windows.Panels.Viewport;

/// <summary>
/// 视口占位显示模型，保存当前选中实体的名称、EntityId、来源路径、位置文本与视觉类型。
/// 不读取 World，不依赖 Engine，不写日志。
/// </summary>
public sealed record ViewportEntitySummary(
    string DisplayName,
    string EntityIdText,
    string PositionText,
    string? SourcePath,
    string VisualKindText);
