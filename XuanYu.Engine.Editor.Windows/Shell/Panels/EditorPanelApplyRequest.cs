using FluidWarfare.Editor.Windows.Panels.Inspector;
using FluidWarfare.Editor.Windows.Panels.LeftDock;
using FluidWarfare.Editor.Windows.Panels.Status;
using FluidWarfare.Editor.Windows.Panels.Viewport;

namespace FluidWarfare.Editor.Windows.Shell.Panels;

/// <summary>Panel Apply Route 持有的面板引用。Shell 在 FindShellControls 后初始化。</summary>
public sealed record EditorPanelApplyPanels(
    InspectorPanel? Inspector,
    StatusBarPanel? StatusBar,
    ViewportPlaceholderPanel? ViewportPlaceholder,
    ProjectWorldDockPanel? DockPanel);
