using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.World;

public sealed class WorldCR4D3CatalogTests
{
    [Fact]
    public void Bind_snapshot_sorted_by_asset_id()
    {
        var catalog = new SceneStaticModelCatalog();
        var model = Model();
        // 确定性 AssetId，保证字典序固定：…00 < …01。
        var a1 = Parse("asset_00000000000000000000000000000000");
        var a2 = Parse("asset_00000000000000000000000000000001");

        Assert.True(catalog.Bind(EntityId.FromInt(2), a1, "p2.glb", model));
        Assert.True(catalog.Bind(EntityId.FromInt(1), a2, "p1.glb", model));
        Assert.Equal([a1.Value, a2.Value], catalog.Snapshot.Select(b => b.AssetId.Value));
    }

    [Fact]
    public void Same_entity_cannot_bind_twice()
    {
        var catalog = new SceneStaticModelCatalog();
        var model = Model();
        var key = EntityId.FromInt(1);

        Assert.True(catalog.Bind(key, AssetId.New(), "a.glb", model));
        Assert.False(catalog.Bind(key, AssetId.New(), "b.glb", model));
        Assert.Single(catalog.Snapshot);
    }

    [Fact]
    public void Remove_and_clear_remove_bindings()
    {
        var catalog = new SceneStaticModelCatalog();
        var model = Model();
        var key = EntityId.FromInt(1);
        var assetId = AssetId.New();
        catalog.Bind(key, assetId, "a.glb", model);

        Assert.True(catalog.Remove(key));
        Assert.False(catalog.Remove(key));
        Assert.Empty(catalog.Snapshot);

        catalog.Bind(key, assetId, "a.glb", model);
        catalog.Clear();
        Assert.Empty(catalog.Snapshot);
        Assert.False(catalog.TryGetByAsset(assetId, out _));
    }

    [Fact]
    public void Revision_increments_on_changes()
    {
        var catalog = new SceneStaticModelCatalog();
        var model = Model();
        var key = EntityId.FromInt(1);
        var r0 = catalog.Revision;

        catalog.Bind(key, AssetId.New(), "a.glb", model);
        Assert.True(catalog.Revision > r0);
        var r1 = catalog.Revision;
        catalog.Remove(key);
        Assert.True(catalog.Revision > r1);
    }

    [Fact]
    public void Changed_event_raised_on_mutation()
    {
        var catalog = new SceneStaticModelCatalog();
        var model = Model();
        var raised = 0;
        catalog.Changed += () => raised++;

        catalog.Bind(EntityId.FromInt(1), AssetId.New(), "a.glb", model);
        catalog.Clear();
        Assert.Equal(2, raised);
    }

    static StaticModelData Model() =>
        new GlbImportService().ImportBytes(WorldCR4D1GlbFactory.Triangle(), "t.glb").Model!;

    static AssetId Parse(string value) =>
        AssetId.TryParse(value, out var id) ? id : throw new InvalidOperationException($"非法 AssetId：{value}");
}
