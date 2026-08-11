using Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DatasetLayerPanelRuntimeLayoutTests : IDisposable
{
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-dataset-f3-{Guid.NewGuid():N}");
    readonly UiHeadlessFixture _fixture;

    public DatasetLayerPanelRuntimeLayoutTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Dataset_and_layer_rows_keep_specified_bounds_at_300_dip()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(Path.Combine(_root, "map.json")));
        Assert.True(await vm.CreateDatasetAsync());
        using var host = new UiRuntimeTestHost(_fixture);
        var bounds = host.Run(() =>
        {
            var dataset = new DatasetPanel { DataContext = vm };
            var layer = new DatasetLayerPanel { DataContext = vm };
            var root = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*") };
            root.Children.Add(dataset); Grid.SetColumn(layer, 1); root.Children.Add(layer);
            host.Show(root, 600, 420); root.UpdateLayout();
            var list = dataset.FindControl<ListBox>("DatasetList")!;
            var datasetRow = list.ContainerFromIndex(0)!;
            var layerList = layer.FindControl<ListBox>("DatasetLayerList")!;
            var layerRow = layerList.ContainerFromIndex(0)!;
            return (datasetRow.Bounds, list.Bounds, layerRow.Bounds, layerList.Bounds);
        });
        Assert.Equal(28, bounds.Item1.Height);
        Assert.Equal(32, bounds.Item3.Height);
        Assert.True(bounds.Item1.Width >= bounds.Item2.Width - 1);
        Assert.True(bounds.Item3.Width >= bounds.Item4.Width - 1);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}
