using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;

namespace XuanYu.Core.Tests.Render;

public sealed class TerrainVisibilitySelectorTests
{
    [Fact] public void VisibleChunkAccepted() => AssertVisible(Box(0, 0, 10));
    [Fact] public void OutsideChunkCulled() => Assert.True(Cull(Box(100, 0, 10)));
    [Fact] public void BehindCameraChunkCulled() => Assert.True(Cull(Box(0, 0, -10)));
    [Fact] public void PerspectiveCulling() => Assert.False(Cull(Box(0, 0, 10)));
    [Fact] public void OrthographicCulling() => Assert.True(Cull(Box(30, 0, 10, ProjectionMode.Orthographic)));

    [Fact] public void NearChunkUsesFineLod() => Assert.Equal(0, Lod(Box(0, 0, 8)));
    [Fact] public void FarChunkUsesCoarseLod() => Assert.True(Lod(Box(0, 0, 1000)) >= 2);
    [Fact] public void ExtremeFarUsesLod4() => Assert.Equal(4, Lod(Box(0, 0, 100_000)));
    [Fact] public void LodRangeAlways0To4() => Assert.InRange(Lod(Box(0, 0, 100)), 0, 4);
    [Fact] public void ProjectedScaleDrivesLod() => Assert.True(Lod(Box(0, 0, 20)) < Lod(Box(0, 0, 200)));
    [Fact] public void PerspectiveLodMonotonic() => AssertMonotonic(ProjectionMode.Perspective);
    [Fact] public void OrthographicLodMonotonic() => AssertMonotonic(ProjectionMode.Orthographic);

    [Fact] public void LodBoundaryDoesNotThrash()
    {
        var state = State(ProjectionMode.Perspective);
        var box = Box(0, 0, 70);
        var first = TerrainLodSelector.Select(state, box);
        var next = TerrainLodSelector.Select(state, box, first.Lod);
        Assert.Equal(first.Lod, next.Lod);
    }

    [Fact]
    public void LodStateIsScopedToTerrainRevisionAndChunk()
    {
        var cache = new TerrainLodStateCache();
        var state = State(ProjectionMode.Perspective);
        var key = new TerrainLodStateKey("terrain", 1, 0, 0);
        var first = cache.Select(key, state, Box(0, 0, 70));
        var otherChunk = cache.Select(key with { ChunkX = 1 }, state, Box(0, 0, 70));
        var otherRevision = cache.Select(key with { Revision = 2 }, state, Box(0, 0, 70));

        Assert.Equal(first.Lod, otherChunk.Lod);
        Assert.Equal(first.Lod, otherRevision.Lod);
        Assert.Equal(3, cache.Count);
    }

    [Fact]
    public void LodStateRetention_drops_old_revision_without_inheriting_it()
    {
        var cache = new TerrainLodStateCache();
        var state = State(ProjectionMode.Perspective);
        cache.Select(new("terrain", 1, 0, 0), state, Box(0, 0, 70));
        cache.Select(new("terrain", 2, 0, 0), state, Box(0, 0, 70));

        cache.RetainOnly([("terrain", 2)]);

        Assert.Equal(1, cache.Count);
    }

    [Fact] public void VisibilityStatsCorrect()
    {
        var result = TerrainChunkVisibility.Select(
            [new(1, Box(0, 0, 10)), new(2, Box(100, 0, 10)), new(3, Box(0, 0, 1000))],
            State(ProjectionMode.Perspective));
        Assert.Equal(2, result.Stats.VisibleChunkCount);
        Assert.Equal(1, result.Stats.CulledChunkCount);
        Assert.Equal(1, result.Stats.Lod0Count);
    }

    [Fact] public void ReverseZFrustumRegression() => Assert.False(Cull(Box(0, 0, 99)));

    static void AssertVisible(SpatialAabb box) => Assert.False(Cull(box));
    static bool Cull(SpatialAabb box, ProjectionMode mode = ProjectionMode.Perspective) =>
        !TerrainFrustumCuller.Intersects(State(mode), box);
    static int Lod(SpatialAabb box) => TerrainLodSelector.Select(State(), box).Lod;
    static void AssertMonotonic(ProjectionMode mode)
    {
        var distances = new[] { 10.0, 30, 100, 300, 1000 };
        var lods = distances.Select(d => TerrainLodSelector.Select(State(mode), Box(0, 0, d)).Lod).ToArray();
        Assert.True(lods.Zip(lods.Skip(1)).All(pair => pair.First <= pair.Second));
    }

    static ViewProjectionState State(ProjectionMode mode = ProjectionMode.Perspective) =>
        ViewProjectionState.Create(new CameraState(Vector3d.Zero, Vector3d.UnitZ, Vector3d.UnitY, 60, 1, 1000, 0,
            mode, mode == ProjectionMode.Orthographic ? 20 : 0), new ViewportState(0, 0, 800, 600, 800, 600, 1, 0));

    static SpatialAabb Box(double x, double y, double z, ProjectionMode mode = ProjectionMode.Perspective) =>
        new(new Vector3d(x - 5, y - 5, z - 5), new Vector3d(x + 5, y + 5, z + 5));
}
