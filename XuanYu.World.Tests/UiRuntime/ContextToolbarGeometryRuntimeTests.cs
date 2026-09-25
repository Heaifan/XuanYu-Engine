using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class ContextToolbarGeometryRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public ContextToolbarGeometryRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Root_board_geometry_does_not_include_submenu_width()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.ToggleEditorMode();
            var toolbar = new ContextToolBar { DataContext = vm };
            var window = host.Show(toolbar, 1000, 180);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            toolbar.FindControl<XYSplitButton>("DrawSplitButton")!.MenuCommand!.Execute(null);
            Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout(); var rootWidth = board.RootMenuSurface.Bounds.Width; var rootX = board.RootMenuSurface.Bounds.X;
            board.SubMenus[0].Open();
            Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout();
            Assert.Equal(rootWidth, board.RootMenuSurface.Bounds.Width, 1); Assert.Equal(rootX, board.RootMenuSurface.Bounds.X, 1);
            Assert.True(board.ChildMenuSurfaces[0].IsVisible); Assert.True(board.ChildMenuSurfaces[0].Bounds.Width <= 128);
        });
    }

    [Fact]
    public void Context_board_reopen_preserves_window_bounds()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var toolbar = new ContextToolBar { DataContext = new UiVm(null, seedInitialScene: false) }; var window = host.Show(toolbar, 1000, 180);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single(); var split = toolbar.FindControl<XYSplitButton>("DrawSplitButton")!;
            split.MenuCommand!.Execute(null); Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout(); var first = board.RootMenuSurface.Bounds;
            board.Close(); split.MenuCommand.Execute(null); Dispatcher.UIThread.RunJobs(); toolbar.UpdateLayout(); var second = board.RootMenuSurface.Bounds;
            Assert.Equal(first.X, second.X, 1); Assert.Equal(first.Y, second.Y, 1); Assert.True(board.Popup.IsOpen); Assert.False(board.Popup.ShouldUseOverlayLayer);
        });
    }
}
