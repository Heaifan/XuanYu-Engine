using XuanYu.Engine.Editor.Windows.Panels.Inspector;
using XuanYu.Engine.Editor.Windows.Panels.LeftDock;
using XuanYu.Engine.Editor.Windows.Panels.Status;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;

namespace XuanYu.Engine.Editor.Windows.Shell.Panels;

/// <summary>Panel Apply Route 持有的面板引用。Shell 在 FindShellControls 后初始化。</summary>
public sealed record EditorPanelApplyPanels(
    InspectorPanel? Inspector,
    StatusBarPanel? StatusBar,
    ViewportPlaceholderPanel? ViewportPlaceholder,
    ProjectWorldDockPanel? DockPanel);
