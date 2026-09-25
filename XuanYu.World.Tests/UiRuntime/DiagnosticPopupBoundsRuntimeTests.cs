using Avalonia.Controls;
using Avalonia.Threading;
using System.Reflection;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticPopupBoundsRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticPopupBoundsRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Popup_targets_get_a_diagnostic_layer_and_highlight()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = NewVm(); vm.RunCommand.Execute("诊断模式");
            var diagnostic = new DiagnosticOverlayHost { DataContext = vm };
            var window = host.Show(new Grid { Children = { diagnostic } }, 900, 220);
            var target = new Button { Content = "Popup目标" };
            var popupRoot = new Window { Content = new Canvas { Children = { target } } };
            popupRoot.Show(); popupRoot.UpdateLayout();
            Dispatcher.UIThread.RunJobs();
            var getLayer = typeof(DiagnosticOverlayHost).GetMethod("GetProbeLayer", BindingFlags.Instance | BindingFlags.NonPublic)!;
            getLayer.Invoke(diagnostic, [popupRoot]);
            diagnostic.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            Assert.True(diagnostic.ActivePopupProbeLayerCount > 0);
            Assert.Equal(1, diagnostic.ActivePopupProbeHighlightCount);
            popupRoot.Close();
            window.Close();
        });
    }

    static UiVm NewVm() => new(new HeadlessBridgeFactory(), () => true, seedInitialScene: false);

    sealed class HeadlessBridgeFactory : INativeHostSurfaceBridgeFactory
    {
        public INativeHostSurfaceBridge Create(Action<string>? log = null, IRenderProjectionSource? source = null) => new HeadlessBridge();
    }
    sealed class HeadlessBridge : INativeHostSurfaceBridge
    {
        public bool Attach(NativeHostSurfaceHandle handle) => false;
        public void Resize(int width, int height) { }
        public void Detach() { }
        public void Dispose() { }
    }
}
