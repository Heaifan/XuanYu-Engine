using Avalonia.Controls;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class ContextToolbarPopupDiagnosticIdentityTests
{
    readonly UiHeadlessFixture _fixture;
    public ContextToolbarPopupDiagnosticIdentityTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Open_popup_registers_stable_diagnostic_identities()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var toolbar = new ContextToolBar { DataContext = NewVm() };
            var window = host.Show(toolbar, 420, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            board.Open(toolbar.FindControl<XYSplitButton>("DrawSplitButton")!);
            var expected = new[] { "XYE.CONTEXT_ACTION.MARKER", "XYE.CONTEXT_ACTION.ROAD", "XYE.CONTEXT_ACTION.REGION",
                "XYE.CONTEXT_MENU", "XYE.CONTEXT_MENU.AREA", "XYE.CONTEXT_MENU.LINE", "XYE.CONTEXT_MENU.POINT" };
            Assert.Equal(expected, DiagnosticRegistry.Targets.Keys.OrderBy(x => x));
            window.Close();
        });
    }

    [Fact]
    public void Closing_popup_unregisters_targets_and_reopening_does_not_duplicate()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var toolbar = new ContextToolBar { DataContext = NewVm() };
            var window = host.Show(toolbar, 420, 220);
            var board = UiRuntimeTestHost.Descendants<XYContextDropdownBoard>(toolbar).Single();
            var split = toolbar.FindControl<XYSplitButton>("DrawSplitButton")!;
            for (var i = 0; i < 10; i++) { board.Open(split); board.Close(); }
            Assert.DoesNotContain(DiagnosticRegistry.Targets.Keys, x => x.StartsWith("XYE.CONTEXT_", StringComparison.Ordinal));
            board.Open(split);
            Assert.Equal(7, DiagnosticRegistry.Targets.Count(x => x.Key.StartsWith("XYE.CONTEXT_", StringComparison.Ordinal)));
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
