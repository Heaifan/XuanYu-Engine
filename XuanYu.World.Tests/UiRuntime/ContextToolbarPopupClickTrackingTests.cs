using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class ContextToolbarPopupClickTrackingTests
{
    readonly UiHeadlessFixture _fixture;
    public ContextToolbarPopupClickTrackingTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Popup_menu_item_tracks_and_still_executes_action()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(new BridgeFactory(), () => true, seedInitialScene: false);
            vm.ToggleEditorMode(); vm.RunCommand.Execute("诊断模式");
            var toolbar = new ContextToolBar { DataContext = vm }; var diagnostic = new DiagnosticOverlayHost { DataContext = vm };
            var window = new Window { Width = 900, Height = 220, Content = new Grid { Children = { toolbar, diagnostic } } };
            window.Show(); window.UpdateLayout();
            var board = toolbar.GetVisualDescendants().OfType<XYContextDropdownBoard>().Single();
            board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!); Dispatcher.UIThread.RunJobs();
            var item = board.SubMenus[0].ChildMenu.GetVisualDescendants().OfType<XYMenuItem>().First(); var executed = false;
            board.ActionExecuted += (_, _) => executed = true; diagnostic.ProbeClick(item); item.Activate();
            Assert.Same(item, diagnostic.LockedProbeResult?.DeepVisual); Assert.True(executed); window.Close();
            Assert.Equal("XYE.CONTEXT_ACTION.MARKER", diagnostic.LockedProbeResult?.DebugId);
        });
    }

    sealed class BridgeFactory : INativeHostSurfaceBridgeFactory
    {
        public INativeHostSurfaceBridge Create(Action<string>? log = null, IRenderProjectionSource? source = null) => new Bridge();
    }
    sealed class Bridge : INativeHostSurfaceBridge
    {
        public bool Attach(NativeHostSurfaceHandle handle) => false;
        public void Resize(int width, int height) { }
        public void Detach() { }
        public void Dispose() { }
    }
}
