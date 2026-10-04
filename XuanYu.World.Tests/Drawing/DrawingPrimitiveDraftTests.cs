using System.Reflection;
using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Drawing;

public sealed class DrawingPrimitiveDraftTests
{
    [Fact]
    public void PointDraft_accepts_zero_or_one_authoring_point()
    {
        var draft = new PointDraft();
        Assert.Equal(0, draft.PointCount);
        draft.Add(new MapPoint(1, 2));
        Assert.Equal(1, draft.PointCount);
        var error = Record.Exception(() => draft.Add(new MapPoint(3, 4)));
        Assert.IsType<InvalidOperationException>(error);
        draft.Clear();
        Assert.Empty(draft.Points);
    }

    [Fact]
    public void PolylineDraft_preserves_control_point_order()
    {
        var points = new[] { new MapPoint(1, 2), new MapPoint(3, 4), new MapPoint(5, 6) };
        var draft = new PolylineDraft();
        foreach (var point in points) draft.AddPoint(point);
        Assert.Equal(points, draft.Points);
    }

    [Fact]
    public void PolygonDraft_preserves_order_without_closure_duplicate()
    {
        var points = new[] { new MapPoint(1, 2), new MapPoint(3, 4), new MapPoint(5, 6) };
        var draft = new PolygonDraft();
        foreach (var point in points) draft.AddPoint(point);
        Assert.Equal(points, draft.Points);
        Assert.NotEqual(draft.Points[0], draft.Points[^1]);
    }

    [Fact]
    public void RemoveLast_changes_only_authoring_points()
    {
        var draft = new PolylineDraft();
        draft.AddPoint(new MapPoint(1, 2));
        draft.AddPoint(new MapPoint(3, 4));
        draft.RemoveLast();
        Assert.Equal(new[] { new MapPoint(1, 2) }, draft.Points);
    }

    [Fact]
    public void PointCount_always_matches_authoring_control_point_count()
    {
        var point = new PointDraft();
        point.Add(new MapPoint(1, 2));
        Assert.Equal(point.Points.Count, point.PointCount);
        var polyline = new PolylineDraft();
        polyline.AddPoint(new MapPoint(1, 2));
        polyline.RemoveLast();
        Assert.Equal(polyline.Points.Count, polyline.PointCount);
        var polygon = new PolygonDraft();
        polygon.AddPoint(new MapPoint(1, 2));
        Assert.Equal(polygon.Points.Count, polygon.PointCount);
    }

    [Fact]
    public void Drafts_expose_no_terrain_render_or_business_metadata()
    {
        var forbidden = new[] { "Terrain", "Draped", "Render", "Sample", "Region", "Road", "Marker", "Province", "Territory" };
        var types = new[] { typeof(PointDraft), typeof(PolylineDraft), typeof(PolygonDraft) };
        foreach (var type in types)
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            Assert.DoesNotContain(forbidden, token => property.Name.Contains(token, StringComparison.Ordinal));
    }

    static int GetPointCount(object draft) => (int)draft.GetType().GetProperty("PointCount")!.GetValue(draft)!;
}
