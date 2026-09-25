using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class ContextToolbarPopupHostRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public ContextToolbarPopupHostRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

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

    [Fact]
    public void Engine_window_has_native_context_popup_host()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() => { var toolbar = new ContextToolBar { DataContext = NewVm() }; var window = host.Show(toolbar); var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single(); board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!); Dispatcher.UIThread.RunJobs(); Assert.True(board.Popup.IsOpen); Assert.False(board.Popup.ShouldUseOverlayLayer); Assert.Contains(board.RootMenuSurface, board.Popup.Child!.GetVisualDescendants()); window.Close(); });
    }

    [Fact]
    public void Context_board_remains_inside_engine_window()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() => { var toolbar = new ContextToolBar { DataContext = NewVm() }; var window = host.Show(toolbar, 420, 220); var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single(); var split = toolbar.FindControl<XYSplitButton>("DrawSplitButton")!; split.MenuCommand!.Execute(null); board.SubMenus[2].Open(); Dispatcher.UIThread.RunJobs(); var surface = (Canvas)board.Popup.Child!; var owner = new Rect(surface.Bounds.Size); AssertInside(board.RootMenuSurface.Bounds, owner); Assert.All(board.ChildMenuSurfaces, x => AssertInside(x.Bounds, owner)); window.Close(); });
    }

    [Fact]
    public void Diagnostic_probe_resolves_open_context_menu_surface()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = NewVm(); vm.RunCommand.Execute("诊断模式");
            var toolbar = new ContextToolBar { DataContext = vm };
            var diagnostic = new DiagnosticOverlayHost { DataContext = vm };
            var window = host.Show(new Grid { Children = { toolbar, diagnostic } }, 900, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            var split = toolbar.FindControl<XYSplitButton>("DrawSplitButton")!;
            split.MenuCommand!.Execute(null); board.SubMenus[0].Open(); Dispatcher.UIThread.RunJobs();
            diagnostic.ProbeHover(board.ChildMenuSurfaces.First(), false); Dispatcher.UIThread.RunJobs();
            Assert.NotNull(diagnostic.CurrentProbeResult); Assert.Equal(1, diagnostic.ActiveProbeHighlightCount);
            window.Close();
        });
    }

    [Fact]
    public void Diagnostic_region_bounds_does_not_open_overlay_popup_inside_native_menu()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = NewVm(); vm.RunCommand.Execute("诊断模式"); vm.RunCommand.Execute("区域边界");
            var toolbar = new ContextToolBar { DataContext = vm };
            var diagnostic = new DiagnosticOverlayHost { DataContext = vm };
            var window = host.Show(new Grid { Children = { toolbar, diagnostic } }, 900, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            toolbar.FindControl<XYSplitButton>("DrawSplitButton")!.MenuCommand!.Execute(null);
            board.SubMenus[0].Open(); Dispatcher.UIThread.RunJobs();
            Assert.True(board.Popup.IsOpen);
            Assert.All(UiRuntimeTestHost.Descendants<Popup>(diagnostic), popup =>
                Assert.Same(window, TopLevel.GetTopLevel(popup.PlacementTarget)));
            window.Close();
        });
    }

    [Fact]
    public async Task Split_label_is_not_clipped_for_three_character_action()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var vm = NewVm(); Assert.True(await vm.BeginContextDrawingAsync("地图标记"));
        host.Run(() => { var toolbar = new ContextToolBar { DataContext = vm }; var window = host.Show(toolbar, 900, 220); var split = toolbar.FindControl<XYSplitButton>("DrawSplitButton")!; Dispatcher.UIThread.RunJobs(); Assert.Equal("点标记", split.Content); Assert.Equal(112, split.Width, 0.5); Assert.True(split.Bounds.Width >= 112); window.Close(); });
    }

    static void AssertInside(Rect inner, Rect owner) { Assert.True(inner.Left >= owner.Left - 0.5); Assert.True(inner.Top >= owner.Top - 0.5); Assert.True(inner.Right <= owner.Right + 0.5); Assert.True(inner.Bottom <= owner.Bottom + 0.5); }
    static UiVm NewVm() { var vm = new UiVm(new HeadlessBridgeFactory(), () => true, seedInitialScene: false); vm.ToggleEditorMode(); return vm; }
    sealed class HeadlessBridgeFactory : INativeHostSurfaceBridgeFactory { public INativeHostSurfaceBridge Create(Action<string>? log = null, IRenderProjectionSource? source = null) => new HeadlessBridge(); }
    sealed class HeadlessBridge : INativeHostSurfaceBridge { public bool Attach(NativeHostSurfaceHandle handle) => false; public void Resize(int width, int height) { } public void Detach() { } public void Dispose() { } }
}
