using System.IO;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class ContextToolbarR2Rework2RuntimeTests : IClassFixture<UiHeadlessFixture>
{
    readonly UiHeadlessFixture _fixture;
    public ContextToolbarR2Rework2RuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;
    static readonly string Root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
    static string Read(string rel) => File.ReadAllText(Path.Combine(Root, "XuanYu.Editor.UI", rel));

    [Fact]
    public void Toolbar_declares_feature_edit_tools_and_xyui_cascade_model()
    {
        var axaml = Read("Top/ContextToolBar.axaml");
        var code = Read("Top/ContextToolBar.axaml.cs");
        Assert.Contains("选择", axaml);
        Assert.Contains("框选", axaml);
        Assert.Contains("XYMenu.FromModels", code);
        Assert.Contains("XYMenuItemModel", code);
        Assert.Contains("XYMenuHost", axaml);
        Assert.DoesNotContain("<Popup", axaml);
        Assert.DoesNotContain("DrawMenuPopup", axaml);
        Assert.DoesNotContain("PlacementTarget", code);
        Assert.DoesNotContain("IsOpen", code);
        Assert.DoesNotContain("new XYSubMenu", code);
        Assert.DoesNotContain("Grid.Children.Add", code);
    }

    [Fact]
    public async Task Context_draw_commands_keep_three_existing_domain_routes()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.ToggleEditorModeCommand.Execute(null);
        Assert.True(await vm.BeginContextDrawingAsync("道路"));
        Assert.True(vm.IsRoadDrawingTool);
        Assert.True(vm.CancelRoadDrawing());
        Assert.True(await vm.BeginContextDrawingAsync("区域面"));
        Assert.True(vm.IsRegionDrawingTool);
        Assert.True(vm.CancelRegionDrawing());
        Assert.True(await vm.BeginContextDrawingAsync("地图标记"));
        Assert.True(vm.IsMarkerPlacementTool);
    }

    [Fact]
    public void Feature_tools_are_hidden_outside_edit_mode()
    {
        var axaml = Read("Top/ContextToolBar.axaml");
        Assert.Contains("IsVisible=\"{Binding IsRegionEditMode}\"", axaml);
    }

    [Fact]
    public void Engine_toolbar_exposes_all_three_cascade_levels()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var toolbar = new ContextToolBar(); host.Show(toolbar, 640, 120); toolbar.UpdateLayout();
            var menu = toolbar.FindControl<XYMenuHost>("DrawMenuHost")!.Menu!;
            Assert.Equal(3, menu.Items.OfType<XYMenuItem>().Count());
            Assert.All(menu.Items.OfType<XYMenuItem>(), item => Assert.NotNull(item.SubMenu));
            Assert.All(menu.Items.OfType<XYMenuItem>(), item =>
                Assert.Single(item.SubMenu!.ChildMenu.Items.OfType<XYMenuItem>()));
            Dispatcher.UIThread.RunJobs();
            Assert.True(menu.Items.OfType<XYMenuItem>().All(item => item.SubMenu!.ParentMenu == menu));
        });
    }

    [Fact]
    public void Opened_engine_menu_mounts_recursive_submenus_in_xyui_host()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var toolbar = new ContextToolBar(); host.Show(toolbar, 640, 120); toolbar.UpdateLayout();
            var split = toolbar.FindControl<XYSplitButton>("DrawSplitButton")!;
            split.MenuCommand!.Execute(null); Dispatcher.UIThread.RunJobs();
            var menuHost = toolbar.FindControl<XYMenuHost>("DrawMenuHost")!;
            var root = Assert.IsType<XYMenu>(menuHost.Menu);
            Assert.Equal(3, root.GetVisualDescendants().OfType<XYSubMenu>().Count());
            Assert.True(menuHost.IsOpen);
            var leaf = root.Items.OfType<XYMenuItem>().First().SubMenu!.ChildMenu
                .Items.OfType<XYMenuItem>().Single();
            leaf.Activate(); Dispatcher.UIThread.RunJobs();
            Assert.False(menuHost.IsOpen);
        });
    }
}
