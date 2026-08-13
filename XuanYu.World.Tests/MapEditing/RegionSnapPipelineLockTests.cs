using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionSnapPipelineLockTests
{
    [Fact]
    public void Edge_lock_reprojects_along_the_same_segment()
    {
        var (map, source, _, projection) = RegionSnapPipelineTestFixture.Create(); var state = new RegionSnapState();
        var first = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(new(40, 0)));
        RegionSnapPipelineTestFixture.Resolve(map, source, new(first.X, first.Y + 5), projection, state);
        var second = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(new(60, 0)));
        var result = RegionSnapPipelineTestFixture.Resolve(map, source, new(second.X, second.Y + 5), projection, state);
        Assert.Equal(RegionSnapKind.Edge, result.Kind); Assert.Equal(0, result.TargetIndex);
        Assert.InRange(Math.Abs(result.ResolvedPoint.X - 60), 0, 0.001);
        Assert.InRange(Math.Abs(result.ResolvedPoint.Y), 0, 0.001);
    }

    [Fact]
    public void Edge_lock_holds_between_eight_and_twelve_pixels()
    {
        var (map, source, _, projection) = RegionSnapPipelineTestFixture.Create(); var state = new RegionSnapState();
        var edge = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(new(50, 0)));
        RegionSnapPipelineTestFixture.Resolve(map, source, new(edge.X, edge.Y + 5), projection, state);
        var result = RegionSnapPipelineTestFixture.Resolve(map, source, new(edge.X, edge.Y + 10), projection, state);
        Assert.Equal(RegionSnapKind.Edge, result.Kind); Assert.True(state.IsSnapped);
    }

    [Fact]
    public void Edge_lock_releases_beyond_twelve_pixels()
    {
        var (map, source, _, projection) = RegionSnapPipelineTestFixture.Create(); var state = new RegionSnapState();
        var edge = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(new(50, 0)));
        RegionSnapPipelineTestFixture.Resolve(map, source, new(edge.X, edge.Y + 5), projection, state);
        var result = RegionSnapPipelineTestFixture.Resolve(map, source, new(edge.X, edge.Y + 13), projection, state);
        Assert.Equal(RegionSnapKind.None, result.Kind); Assert.False(state.IsSnapped);
    }

    [Fact]
    public void Edge_lock_upgrades_to_vertex_at_endpoint()
    {
        var (map, source, target, projection) = RegionSnapPipelineTestFixture.Create(); var state = new RegionSnapState();
        var edge = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(new(50, 0)));
        RegionSnapPipelineTestFixture.Resolve(map, source, new(edge.X, edge.Y + 5), projection, state);
        var endpoint = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(target.Vertices[1]));
        var result = RegionSnapPipelineTestFixture.Resolve(map, source, endpoint, projection, state);
        Assert.Equal(RegionSnapKind.Vertex, result.Kind); Assert.Equal(1, result.TargetIndex);
    }

    [Fact]
    public void Vertex_lock_does_not_downgrade_to_edge_inside_release_radius()
    {
        var (map, source, target, projection) = RegionSnapPipelineTestFixture.Create(); var state = new RegionSnapState();
        var endpoint = projection.ProjectWorldPoint(RegionSnapPipelineTestFixture.World(target.Vertices[0]));
        RegionSnapPipelineTestFixture.Resolve(map, source, endpoint, projection, state);
        var result = RegionSnapPipelineTestFixture.Resolve(map, source, new(endpoint.X + 10, endpoint.Y), projection, state);
        Assert.Equal(RegionSnapKind.Vertex, result.Kind); Assert.Equal(0, result.TargetIndex);
    }
}
