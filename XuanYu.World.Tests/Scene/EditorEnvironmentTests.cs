using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Editor.Assets;
using XuanYu.Editor.SceneDocument;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

// WORLD-D-R1：编辑器环境（天空/光照）契约测试。
// 不触碰 GPU：只验证默认材质路径与场景文档边界。
public sealed class EditorEnvironmentTests
{
    readonly GlbImportService _service = new();

    [Fact]
    public void Glb_without_material_uses_neutral_default_base_color()
    {
        var result = _service.ImportBytes(GlbFactory.Triangle(), "test.glb");

        Assert.True(result.Succeeded);
        Assert.Equal(StaticModelColor.Neutral, result.Model!.Primitives[0].BaseColorFactor);
    }

    [Fact]
    public async Task Scene_document_has_no_sky_or_lighting_fields()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.xyscene");
        var scene = new SceneStateOwner(null, seedInitialEntity: false);
        scene.CreateEntity("环境测试实体", "MinimalSceneEntity",
            new CommittedTransform(new Vector3d(1, 2, 3), new Vector3d(4, 5, 6), new Vector3d(1, 2, 3)));
        var storage = new SceneStorageService();

        var saved = await storage.SaveAsync(path,
            SceneDocumentWorldBridge.Capture(scene, "scene-1", "环境测试场景"));

        Assert.True(saved.Succeeded);
        var json = await File.ReadAllTextAsync(path);
        Assert.DoesNotContain("\"sky", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\"light", json, StringComparison.OrdinalIgnoreCase);
    }
}
