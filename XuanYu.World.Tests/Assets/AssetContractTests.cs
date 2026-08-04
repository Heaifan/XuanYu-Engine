using XuanYu.Core.Math;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.World;

public sealed class AssetContractTests
{
    [Fact]
    public void Asset_id_is_stable_serializable_and_path_safe()
    {
        var id = AssetId.New();
        Assert.True(AssetId.TryParse(id.ToString(), out var parsed));
        Assert.Equal(id, parsed);
        Assert.Equal($"models/{id}/source.glb", SceneAssetPathPolicy.ModelSourceRelativePath(id));
    }

    [Theory]
    [InlineData("../x.glb")]
    [InlineData("C:/x.glb")]
    [InlineData("//server/share/x.glb")]
    [InlineData("models\\asset/source.glb")]
    public void Managed_asset_paths_reject_escape_or_absolute_paths(string path)
    {
        Assert.False(SceneAssetPathPolicy.IsSafeRelativePath(path));
    }

    [Fact]
    public void Managed_asset_path_resolves_inside_scene_asset_root()
    {
        var id = AssetId.New();
        var root = Path.Combine(Path.GetTempPath(), "scene.xyassets");
        var relative = SceneAssetPathPolicy.ModelSourceRelativePath(id);
        Assert.True(SceneAssetPathPolicy.TryResolveManagedPath(root, relative, out var full));
        Assert.EndsWith(Path.Combine("models", id.ToString(), "source.glb"), full);
    }

    [Fact]
    public void Gltf_y_up_maps_to_xuanyu_z_up_without_winding_flip()
    {
        Assert.Equal(new Vector3d(1, 0, 0), GltfCoordinatePolicy.ToXuanYuPosition(new Vector3d(1, 0, 0)));
        Assert.Equal(new Vector3d(0, 0, 1), GltfCoordinatePolicy.ToXuanYuPosition(new Vector3d(0, 1, 0)));
        Assert.Equal(new Vector3d(0, -1, 0), GltfCoordinatePolicy.ToXuanYuPosition(new Vector3d(0, 0, 1)));
        Assert.Equal((0, 1, 2), GltfCoordinatePolicy.ToXuanYuTriangle(0, 1, 2));
    }
}
