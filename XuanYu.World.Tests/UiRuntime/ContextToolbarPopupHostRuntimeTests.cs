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
            var submenu = toolbar.FindControl<XYSubMenu>("DrawSubMenu")!;
            for (var i = 0; i < 10; i++)
            {
                split.MenuCommand!.Execute(null); Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
                root.Items.OfType<XYMenuItem>().Single(x => x.Label == (i % 2 == 0 ? "点" : "线")).Activate();
                Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
                Assert.NotNull(submenu.GetVisualParent());
                Assert.True(submenu.Bounds.Width > 0 && submenu.Bounds.Height > 0);
                Assert.NotNull(submenu.ChildMenu.GetVisualParent());
                Assert.True(submenu.ChildMenu.Bounds.Width > 0 && submenu.ChildMenu.Bounds.Height > 0);
                toolbar.FindControl<Popup>("DrawMenuPopup")!.IsOpen = false;
                Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
                Assert.False(toolbar.FindControl<Popup>("DrawMenuPopup")!.IsOpen);
            }
        });
    }
}
