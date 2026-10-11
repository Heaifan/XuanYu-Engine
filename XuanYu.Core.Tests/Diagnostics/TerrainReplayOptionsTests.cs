using XuanYu.Core.Diagnostics;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Diagnostics;

public sealed class TerrainReplayOptionsTests
{
    [Theory]
    [InlineData("baseline", TerrainReplayVariant.Baseline)]
    [InlineData("far-expanded", TerrainReplayVariant.ExpandedFarPlane)]
    [InlineData("cull-off", TerrainReplayVariant.DisableChunkCulling)]
    [InlineData("lod0", TerrainReplayVariant.FixedLod0)]
    [InlineData("near-small", TerrainReplayVariant.ReducedNearPlane)]
    public void Parses_only_named_diagnostic_variants(string value,
        TerrainReplayVariant expected)
    {
        Assert.True(TerrainReplayOptions.TryParseVariant(value, out var actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Variant_changes_only_its_requested_render_decision()
    {
        var camera = new CameraState(new(10, 20, 30), new(0, 0, -1),
            Vector3d.UnitY, 60, 1, 1000, 7);

        var expanded = TerrainReplayOptions.ApplyCamera(camera,
            TerrainReplayVariant.ExpandedFarPlane);
        var reduced = TerrainReplayOptions.ApplyCamera(camera,
            TerrainReplayVariant.ReducedNearPlane);

        Assert.Equal(2000, expanded.FarPlane);
        Assert.Equal(camera.NearPlane, expanded.NearPlane);
        Assert.Equal(camera.Position, expanded.Position);
        Assert.Equal(0.5, reduced.NearPlane);
        Assert.Equal(camera.FarPlane, reduced.FarPlane);
        Assert.False(TerrainReplayOptions.ShouldCull(false,
            TerrainReplayVariant.DisableChunkCulling));
        Assert.True(TerrainReplayOptions.ShouldCull(false,
            TerrainReplayVariant.Baseline));
        Assert.Equal(0, TerrainReplayOptions.SelectLod(3,
            TerrainReplayVariant.FixedLod0));
        Assert.Equal(3, TerrainReplayOptions.SelectLod(3,
            TerrainReplayVariant.Baseline));
    }

    [Theory]
    [InlineData("1221.4", 1221.4)]
    [InlineData("0.5", 0.5)]
    public void Parses_finite_positive_far_plane_thresholds(string value, double expected)
    {
        Assert.True(TerrainReplayOptions.TryParseFarPlane(value, out var actual));
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("not-a-number")]
    public void Rejects_invalid_far_plane_thresholds(string? value)
    {
        Assert.False(TerrainReplayOptions.TryParseFarPlane(value, out _));
    }

    [Fact]
    public void Far_plane_threshold_preserves_frozen_camera_pose()
    {
        var camera = new CameraState(new(10, 20, 30), new(0, 0, -1),
            Vector3d.UnitY, 60, 1, 1000, 7);

        var adjusted = TerrainReplayOptions.ApplyCamera(camera,
            TerrainReplayVariant.Baseline, 1800);

        Assert.Equal(1800, adjusted.FarPlane);
        Assert.Equal(new CameraState(camera.Position, camera.Forward, camera.Up,
            camera.VerticalFovDegrees, camera.NearPlane, 1800, camera.Revision), adjusted);
    }
}
