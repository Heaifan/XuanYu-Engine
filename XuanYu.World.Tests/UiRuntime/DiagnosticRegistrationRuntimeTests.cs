using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Threading;
using Xunit;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticRegistrationRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticRegistrationRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Shown_editor_exposes_frozen_area_and_module_ids()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var root = new UiRoot { DataContext = NewVm().Vm };
            host.Show(root, 1200, 800);
            Dispatcher.UIThread.RunJobs();
            var ids = root.GetVisualDescendants().OfType<Control>()
                .Select(XYDiagnostic.GetDebugId).Where(x => x is not null).ToHashSet();
            Assert.All(FeatureDiagnosticIds.ModuleIds, id => Assert.Contains(id, ids));
            Assert.Contains(FeatureDiagnosticIds.Menu, ids);
            Assert.DoesNotContain(ids, id => id!.EndsWith(".OTHER", StringComparison.Ordinal));
            var areas = root.GetVisualDescendants().OfType<Control>()
                .Select(XYDiagnostic.GetAreaId).Where(x => x is not null).ToHashSet();
            Assert.Equal(5, areas.Count);
            Assert.Contains("XYE.AREA.TOP", areas); Assert.Contains("XYE.AREA.LEFT", areas);
            Assert.Contains("XYE.AREA.CENTER", areas); Assert.Contains("XYE.AREA.RIGHT", areas);
            Assert.Contains("XYE.AREA.BOTTOM", areas);
        });
    }

    [Theory]
    [InlineData(MapGeometryFeatureKind.Road, "XYE.INSPECTOR.ROAD")]
    [InlineData(MapGeometryFeatureKind.Region, "XYE.INSPECTOR.REGION")]
    public void Shared_feature_panel_resolves_module_and_four_sections(
        MapGeometryFeatureKind kind, string moduleId)
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var fixture = NewVm();
            fixture.Vm.SelectMapGeometry(kind == MapGeometryFeatureKind.Road ? fixture.Road : fixture.Region);
            var root = new UiRoot { DataContext = fixture.Vm };
            host.Show(root, 1200, 800);
            Dispatcher.UIThread.RunJobs();
            var panel = root.GetVisualDescendants().OfType<FeatureInspectorPanel>().Single();
            Assert.Equal(moduleId, XYDiagnostic.GetDebugId(panel));
            Assert.Equal($"{moduleId}.BASIC", DebugId(panel, "FeatureBasicSection"));
            Assert.Equal($"{moduleId}.GEOMETRY", DebugId(panel, "FeatureGeometrySection"));
            Assert.Equal($"{moduleId}.STATE", DebugId(panel, "FeatureStateSection"));
            Assert.Equal($"{moduleId}.RELATIONS", DebugId(panel, "FeatureRelationsSection"));
            Assert.Contains(moduleId, DiagnosticRegistry.Targets.Keys);
        });
    }

    static string? DebugId(Control root, string name) =>
        root.GetVisualDescendants().OfType<Control>().SingleOrDefault(x => x.Name == name) is { } target
            ? XYDiagnostic.GetDebugId(target) : null;

    static (UiVm Vm, MapGeometrySelection Road, MapGeometrySelection Region) NewVm()
    {
        var vm = new UiVm(new HeadlessBridgeFactory(), () => true, seedInitialScene: false);
        vm.ToggleFeatureEditingCommand.Execute(null);
        var layer = vm.MapSession.ActiveRegionLayerId;
        var road = new MapRoad(MapRoadId.New(), layer, "道路", "generic", [new(0, 0), new(1, 1)]);
        var region = new MapRegion(MapRegionId.New(), layer, "区域", MapRegionKind.Generic,
            [new(0, 0), new(1, 0), new(0, 1)]);
        Assert.True(vm.MapSession.CreateRoad(road).IsSuccess);
        Assert.True(vm.MapSession.CreateRegion(region).IsSuccess);
        return (vm, new(MapGeometryFeatureKind.Road, road.RoadId.ToString()),
            new(MapGeometryFeatureKind.Region, region.RegionId.ToString()));
    }

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
