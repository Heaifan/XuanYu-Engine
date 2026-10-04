using XuanYu.Editor.Drawing;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Drawing;

public sealed class MarkerDrawingAdapterTests
{
    [Fact]
    public void Marker_controller_commits_once_through_map_edit_session()
    {
        var map = new MapEditSession();
        var controller = new MarkerDrawingController(map);

        Assert.True(controller.Begin());
        var result = controller.Commit(new(7, 8));

        Assert.True(result.IsSuccess);
        Assert.Single(map.CurrentMap.Markers);
        Assert.Equal(new MapPoint(7, 8), map.CurrentMap.Markers[0].Position);
        Assert.False(controller.IsActive);
        Assert.False(controller.Commit(new(9, 10)).IsSuccess);
        Assert.Single(map.CurrentMap.Markers);
    }

    [Fact]
    public void Cancel_does_not_create_marker_and_clears_preview()
    {
        var map = new MapEditSession();
        var controller = new MarkerDrawingController(map);

        Assert.True(controller.Begin());
        controller.UpdatePreview(new(2, 3), null);
        controller.Cancel();

        Assert.Empty(map.CurrentMap.Markers);
        Assert.False(controller.IsActive);
        Assert.Null(controller.Preview);
    }
}
