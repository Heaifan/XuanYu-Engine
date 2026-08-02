using XuanYu.Core.Map;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D3：唯一地表采样器——确定性、范围与参数语义。
public sealed class MapSurfaceSamplerTests
{
    [Fact]
    public void Flat_returns_base_height_everywhere()
    {
        Assert.Equal(5.0, MapSurfaceSampler.SampleHeight(
            MapSurfaceKind.Flat, 5.0, 12.0, 400.0, 1, 0.0, 0.0));
        Assert.Equal(5.0, MapSurfaceSampler.SampleHeight(
            MapSurfaceKind.Flat, 5.0, 12.0, 400.0, 1, 123.45, -678.9));
    }

    [Fact]
    public void GentleHills_same_coordinate_always_same_height()
    {
        var a = MapSurfaceSampler.SampleHeight(
            MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 1, 100.0, 200.0);
        var b = MapSurfaceSampler.SampleHeight(
            MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 1, 100.0, 200.0);
        Assert.Equal(a, b);
    }

    [Fact]
    public void GentleHills_is_deterministic_across_many_samples()
    {
        for (var i = 0; i < 200; i++)
        {
            var x = i * 13.7 - 500;
            var y = i * 7.3 + 100;
            var a = MapSurfaceSampler.SampleHeight(
                MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 7, x, y);
            var b = MapSurfaceSampler.SampleHeight(
                MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 7, x, y);
            Assert.Equal(a, b);
        }
    }

    [Fact]
    public void GentleHills_height_stays_within_amplitude_range()
    {
        for (var i = -50; i <= 50; i++)
        {
            var z = MapSurfaceSampler.SampleHeight(
                MapSurfaceKind.GentleHillsV1, 10.0, 12.0, 400.0, 1,
                i * 3.0, i * 5.0);
            Assert.InRange(z, 10.0 - 12.0, 10.0 + 12.0);
        }
    }

    [Fact]
    public void Different_seeds_produce_different_heights()
    {
        var z1 = MapSurfaceSampler.SampleHeight(
            MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 1, 100.0, 200.0);
        var z2 = MapSurfaceSampler.SampleHeight(
            MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 2, 100.0, 200.0);
        Assert.NotEqual(z1, z2);
    }

    [Fact]
    public void Different_locations_produce_different_heights()
    {
        var z1 = MapSurfaceSampler.SampleHeight(
            MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 1, 100.0, 200.0);
        var z2 = MapSurfaceSampler.SampleHeight(
            MapSurfaceKind.GentleHillsV1, 0.0, 12.0, 400.0, 1, 101.0, 200.0);
        Assert.NotEqual(z1, z2);
    }

    [Fact]
    public void Amplitude_zero_means_flat_hills()
    {
        var z = MapSurfaceSampler.SampleHeight(
            MapSurfaceKind.GentleHillsV1, 7.0, 0.0, 400.0, 1, 50.0, 50.0);
        Assert.Equal(7.0, z);
    }

    [Fact]
    public void Flat_ignores_other_parameters()
    {
        Assert.Equal(-3.0, MapSurfaceSampler.SampleHeight(
            MapSurfaceKind.Flat, -3.0, double.MaxValue, 1e-9, int.MaxValue, 1e9, -1e9));
    }
}
