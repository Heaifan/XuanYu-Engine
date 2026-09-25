using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class ContextToolbarNativeDismissRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public ContextToolbarNativeDismissRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Native_viewport_owner_pointer_down_dismisses_popup()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var toolbar = new ContextToolBar { DataContext = NewVm() }; var window = host.Show(toolbar, 420, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!); Dispatcher.UIThread.RunJobs();
            XYContextDropdownBoard.NotifyOwnerPointerDown();
            Assert.False(board.Popup.IsOpen); window.Close();
        });
    }

    [Fact]
    public void Native_viewport_owner_pointer_down_dismisses_open_area_child()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var toolbar = new ContextToolBar { DataContext = NewVm() }; var window = host.Show(toolbar, 420, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!); board.SubMenus[2].Open(); Dispatcher.UIThread.RunJobs();
            XYContextDropdownBoard.NotifyOwnerPointerDown();
            Assert.False(board.IsOpen); Assert.All(board.ChildMenuSurfaces, surface => Assert.False(surface.IsVisible)); window.Close();
        });
    }

    [Fact]
    public void Native_viewport_owner_pointer_down_has_no_stale_popup_after_ten_reopens()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var toolbar = new ContextToolBar { DataContext = NewVm() }; var window = host.Show(toolbar, 420, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single(); var split = toolbar.FindControl<XYSplitButton>("DrawSplitButton")!;
            for (var i = 0; i < 10; i++) { board.Open(split); Assert.True(board.Popup.IsOpen); XYContextDropdownBoard.NotifyOwnerPointerDown(); Assert.False(board.Popup.IsOpen); }
            window.Close();
        });
    }

    [Fact]
    public void Project_tree_pointer_down_dismisses_popup()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() => AssertExternalDismiss(new ProjectWorkspace()));
    }

    [Fact]
    public void Inspector_pointer_down_dismisses_popup()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() => AssertExternalDismiss(new InspectorPanel()));
    }

    void AssertExternalDismiss(Control external)
    {
        var vm = NewVm(); var toolbar = new ContextToolBar { DataContext = vm }; external.DataContext = vm;
        var window = new Window { Width = 900, Height = 400, Content = new Grid { Children = { toolbar, external } } };
        window.Show(); window.UpdateLayout();
        var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
        board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!); Dispatcher.UIThread.RunJobs();
        external.RaiseEvent(new PointerPressedEventArgs(external, null!, window, new Point(), 0,
            new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonPressed), KeyModifiers.None));
        Assert.False(board.Popup.IsOpen); window.Close();
    }

    static UiVm NewVm() { var vm = new UiVm(new HeadlessBridgeFactory(), () => true, seedInitialScene: false); vm.ToggleEditorMode(); return vm; }
    sealed class HeadlessBridgeFactory : INativeHostSurfaceBridgeFactory { public INativeHostSurfaceBridge Create(Action<string>? log = null, IRenderProjectionSource? source = null) => new HeadlessBridge(); }
    sealed class HeadlessBridge : INativeHostSurfaceBridge { public bool Attach(NativeHostSurfaceHandle handle) => false; public void Resize(int width, int height) { } public void Detach() { } public void Dispose() { } }
}
