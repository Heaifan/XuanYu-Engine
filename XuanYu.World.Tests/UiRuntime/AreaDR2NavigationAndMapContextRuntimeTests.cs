using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class AreaDR2NavigationAndMapContextRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public AreaDR2NavigationAndMapContextRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;
    [Fact]
    public void Left_uses_xy_tabs_and_switches_project_file_project()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run<(int Tabs, int Toggles, bool Project, bool File)>(() =>
        {
            var left = new Left { DataContext = new UiVm(null, seedInitialScene: false) };
            host.Show(left, 216, 860); left.UpdateLayout();
            var tabs = UiRuntimeTestHost.Descendants<XYTabs>(left).SingleOrDefault();
            if (tabs is null) return (Tabs: 0, Toggles: UiRuntimeTestHost.Descendants<XYToggleButton>(left).Count(), Project: false, File: false);
            tabs.Select("file"); Dispatcher.UIThread.RunJobs();
            var file = left.FindControl<StackPanel>("FileWorkspace")!.IsVisible;
            tabs.Select("project"); Dispatcher.UIThread.RunJobs();
            return (1, UiRuntimeTestHost.Descendants<XYToggleButton>(left).Count(),
                left.FindControl<ProjectWorkspace>("ProjectWorkspace")!.IsVisible, file);
        });
        Assert.Equal(1, state.Tabs); Assert.Equal(0, state.Toggles);
        Assert.True(state.Project); Assert.True(state.File);
    }
    [Fact]
    public void Right_uses_xy_tabs_for_inspector_hierarchy_debug_and_back()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run<(int Tabs, int Native, bool Hierarchy, bool Debug, bool Inspector, string Selection)>(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.AddCubeEntity(); vm.ToggleEditorMode(); var selected = vm.SelectionKey;
            var tabs = new EditorRightTabs { DataContext = vm };
            host.Show(tabs, 300, 420); tabs.UpdateLayout();
            var nav = UiRuntimeTestHost.Descendants<XYTabs>(tabs).SingleOrDefault();
            if (nav is null) return (Tabs: 0, Native: UiRuntimeTestHost.Descendants<TabControl>(tabs).Count(), Hierarchy: false, Debug: false, Inspector: false, Selection: "");
            nav.Select("hierarchy"); Dispatcher.UIThread.RunJobs();
            var hierarchy = UiRuntimeTestHost.Descendants<HierarchyWorkspace>(tabs).Single().IsEffectivelyVisible;
            nav.Select("debug"); Dispatcher.UIThread.RunJobs();
            var debug = tabs.FindControl<Grid>("DebugWorkspace") is not null;
            nav.Select("inspector"); Dispatcher.UIThread.RunJobs();
            return (1, UiRuntimeTestHost.Descendants<TabControl>(tabs).Count(), hierarchy, debug,
                UiRuntimeTestHost.Descendants<InspectorPanel>(tabs).Single().IsEffectivelyVisible, vm.SelectionKey == selected ? vm.SelectionKey : "");
        });
        Assert.Equal(1, state.Tabs); Assert.Equal(0, state.Native);
        Assert.True(state.Hierarchy); Assert.True(state.Debug); Assert.True(state.Inspector);
        Assert.NotEmpty(state.Selection);
    }
    [Fact]
    public void Map_context_renders_map_inspector_asset_and_layer_once()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.SwitchWorkspaceCommand.Execute("MapEditor"); vm.ToggleEditorMode();
            var right = new Right { DataContext = vm }; host.Show(right, 480, 720); Dispatcher.UIThread.RunJobs();
            UiRuntimeTestHost.Descendants<XYTabs>(right).Single().Select("inspector"); right.UpdateLayout();
            return Snapshot(right);
        });
        Assert.Equal(1, state.MapForms); Assert.Equal(1, state.MapPages); Assert.Equal(1, state.Layers);
    }
    [Fact]
    public void Clearing_entity_owner_restores_single_map_context()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.SwitchWorkspaceCommand.Execute("MapEditor"); vm.AddCubeEntity(); vm.ToggleEditorMode();
            vm.SelectedHierarchyItem = null; Dispatcher.UIThread.RunJobs();
            var right = new Right { DataContext = vm }; host.Show(right, 480, 720); Dispatcher.UIThread.RunJobs();
            UiRuntimeTestHost.Descendants<XYTabs>(right).Single().Select("inspector"); right.UpdateLayout();
            return Snapshot(right);
        });
        Assert.Equal(1, state.MapForms); Assert.Equal(1, state.MapPages); Assert.Equal(1, state.Layers);
    }
    [Fact]
    public void Navigation_sources_use_xy_tabs_without_native_tab_hosts()
    {
        var left = Read("Left", "Left.axaml"); var right = Read("Right", "EditorRightTabs.axaml");
        Assert.Contains("<xy:XYTabs", left); Assert.DoesNotContain("XYToggleButton", left);
        Assert.Contains("<xy:XYTabs", right); Assert.DoesNotContain("<TabControl", right); Assert.DoesNotContain("<TabItem", right);
    }
    static SnapshotData Snapshot(Right right) => new(
        UiRuntimeTestHost.Descendants<MapFormPanel>(right).Count(),
        UiRuntimeTestHost.Descendants<MapPagePanel>(right).Count(),
        UiRuntimeTestHost.Descendants<EditorLayerDock>(right).Count());

    static string Read(params string[] path) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", Path.Combine(path)));
    readonly record struct SnapshotData(int MapForms, int MapPages, int Layers);
}
