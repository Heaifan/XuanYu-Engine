using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class TerrainInspectorRuntimeTests
{
    [Fact]
    public async Task Terrain_selection_projects_formal_state_into_inspector()
    {
        var vm = NewVm();
        await CreateTerrainDataset(vm);
        var elevations = new[] { 12d, 30d, 846d, -9999d, 100d, 200d };
        vm.SetTerrainInspectorSource(CreateSource(elevations));

        var fields = vm.InspectorProperties;

        Assert.True(vm.IsTerrainInspector);
        Assert.Contains(fields, row => row.DisplayName == "网格" && row.Value == "3 × 2");
        Assert.Contains(fields, row => row.DisplayName == "分辨率" && row.Value == "30 m");
        Assert.Contains(fields, row => row.DisplayName == "最低高程" && row.Value == "12 m");
        Assert.Contains(fields, row => row.DisplayName == "最高高程" && row.Value == "846 m");
        Assert.Contains(fields, row => row.DisplayName == "NoData" && row.Value == "-9999");
    }

    [Fact]
    public async Task Non_terrain_selection_does_not_show_terrain_inspector()
    {
        var vm = NewVm();
        await CreateTerrainDataset(vm);
        vm.SetTerrainInspectorSource(CreateSource([1, 2, 3, 4, 5, 6]));
        vm.DatasetSelectedId = null;

        Assert.False(vm.IsTerrainInspector);
        Assert.DoesNotContain(vm.InspectorProperties, row => row.DisplayName == "垂直夸张");
    }

    [Fact]
    public async Task Vertical_exaggeration_notifies_and_does_not_change_source_elevations()
    {
        var vm = NewVm();
        await CreateTerrainDataset(vm);
        var elevations = new[] { 12d, 30d, 846d, -9999d, 100d, 200d };
        vm.SetTerrainInspectorSource(CreateSource(elevations));
        Assert.Equal(1.0, vm.VerticalExaggeration);
        var notifications = new List<string?>();
        vm.PropertyChanged += (_, args) => notifications.Add(args.PropertyName);

        var target = vm.CreateInspectorEditTarget("Terrain.Display.VerticalExaggeration");
        Assert.True(vm.CommitInspectorProperty(target, "2.5"));

        Assert.Equal(2.5, vm.VerticalExaggeration);
        Assert.Contains(nameof(vm.VerticalExaggeration), notifications);
        Assert.Equal(new[] { 12d, 30d, 846d, -9999d, 100d, 200d }, elevations);
    }

    static UiVm NewVm() => new(null, () => true, seedInitialScene: false);

    static async Task CreateTerrainDataset(UiVm vm)
    {
        vm.DatasetCreateType = MapDatasetTypes.TerrainArea;
        Assert.True(await vm.CreateDatasetAsync());
    }

    static TerrainSourceData CreateSource(double[] elevations) =>
        new(new TerrainSourceRaster(3, 2, elevations, [false, false, false, true, false, false]),
            "EPSG:4326", new(0, 0, 90, 60), new(30, 30),
            new Dictionary<string, string> { ["nodata_value"] = "-9999" });
}
