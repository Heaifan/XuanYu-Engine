namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RoadDrawingSelectionF1Tests
{
    [Fact]
    public async Task Real_road_creation_uses_sequential_names()
    {
        var vm = await CompleteAsync();
        Assert.Equal("道路1", vm.MapSession.CurrentMap.Roads.Single().DisplayName);
        Assert.True(await vm.BeginRoadDrawingAsync());
        AddNodes(vm, new(-2, -2), new(2, 2));
        Assert.True(vm.CompleteRoadDrawing());

        Assert.Equal(["道路1", "道路2"], vm.MapSession.CurrentMap.Roads.Select(x => x.DisplayName));
    }
}
