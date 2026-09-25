using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticFix13PopupTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticFix13PopupTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Popup_menu_item_updates_target_and_keeps_action_executed()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = NewVm(); vm.RunCommand.Execute("诊断模式");
            var toolbar = new ContextToolBar { DataContext = vm };
            var diagnostic = new DiagnosticOverlayHost { DataContext = vm };
            var window = host.Show(new Grid { Children = { toolbar, diagnostic } }, 900, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            var executed = 0; board.ActionExecuted += (_, _) => executed++;
            board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!);
            board.SubMenus[0].Open(); Dispatcher.UIThread.RunJobs();
            var item = board.SubMenus[0].ChildMenu.Items.OfType<XYMenuItem>().Single();
            diagnostic.ProbeHover(item, false); diagnostic.ProbeClick().GetAwaiter().GetResult();
            item.Activate(); Dispatcher.UIThread.RunJobs();
            Assert.Same(item, diagnostic.LockedProbeResult?.DeepVisual);
            Assert.Equal(1, executed); window.Close();
        });
    }

    static UiVm NewVm() => new(new HeadlessBridgeFactory(), () => true, seedInitialScene: false);
    sealed class HeadlessBridgeFactory : INativeHostSurfaceBridgeFactory
    { public INativeHostSurfaceBridge Create(Action<string>? log = null, IRenderProjectionSource? source = null) => new HeadlessBridge(); }
    sealed class HeadlessBridge : INativeHostSurfaceBridge
    { public bool Attach(NativeHostSurfaceHandle handle) => false; public void Resize(int w, int h) { } public void Detach() { } public void Dispose() { } }
}
