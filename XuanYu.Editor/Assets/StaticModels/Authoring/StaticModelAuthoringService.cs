using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.Editor.Assets;

public sealed record StaticModelAuthorResult(
    bool Succeeded,
    WorldEntitySnapshot? Entity,
    AssetId AssetId,
    StaticModelData? Model,
    string SourcePath,
    string UserMessage,
    string TechnicalDetail)
{
    public static StaticModelAuthorResult Success(
        WorldEntitySnapshot entity,
        AssetId assetId,
        StaticModelData model,
        string sourcePath) =>
        new(true, entity, assetId, model, sourcePath, "", "");

    public static StaticModelAuthorResult Fail(
        string message,
        string detail = "") =>
        new(false, null, default, null, "", message, detail);
}

// D3：GLB 导入 → World 实体创建 → Catalog 绑定 的最小事务组合服务。
// 回滚：导入失败不创建实体；实体创建失败不写 Catalog；绑定失败删除已建实体。
public sealed class StaticModelAuthoringService
{
    readonly GlbImportService _importer = new();

    public StaticModelAuthorResult Import(
        string path,
        SceneStateOwner scene,
        SceneStaticModelCatalog catalog)
    {
        var normalized = NormalizePath(path, out var pathError);
        if (pathError is not null || normalized is null)
            return StaticModelAuthorResult.Fail(pathError ?? "导入路径无效。", path);

        var imported = _importer.ImportFile(normalized);
        if (!imported.Succeeded) return StaticModelAuthorResult.Fail(imported.UserMessage, imported.TechnicalDetail);
        var model = imported.Model!;

        var assetId = AssetId.New();
        WorldEntitySnapshot entity;
        try
        {
            entity = scene.AddStaticModelEntity(
                EntityDisplayName(normalized),
                CommittedTransform.Identity,
                model.LocalBounds);
        }
        catch (Exception)
        {
            return StaticModelAuthorResult.Fail("创建静态模型实体失败。", "实体创建异常。");
        }

        if (!catalog.Bind(entity.EntityKey, assetId, normalized, model))
        {
            scene.DestroyEntity(entity.EntityKey);
            return StaticModelAuthorResult.Fail("静态模型绑定失败，已回滚实体。", $"entity={entity.EntityKey}");
        }

        return StaticModelAuthorResult.Success(entity, assetId, model, normalized);
    }

    static string? NormalizePath(string path, out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(path)) { error = "导入路径不能为空。"; return null; }
        string full;
        try { full = Path.GetFullPath(path); }
        catch (IOException) { error = "导入路径无效。"; return null; }
        if (!File.Exists(full)) { error = "GLB 源文件不存在。"; return null; }
        if (!string.Equals(Path.GetExtension(full), ".glb", StringComparison.OrdinalIgnoreCase))
        {
            error = "仅支持 .glb 文件。";
            return null;
        }
        return full;
    }

    static string EntityDisplayName(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        return string.IsNullOrWhiteSpace(name) ? "静态模型" : name;
    }
}
