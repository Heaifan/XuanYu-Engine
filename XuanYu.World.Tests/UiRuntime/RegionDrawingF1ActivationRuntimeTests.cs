using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class RegionDrawingF1ActivationRuntimeTests : IDisposable
{
    readonly UiHeadlessFixture _fixture;
    readonly string _root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-f1-{Guid.NewGuid():N}");

    public RegionDrawingF1ActivationRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Real_top_click_enters_region_drawing_and_creates_draft_vertex()
    {
        Directory.CreateDirectory(_root);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var path = Path.Combine(_root, "map.json");
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.DatasetCreateType = "region";
        Assert.True(await vm.CreateDatasetAsync());
        var id = vm.DatasetSelectedId!;
        vm.ToggleEditorMode();
        vm.SelectDataset(id);
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        Assert.True(vm.IsRegionEditMode);
        Assert.True(vm.CanStartRegionDrawing);
        Assert.Equal("选择", vm.ActiveTool);

        using var host = new UiRuntimeTestHost(_fixture);
        Top top = null!;
        var enabled = host.Run(() =>
        {
            top = new Top { DataContext = vm };
            host.Show(top, 1200, 180);
            var button = UiRuntimeTestHost.Descendants<ToggleButton>(top).Single(item =>
                UiRuntimeTestHost.Descendants<TextBlock>(item).Any(text => text.Text == "绘制区域"));
            var state = button.IsVisible && button.IsEnabled;
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            return state;
        });

        Assert.True(enabled);
        for (var i = 0; i < 20 && !host.Run(() => vm.IsRegionDrawingTool); i++) await Task.Delay(25);
        var result = host.Run(() =>
        {
            top.UpdateLayout();
            var button = UiRuntimeTestHost.Descendants<ToggleButton>(top).Single(item =>
                UiRuntimeTestHost.Descendants<TextBlock>(item).Any(text => text.Text == "绘制区域"));
            var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
            var hit = FindHit(vm, viewport);
            var handled = vm.RegionDrawingPointerPressed(hit.X, hit.Y, viewport);
            return (vm.IsRegionDrawingTool, button.IsChecked, handled, vm.RegionDrawingDraftVertexCount);
        });

        Assert.True(result.IsRegionDrawingTool);
        Assert.True(result.IsChecked);
        Assert.True(result.handled);
        Assert.Equal(1, result.RegionDrawingDraftVertexCount);
    }

    static (double X, double Y) FindHit(UiVm vm, ViewportState viewport)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
        return Enumerable.Range(0, 17).SelectMany(x => Enumerable.Range(0, 13)
            .Select(y => (X: x * 50.0, Y: y * 50.0)))
            .First(point => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection,
                point.X, point.Y, out _));
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_root)) Directory.Delete(_root, true); }
        catch (IOException) { }
    }
}
