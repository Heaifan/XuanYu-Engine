using Avalonia.Threading;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticRegionSelectionRegressionTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticRegionSelectionRegressionTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Selecting_region_after_root_load_does_not_crash_diagnostic_refresh()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(new BridgeFactory(), () => true, seedInitialScene: false);
            vm.ToggleFeatureEditingCommand.Execute(null);
            var layer = vm.MapSession.ActiveRegionLayerId;
            var region = new MapRegion(MapRegionId.New(), layer, "区域1", MapRegionKind.Generic,
                [new(0, 0), new(1, 0), new(0, 1)]);
            Assert.True(vm.MapSession.CreateRegion(region).IsSuccess);
            var root = new UiRoot { DataContext = vm };
            host.Show(root, 1200, 800); Dispatcher.UIThread.RunJobs();

            var error = Record.Exception(() =>
            {
                vm.SelectMapGeometry(new(MapGeometryFeatureKind.Region, region.RegionId.ToString()));
                Dispatcher.UIThread.RunJobs();
            });

            Assert.Null(error);
            Assert.Contains("XYE.INSPECTOR.REGION", DiagnosticRegistry.Targets.Keys);
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
