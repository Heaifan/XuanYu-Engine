using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticTargetBoundsRuntimeTests : IDisposable
{
    readonly UiHeadlessFixture _fixture;

    public DiagnosticTargetBoundsRuntimeTests(UiHeadlessFixture fixture)
    { _fixture = fixture; DiagnosticRegistry.Clear(); }

    public void Dispose() => DiagnosticRegistry.Clear();

    [Fact]
    public void Context_toolbar_snapshot_uses_visible_nonzero_target_bounds()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(new HeadlessBridgeFactory(), () => true, seedInitialScene: false);
            vm.ToggleEditorModeCommand.Execute(null);
            var target = new ContextToolBar { DataContext = vm };
            XYDiagnostic.SetDebugId(target, FeatureDiagnosticIds.ContextToolbar);
            DiagnosticRegistry.Register(target);
            var window = new Window { Width = 900, Height = 120, Content = target };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();

            var snapshot = DiagnosticSnapshotFactory.Capture(target);
            Assert.True(snapshot.Visible);
            Assert.True(snapshot.Bounds is { Width: > 0, Height: > 0 },
                $"Bounds={snapshot.Bounds}");
            window.Close();
        });
    }

    [Fact]
    public void Registered_context_toolbar_in_editor_has_nonzero_bounds()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(new HeadlessBridgeFactory(), () => true, seedInitialScene: false);
            vm.SwitchWorkspaceCommand.Execute("RegionEditor");
            vm.ToggleEditorModeCommand.Execute(null);
            var root = new UiRoot { DataContext = vm };
            var window = new Window { Width = 1200, Height = 800, Content = root };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            var target = root.GetVisualDescendants().OfType<ContextToolBar>().Single();
            var snapshot = DiagnosticSnapshotFactory.Capture(target);
            Assert.True(snapshot.Bounds is { Width: > 0, Height: > 0 },
                $"Bounds={snapshot.Bounds}, Visible={snapshot.Visible}");
            window.Close();
        });
    }

    sealed class HeadlessBridgeFactory : INativeHostSurfaceBridgeFactory
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
