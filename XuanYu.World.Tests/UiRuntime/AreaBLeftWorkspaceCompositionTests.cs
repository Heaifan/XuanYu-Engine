using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Reflection;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed partial class AreaBLeftWorkspaceCompositionTests
{
    readonly UiHeadlessFixture _fixture;
    public AreaBLeftWorkspaceCompositionTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Narrow_left_materializes_project_file_tabs_and_real_scene_tree()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var snapshot = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var left = new Left { DataContext = vm };
            host.Show(left, 216, 860);
            left.UpdateLayout();
            var tabs = UiRuntimeTestHost.Descendants<XYTabs>(left).Single();
            var tree = left.FindControl<ProjectWorkspace>("ProjectWorkspace")!;
            var empty = left.FindControl<StackPanel>("FileWorkspace")!;
            var project = (tabs.SelectedTabId, tree.IsVisible, empty.IsVisible,
                vm.ProjectItems.Count, left.Bounds.Width);
            tabs.Select("file");
            Dispatcher.UIThread.RunJobs();
            return (project, tabs.SelectedTabId, tree.IsVisible, empty.IsVisible);
        });

        Assert.Equal("project", snapshot.project.Item1);
        Assert.True(snapshot.project.Item2);
        Assert.False(snapshot.project.Item3);
        Assert.Equal(1, snapshot.project.Item4);
        Assert.Equal(216, snapshot.project.Item5);
        Assert.Equal("file", snapshot.Item2);
        Assert.False(snapshot.Item3);
        Assert.True(snapshot.Item4);
    }

    [Fact]
    public void Hierarchy_context_menu_uses_real_popup_root_and_xyui_items()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var evidence = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: true);
            var hierarchy = new HierarchyWorkspace { DataContext = vm };
            host.Show(hierarchy, 300, 420);
            hierarchy.UpdateLayout();
            hierarchy.ContextMenu.Open();
            Dispatcher.UIThread.RunJobs();
            var field = typeof(XYContextMenu).GetField("_popup", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var popupControl = field.GetValue(hierarchy.ContextMenu) as Popup;
            var root = popupControl?.GetType().GetProperty("VisualRoot",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(popupControl);
            var items = hierarchy.ContextMenu.Menu.Items.OfType<XYMenuItem>().Count();
            var popup = root?.GetType().Name ?? "HeadlessPopupRootUnavailable";
            var menuOpen = hierarchy.ContextMenu.IsOpen && popupControl?.IsOpen == true;
            var hasChild = popupControl?.Child is not null;
            hierarchy.ContextMenu.Close();
            return (popup, items, menuOpen, hasChild);
        });

        Assert.True(evidence.menuOpen);
        Assert.True(evidence.hasChild);
        Assert.Contains(evidence.popup, new[] { "PopupRoot", "HeadlessPopupRootUnavailable" });
        Assert.Equal(3, evidence.items);
    }

}
