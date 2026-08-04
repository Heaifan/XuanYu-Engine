using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

// MAP-A-R2-D3：MapDefinition → MapRenderSnapshot 投影合同（渲染唯一输入）。
public sealed class MapRenderSnapshotProjectionTests
{
    [Fact]
    public void Default_map_projects_full_snapshot()
    {
        var map = MapDefaultDefinition.CreateDefault();

        var snap = MapRenderSnapshotProjection.Project(map, changeSequence: 3);

        Assert.True(snap.HasMap);
        Assert.True(snap.IsVisible);
        Assert.Equal(map.MapId.Value, snap.MapId);
        Assert.Equal(10000.0, snap.WidthMeters);
        Assert.Equal(10000.0, snap.DepthMeters);
        Assert.Equal(Core.Map.MapSurfaceKind.Flat, snap.SurfaceKind);
        Assert.Equal(0.0, snap.BaseHeightMeters);
        Assert.Equal(3, snap.SourceChangeSequence);
        Assert.Equal(-5000.0, snap.MinX);
        Assert.Equal(5000.0, snap.MaxX);
        Assert.Equal(-5000.0, snap.MinY);
        Assert.Equal(5000.0, snap.MaxY);
    }

    [Fact]
    public void Resize_projects_new_bounds()
    {
        var map = MapDefaultDefinition.CreateDefault() with
        {
            SizeMeters = new MapSize(20000, 8000)
        };

        var snap = MapRenderSnapshotProjection.Project(map, 1);

        Assert.Equal(20000.0, snap.WidthMeters);
        Assert.Equal(8000.0, snap.DepthMeters);
        Assert.Equal(-10000.0, snap.MinX);
        Assert.Equal(-4000.0, snap.MinY);
    }

    [Fact]
    public void Base_height_and_surface_kind_projected()
    {
        var map = MapDefaultDefinition.CreateDefault() with
        {
            Surface = new MapSurfaceDefinition(MapSurfaceKinds.GentleHillsV1, 25, 12, 400, 1)
        };

        var snap = MapRenderSnapshotProjection.Project(map, 0);

        Assert.Equal(25.0, snap.BaseHeightMeters);
        Assert.Equal(Core.Map.MapSurfaceKind.GentleHillsV1, snap.SurfaceKind);
        Assert.Equal(12.0, snap.AmplitudeMeters);
        Assert.Equal(400.0, snap.WavelengthMeters);
    }

    [Fact]
    public void Session_resize_drives_snapshot_via_content_changed()
    {
        var session = new MapEditSession();
        MapRenderSnapshot? received = null;
        session.ContentChanged += e => received = MapRenderSnapshotProjection.Project(e.CurrentMap, e.ChangeSequence);
        Assert.Null(received);

        var result = session.ResizeMap(30000, 30000);

        Assert.True(result.IsSuccess);
        Assert.NotNull(received);
        Assert.Equal(30000.0, received.Value.WidthMeters);
        Assert.Equal(1, received.Value.SourceChangeSequence);
    }

    [Fact]
    public void Rename_does_not_change_geometry_fields()
    {
        var session = new MapEditSession();
        session.ResizeMap(30000, 30000); // 基线修改（ChangeSequence=1）
        var before = MapRenderSnapshotProjection.Project(session.CurrentMap, session.ChangeSequence);
        MapRenderSnapshot? after = null;
        session.ContentChanged += e => after = MapRenderSnapshotProjection.Project(e.CurrentMap, e.ChangeSequence);

        session.RenameMap("新名字");

        Assert.NotNull(after);
        Assert.Equal(before.WidthMeters, after.Value.WidthMeters);
        Assert.Equal(before.BaseHeightMeters, after.Value.BaseHeightMeters);
        Assert.Equal(before.MapId, after.Value.MapId);
        Assert.NotEqual(before.SourceChangeSequence, after.Value.SourceChangeSequence);
    }
}
