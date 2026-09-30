using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class ContextToolbarPopupHostCompositionTests
{
    [Fact]
    public void Engine_context_toolbar_uses_XYContextDropdownBoard()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() => { var toolbar = new ContextToolBar { DataContext = NewVm() }; host.Show(toolbar); Assert.Single(UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar)); Assert.Null(toolbar.FindControl<Popup>("DrawMenuPopup")); Assert.Null(toolbar.FindControl<Popup>("DrawSubMenuPopup")); });
    }

    [Fact]
    public void Engine_context_board_contains_only_supported_actions()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() => { var toolbar = new ContextToolBar { DataContext = NewVm() }; host.Show(toolbar); var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single(); Assert.Equal(["点标记", "道路", "区域"], board.SubMenus.SelectMany(x => x.ChildMenu.Items.OfType<XYMenuItem>()).Select(x => x.Label)); });
    }

    [Fact]
    public void Engine_context_board_uses_native_popup_surface()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var toolbar = new ContextToolBar { DataContext = NewVm() };
            var window = host.Show(toolbar, 420, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!);
            Dispatcher.UIThread.RunJobs();
            Assert.True(board.Popup.IsOpen);
            Assert.Contains(board.RootMenuSurface, board.Popup.Child!.GetVisualDescendants());
            window.Close();
        });
    }
}
