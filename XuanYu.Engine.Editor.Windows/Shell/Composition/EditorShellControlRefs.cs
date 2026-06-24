using Avalonia.Controls;
using XuanYu.Engine.Editor.Windows.Panels.DebugDock;
using XuanYu.Engine.Editor.Windows.Panels.Inspector;
using XuanYu.Engine.Editor.Windows.Panels.LeftDock;
using XuanYu.Engine.Editor.Windows.Panels.Status;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.Tools;

using FluidWarfare.Editor.Windows.Panels.LeftDock;
using FluidWarfare.Editor.Windows.Panels.WorldHierarchy;
using FluidWarfare.Editor.Windows.Panels.ProjectContentTree;
using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Panels.Inspector;
namespace XuanYu.Engine.Editor.Windows.Shell.Composition;

/// <summary>EditorShell 的 Avalonia 控件引用。由 Find() 从 VisualTree 查找。</summary>
public sealed record EditorShellControlRefs(
    InspectorPanel? Inspector,
    DebugDockPanel? DebugDock,
    StatusBarPanel? StatusBar,
    ViewportPlaceholderPanel? ViewportPlaceholder,
    VulkanViewportHostPanel? VulkanViewportHost,
    ProjectWorldDockPanel? DockPanel,
    ViewportToolPalette? ToolPalette,
    Button? RunMenuButton,
    MenuItem? PreferencesItem,
    MenuItem? InputBindingsItem,
    MenuItem? AboutItem)
{
    public static EditorShellControlRefs Find(UserControl shell)
    {
        return new(
            shell.FindControl<InspectorPanel>("InspectorPanel"),
            shell.FindControl<DebugDockPanel>("DebugDockPanel"),
            shell.FindControl<StatusBarPanel>("EditorStatusBarPanel"),
            shell.FindControl<ViewportPlaceholderPanel>("ViewportPlaceholderPanel"),
            shell.FindControl<VulkanViewportHostPanel>("VulkanViewportHostPanel"),
            shell.FindControl<ProjectWorldDockPanel>("ProjectWorldDockPanel"),
            shell.FindControl<ViewportToolPalette>("ViewportToolPalette"),
            shell.FindControl<Button>("RunMenuButton"),
            shell.FindControl<MenuItem>("PreferencesMenuItem"),
            shell.FindControl<MenuItem>("ShowInputBindingsMenuItem"),
            shell.FindControl<MenuItem>("AboutFluidWarfareMenuItem"));
    }
}
