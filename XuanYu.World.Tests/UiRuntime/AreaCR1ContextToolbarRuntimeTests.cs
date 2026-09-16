using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class AreaCR1ContextToolbarRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public AreaCR1ContextToolbarRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Region_tools_follow_the_region_edit_workspace_context()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var top = new Top { DataContext = vm };
            host.Show(top, 1200, 180); top.UpdateLayout();
            var toolbar = UiRuntimeTestHost.Descendants<ContextToolBar>(top).Single();
            var root = toolbar.FindControl<Border>("ContextRoot")!;
            var startup = root.IsEffectivelyVisible;
            vm.ToggleEditorMode(); Dispatcher.UIThread.RunJobs(); top.UpdateLayout();
            var mapEdit = root.IsEffectivelyVisible;
            vm.SwitchWorkspaceCommand.Execute("RegionEditor");
            Dispatcher.UIThread.RunJobs(); top.UpdateLayout();
            var scroll = UiRuntimeTestHost.Descendants<ScrollViewer>(top).Single();
            return (startup, mapEdit, regionEdit: root.IsEffectivelyVisible,
                scrollHosts: UiRuntimeTestHost.Descendants<ScrollViewer>(top).Count(),
                horizontalBar: scroll.HorizontalScrollBarVisibility);
        });

        Assert.False(state.startup);
        Assert.False(state.mapEdit);
        Assert.True(state.regionEdit);
        Assert.Equal(1, state.scrollHosts);
        Assert.Equal(ScrollBarVisibility.Hidden, state.horizontalBar);
    }

    [Fact]
    public void Feature_context_toolbar_is_available_after_workspace_switch()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var top = new Top { DataContext = vm };
            host.Show(top, 1200, 180); top.UpdateLayout();
            vm.ToggleEditorMode();
            vm.SwitchWorkspaceCommand.Execute("RegionEditor");
            Dispatcher.UIThread.RunJobs(); top.UpdateLayout();
            var scroll = top.FindControl<ScrollViewer>("ContextToolScrollHost")!;
            var toolbar = UiRuntimeTestHost.Descendants<ContextToolBar>(top).Single();
            var root = toolbar.FindControl<Border>("ContextRoot")!;
            return (visible: root.IsEffectivelyVisible, scrollPresent: scroll is not null);
        });

        Assert.True(state.visible);
        Assert.True(state.scrollPresent);
    }
}
