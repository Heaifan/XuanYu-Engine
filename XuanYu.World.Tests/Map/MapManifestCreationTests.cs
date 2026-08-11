using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-DOC-A-R1：Manifest 创建合同 A01～A07。
public sealed class MapManifestCreationTests
{
    [Fact]
    public void New_manifest_has_the_approved_minimum_contract()
    {
        var manifest = MapManifest.CreateNew("south-china", "华南");
        var result = MapManifestValidator.Validate(manifest);

        Assert.True(result.Succeeded);
        Assert.Equal("xuanyu-map", manifest.Format);
        Assert.Equal("0.1.0", manifest.Version);
        Assert.Equal("south-china", manifest.Id);
        Assert.Equal("华南", manifest.Name);
        Assert.Equal("local_cartesian", manifest.CoordinateSystem.Type);
        Assert.Equal("meter", manifest.CoordinateSystem.Unit);
        Assert.Empty(manifest.Datasets);
        Assert.Empty(manifest.Assets);
    }

    [Fact]
    public void Current_world_map_projects_to_manifest_identity()
    {
        var manifest = MapManifest.FromMap(XuanYu.World.Map.MapDefaultDefinition.CreateDefault());

        Assert.Equal("xuanyu-map", manifest.Format);
        Assert.Equal("未命名地图", manifest.Name);
        Assert.Equal(32, manifest.Id.Length);
        Assert.Empty(manifest.Datasets);
        Assert.Empty(manifest.Assets);
    }
}
