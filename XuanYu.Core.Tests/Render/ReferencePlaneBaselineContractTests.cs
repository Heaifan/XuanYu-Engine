using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

public sealed class ReferencePlaneBaselineContractTests
{
    static readonly ViewportState Viewport = new(0, 0, 1920, 1080, 1920, 1080, 1, 1);

    [Fact]
    public void Terrain_presence_does_not_own_reference_plane()
    {
        var empty = EmptyProjection();
        var withTerrain = empty with
        {
            Terrains = [new TerrainRenderResource("terrain", 1,
                new TerrainHeightfield(2, 2, [0.0, 0.0, 0.0, 0.0]))]
        };

        Assert.True(empty.HasReferencePlane);
        Assert.True(withTerrain.HasReferencePlane);
    }

    [Fact]
    public void Reference_plane_does_not_change_when_render_origin_changes()
    {
        var before = EmptyProjection();
        var after = before with
        {
            Camera = new RenderCameraProjection(new(10000, 0, 10000),
                new(0, 0, -1), Vector3d.UnitY, 60, 0.1, 100000, 2)
        };

        Assert.Equal(before.ReferencePlane, after.ReferencePlane);
        Assert.True(after.HasReferencePlane);
    }

    static RenderProjection EmptyProjection() => new(
        new RenderCameraProjection(new(0, -10, 10), new(0, 0, -1), Vector3d.UnitY,
            60, 0.1, 100000, 1), [], false, default,
        Assist: EditorViewportAssistState.Default,
        Map: MapRenderSnapshot.Empty,
        ReferencePlane: ReferencePlaneRenderSnapshot.Default);
}
