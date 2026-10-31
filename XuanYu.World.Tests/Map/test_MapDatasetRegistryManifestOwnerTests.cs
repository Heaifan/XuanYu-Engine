using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

public sealed class test_MapDatasetRegistryManifestOwnerTests
{
    [Fact]
    public void Layer_update_publishes_through_the_bound_manifest_owner()
    {
        var manifest = MapManifest.CreateNew("map", "地图");
        var owner = new MapManifestOwner();
        owner.Load("map.json", manifest);
        var registry = new MapDatasetRegistry("map.json", owner);

        var result = registry.UpdateLayerStates([]);

        Assert.True(result.Succeeded);
        Assert.Same(owner.CurrentManifest, registry.CurrentManifest);
        Assert.True(owner.IsDirty);
    }
}
