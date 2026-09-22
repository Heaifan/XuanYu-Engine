using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class ContextToolbarPopupHostRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public ContextToolbarPopupHostRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Cascading_menu_reopens_and_switches_siblings_without_zero_layout()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var toolbar = new ContextToolBar { DataContext = vm };
            host.Show(toolbar, 800, 100);
            var split = toolbar.FindControl<XYSplitButton>("DrawSplitButton")!;
            var root = toolbar.FindControl<XYMenu>("DrawMenu")!;
            var submenuPopup = toolbar.FindControl<Popup>("DrawSubMenuPopup")!;
            var submenu = toolbar.FindControl<XYMenu>("DrawChildMenu")!;
            for (var i = 0; i < 10; i++)
            {
                split.MenuCommand!.Execute(null); Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
                root.Items.OfType<XYMenuItem>().Single(x => x.Label == (i % 2 == 0 ? "点" : "线")).Activate();
                Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
                Assert.True(submenuPopup.IsOpen);
                Assert.Same(submenuPopup.Child, submenu.GetVisualParent());
                Assert.True(submenu.Bounds.Width > 0 && submenu.Bounds.Height > 0);
                toolbar.FindControl<Popup>("DrawMenuPopup")!.IsOpen = false;
                Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
                Assert.False(toolbar.FindControl<Popup>("DrawMenuPopup")!.IsOpen);
                Assert.False(submenuPopup.IsOpen);
            }
        });
    }

    [Fact]
    public void Root_menu_keeps_three_visible_vertical_items_while_submenu_is_open()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var toolbar = new ContextToolBar { DataContext = vm };
            host.Show(toolbar, 1000, 180);
            var popup = toolbar.FindControl<Popup>("DrawMenuPopup")!;
            toolbar.FindControl<XYSplitButton>("DrawSplitButton")!.MenuCommand!.Execute(null);
            Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
            var root = toolbar.FindControl<XYMenu>("DrawMenu")!;
            var items = root.Items.OfType<XYMenuItem>().ToArray();
            Assert.Equal(["点", "线", "面"], items.Select(x => x.Label));
            AssertRootItemsAreVisibleAndVertical(items);
            foreach (var (index, leaf) in new[] { (0, "点标记"), (1, "道路"), (2, "区域") })
            {
                items[index].Activate(); Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
                Assert.True(popup.IsOpen);
                Assert.True(root.IsVisible);
                AssertRootItemsAreVisibleAndVertical(items);
                var childItems = toolbar.FindControl<XYMenu>("DrawChildMenu")!.Items.OfType<XYMenuItem>();
                Assert.Equal(leaf, childItems.Single().Label);
            }
        });
    }

    static void AssertRootItemsAreVisibleAndVertical(IReadOnlyList<XYMenuItem> items)
    {
        Assert.All(items, item =>
        {
            Assert.True(item.IsVisible);
            Assert.True(item.Bounds.Width > 0 && item.Bounds.Height > 0);
        });
        Assert.True(items[0].Bounds.Bottom <= items[1].Bounds.Top);
        Assert.True(items[1].Bounds.Bottom <= items[2].Bounds.Top);
        Assert.NotEqual(items[0].Bounds, items[1].Bounds);
        Assert.NotEqual(items[1].Bounds, items[2].Bounds);
    }
}
