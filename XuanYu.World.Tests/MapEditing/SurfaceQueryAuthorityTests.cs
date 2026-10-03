using XuanYu.Core.Map;
using XuanYu.World;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

public sealed class SurfaceQueryAuthorityTests
{
    [Fact]
    public void Surface_owner_reports_no_terrain_without_silent_zero_success()
    {
        var result = new WorldMapStateOwner().QuerySurface(0, 0);

        Assert.Equal(WorldQueryStatus.NoTerrain, result.Status);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Surface_state_reports_out_of_bounds_as_typed_failure()
    {
        var result = State().QuerySurface(5000, 0);

        Assert.Equal(WorldQueryStatus.OutOfBounds, result.Status);
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Surface_binding_mismatch_is_invalid_binding()
    {
        var result = SurfaceQueryResult.InvalidBinding;

        Assert.Equal(WorldQueryStatus.InvalidBinding, result.Status);
        Assert.False(result.IsValid);
    }

    static WorldMapState State() => new(
        "map", "Test", 2000, 2000, MapSurfaceKind.GentleHillsV1,
        0, 12, 400, 1);
}
