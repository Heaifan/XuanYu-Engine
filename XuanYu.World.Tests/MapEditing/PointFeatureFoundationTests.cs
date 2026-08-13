using System.Text.Json;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class PointFeatureFoundationTests
{
    [Fact]
    public void Marker_is_a_point_with_one_vertex_and_no_segments()
    {
        var session = new MapEditSession();
        var marker = new MapMarker(MapMarkerId.New(), session.ActiveRegionLayerId, "标记", new(10, 20));
        Assert.True(session.CreateMarker(marker).IsSuccess);
        Assert.True(GeometryFeatureAdapters.TryGet(session.CurrentMap,
            new(GeometryFeatureKind.Marker, marker.MarkerId.ToString()), out var adapter));
        Assert.Equal(GeometryKind.Point, adapter.Kind);
        Assert.Single(adapter.Points);
        Assert.Equal(0, adapter.SegmentCount);
        Assert.True(adapter.Capabilities.HasFlag(GeometryCapabilities.SnapTarget));
    }

    [Fact]
    public void Marker_edit_undo_redo_preserves_identity_and_local_query()
    {
        var session = new MapEditSession();
        var marker = new MapMarker(MapMarkerId.New(), session.ActiveRegionLayerId, "标记", new(10, 20));
        Assert.True(session.CreateMarker(marker).IsSuccess);
        Assert.True(session.EditMarkerPosition(marker.MarkerId, new(30, 40)).IsSuccess);
        Assert.Equal(new MapPoint(30, 40), session.CurrentMap.Markers.Single().Position);
        Assert.Contains(new GeometryFeatureKey(GeometryFeatureKind.Marker, marker.MarkerId.ToString()),
            session.QueryLocalGeometry(new(29, 39, 31, 41)));
        Assert.True(session.Undo().IsSuccess);
        Assert.Equal(new MapPoint(10, 20), session.CurrentMap.Markers.Single().Position);
        Assert.True(session.Redo().IsSuccess);
        Assert.Equal(marker.MarkerId, session.CurrentMap.Markers.Single().MarkerId);
    }

    [Fact]
    public void Marker_codec_round_trip_is_dataset_backed()
    {
        var marker = new MapMarker(MapMarkerId.New(), MapLayerId.New(), "标记", new(1.5, -2.5));
        var raw = MapMarkerDatasetCodec.Write(marker);
        var read = MapMarkerDatasetCodec.Read(raw);
        Assert.True(read.Succeeded);
        Assert.Equal(marker.MarkerId, read.Value!.MarkerId);
        Assert.Equal(marker.Position, read.Value.Position);
        var document = new MapDatasetDocument(MapDatasetDocument.CurrentFormat, MapDatasetDocument.CurrentVersion,
            "marker-a", MapDatasetTypes.Marker, [raw]);
        Assert.True(MapDatasetDocumentValidator.Validate(document).Succeeded);
    }

    [Fact]
    public void Point_policy_excludes_self_but_allows_marker_targets()
    {
        var source = new GeometryFeatureKey(GeometryFeatureKind.Marker, "a");
        Assert.False(GeometrySnapPolicy.CanTarget(source, source));
        Assert.True(GeometrySnapPolicy.CanTarget(source, new(GeometryFeatureKind.Region, "r")));
        Assert.True(GeometrySnapPolicy.CanTarget(source, new(GeometryFeatureKind.Road, "d")));
        Assert.True(GeometrySnapPolicy.CanTarget(source, new(GeometryFeatureKind.Marker, "b")));
    }
}
