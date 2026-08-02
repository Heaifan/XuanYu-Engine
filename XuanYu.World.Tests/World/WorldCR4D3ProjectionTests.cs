using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;
using XuanYu.Editor.Assets;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed class WorldCR4D3ProjectionTests
{
    [Fact]
    public void Static_model_entity_uses_catalog_render_key()
    {
        var (scene, catalog, resources) = BindModel();
        var projection = Project(scene, catalog, resources);

        Assert.True(projection.Success);
        var entity = Assert.Single(projection.Projection.Entities,
            e => e.EntityType == RenderEntityType.StaticModel);
        Assert.Equal(Key(catalog.Snapshot.Single().AssetId), entity.StaticModelKey);
        Assert.Single(projection.Projection.StaticModelResources);
    }

    [Fact]
    public void Resource_is_not_reused_for_other_entities()
    {
        var (scene, catalog, resources) = BindModel();
        var other = scene.CreateEntity("其它实体", WorldEntityTypes.Cube,
            CommittedTransform.Identity, MinimalExtent);
        _ = other;
        var projection = Project(scene, catalog, resources);

        var cube = Assert.Single(projection.Projection.Entities,
            e => e.EntityType == RenderEntityType.Cube);
        Assert.Equal(default, cube.StaticModelKey);
        Assert.Single(projection.Projection.Entities, e => e.EntityType == RenderEntityType.StaticModel);
    }

    [Fact]
    public void Unbound_static_model_entity_is_skipped()
    {
        var scene = new SceneStateOwner(null, seedInitialEntity: false);
        scene.CreateEntity("模型", WorldEntityTypes.StaticModel, CommittedTransform.Identity, MinimalExtent);
        var projection = Project(scene, new SceneStaticModelCatalog(), new Dictionary<AssetId, RenderStaticModelResource>());

        Assert.True(projection.Success);
        Assert.DoesNotContain(projection.Projection.Entities,
            e => e.EntityType == RenderEntityType.StaticModel);
    }

    static RenderProjectionResult Project(
        SceneStateOwner scene,
        SceneStaticModelCatalog catalog,
        IReadOnlyDictionary<AssetId, RenderStaticModelResource> resources)
    {
        var snapshot = new SceneRenderSnapshot(
            SceneWorldProjection.ToSceneEntity(scene.Entities.First()),
            RenderEntities: scene.Entities.Select(SceneWorldProjection.ToSceneEntity).ToArray(),
            Camera: new CameraState(
                new Vector3d(0, -5, 5), new Vector3d(0, 0, -1),
                new Vector3d(0, 1, 0), 45, 0.01, 100, 1));
        return SceneRenderProjectionAdapter.TryCreate(snapshot,
            staticModelCatalog: catalog, staticModelResources: resources);
    }

    static (SceneStateOwner, SceneStaticModelCatalog, IReadOnlyDictionary<AssetId, RenderStaticModelResource>) BindModel()
    {
        var scene = new SceneStateOwner(null, seedInitialEntity: false);
        var catalog = new SceneStaticModelCatalog();
        var authoring = new StaticModelAuthoringService();
        var result = authoring.Import(TempGlb(), scene, catalog);
        var model = result.Model!;
        var resource = StaticModelRenderAdapter.ToRenderResource(model, Key(result.AssetId), 1);
        return (scene, catalog, new Dictionary<AssetId, RenderStaticModelResource>
        {
            [result.AssetId] = resource
        });
    }

    static string TempGlb()
    {
        var path = Path.Combine(Path.GetTempPath(), "xuanyu-d3p-" + Guid.NewGuid().ToString("N") + ".glb");
        File.WriteAllBytes(path, WorldCR4D1GlbFactory.Triangle());
        return path;
    }

    static RenderStaticModelKey Key(AssetId assetId) => new(assetId.Value);

    static readonly SpatialAabb MinimalExtent =
        new(new Vector3d(-0.5, -0.5, -0.5), new Vector3d(0.5, 0.5, 0.5));
}
