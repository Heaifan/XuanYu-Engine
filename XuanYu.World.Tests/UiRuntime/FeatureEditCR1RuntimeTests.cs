using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class FeatureEditCR1RuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public FeatureEditCR1RuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Manage_mode_hides_context_tools()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var isVisible = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var toolbar = new ContextToolBar { DataContext = vm };
            host.Show(toolbar, 800, 100); toolbar.UpdateLayout();
            
            var root = toolbar.FindControl<Border>("ContextRoot");
            return root?.IsVisible ?? false;
        });
        
        Assert.False(isVisible);
    }

    [Fact]
    public void Map_edit_mode_shows_context_tools_and_root_menu()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.ToggleEditorModeCommand.Execute(null);
            var toolbar = new ContextToolBar { DataContext = vm };
            host.Show(toolbar, 800, 100); toolbar.UpdateLayout();
            
            var root = toolbar.FindControl<Border>("ContextRoot");
            Assert.True(root?.IsVisible ?? false);
            
            var split = toolbar.FindControl<XYSplitButton>("DrawSplitButton");
            Assert.NotNull(split);
            split!.MenuCommand!.Execute(null);
            Dispatcher.UIThread.RunJobs();
            var popup = toolbar.FindControl<Popup>("DrawMenuPopup");
            var labels = (popup!.Child as XYMenu)!.Items.OfType<XYMenuItem>().Select(x => x.Label).ToArray();
            Assert.Equal(["点", "线", "面"], labels);
            Assert.All((popup.Child as XYMenu)!.Items.OfType<XYMenuItem>(), item => Assert.True(item.HasSubMenu));
        });
    }

    [Fact]
    public void Root_line_then_road_leaf_starts_real_drawing_transaction()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(async () =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.ToggleEditorModeCommand.Execute(null);
            var toolbar = new ContextToolBar { DataContext = vm };
            host.Show(toolbar, 800, 100); toolbar.UpdateLayout();
            var split = toolbar.FindControl<XYSplitButton>("DrawSplitButton")!;
            split.MainCommand!.Execute(null); Dispatcher.UIThread.RunJobs();
            var root = (toolbar.FindControl<Popup>("DrawMenuPopup")!.Child as XYMenu)!;
            root.Items.OfType<XYMenuItem>().Single(x => x.Label == "线").Activate();
            var submenu = Assert.IsType<XYSubMenu>(toolbar.FindControl<Popup>("DrawMenuPopup")!.Child);
            submenu.ChildMenu.Items.OfType<XYMenuItem>().Single().Activate();
            await Task.Delay(1);
            Assert.True(vm.IsDrawingTransactionActive);
            Assert.True(vm.IsRoadDrawingTool);
        });
    }
}
