using XuanYu.Core.Math;
using XuanYu.World;

namespace XuanYu.Editor.SceneDocument;

static class SceneDocumentValidator
{
    public static SceneDocumentResult<SceneDocumentJson> Validate(SceneDocumentJson? doc)
    {
        if (doc is null) return Fail("BrokenJson", "场景文件结构损坏。", "Parse");
        if (doc.Format != "XuanYuScene") return Fail("UnsupportedFormat", "不是玄域场景文件。", "Schema", "format");
        if (doc.SchemaVersion is < 1 or > 2) return Fail("UnsupportedSchema", "场景文件版本不受支持。", "Schema", "schemaVersion");
        if (doc.Scene is null || doc.Entities is null)
            return Fail("BrokenJson", "场景文件结构损坏。", "Validate");
        if (string.IsNullOrWhiteSpace(doc.Scene.Id)) return Fail("InvalidSceneId", "场景ID不能为空。", "Validate", "scene.id");
        var ids = new HashSet<int>();
        foreach (var entity in doc.Entities)
        {
            if (entity is null) return Fail("BrokenJson", "实体数据不能为空。", "Validate");
            if (entity.Id <= 0 || !ids.Add(entity.Id)) return Fail("DuplicateEntityId", "实体ID重复或非法。", "Validate", $"entity.id={entity.Id}");
            if (string.IsNullOrWhiteSpace(entity.Name)) return Fail("InvalidEntityName", "实体名称不能为空。", "Validate", $"entity.id={entity.Id}");
            if (doc.SchemaVersion == 2 && string.IsNullOrWhiteSpace(entity.EntityType))
                return Fail("MissingEntityType", "Schema v2 实体类型不能为空。", "Validate", $"entity.id={entity.Id}");
            if (entity.EntityType is not null && !WorldEntityTypes.TryParse(entity.EntityType, out _))
                return Fail("UnknownEntityType", "实体类型不受支持。", "Validate", $"entity.id={entity.Id}");
            if (entity.SiblingOrder < 0)
                return Fail("InvalidSiblingOrder", "实体顺序不能为负数。", "Validate", $"entity.id={entity.Id}");
            if (entity.Position is null || entity.Rotation is null || entity.Scale is null ||
                !Finite(entity.Position) || !Finite(entity.Rotation) || !Finite(entity.Scale))
                return Fail("InvalidTransform", "实体Transform包含非法数值。", "Validate", $"entity.id={entity.Id}");
            if (entity.Scale.X <= 0 || entity.Scale.Y <= 0 || entity.Scale.Z <= 0)
                return Fail("InvalidTransform", "实体Scale必须大于0。", "Validate", $"entity.id={entity.Id}");
        }
        if (doc.Entities.Any(e => e.ParentId is not null && !ids.Contains(e.ParentId.Value)))
            return Fail("MissingParent", "实体父节点不存在。", "Validate", "parentId");
        if (HasCycle(doc.Entities)) return Fail("HierarchyCycle", "实体层级存在循环。", "Validate", "parentId");
        if (doc.Entities.GroupBy(e => e.ParentId).Any(g => g.Select(e => e.SiblingOrder).Distinct().Count() != g.Count()))
            return Fail("DuplicateSiblingOrder", "同级实体顺序重复。", "Validate", "siblingOrder");
        return SceneDocumentResult<SceneDocumentJson>.Ok(doc);
    }

    static bool HasCycle(IReadOnlyList<SceneEntityJson> entities)
    {
        var parents = entities.ToDictionary(e => e.Id, e => e.ParentId);
        foreach (var id in parents.Keys)
        {
            var seen = new HashSet<int>();
            int? current = id;
            while (current is not null)
            {
                if (!seen.Add(current.Value)) return true;
                current = parents[current.Value];
            }
        }
        return false;
    }

    static bool Finite(Vector3Json value) =>
        Valid(value.X) && Valid(value.Y) && Valid(value.Z);

    static bool Valid(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

    static SceneDocumentResult<SceneDocumentJson> Fail(
        string code,
        string message,
        string stage,
        string detail = "") =>
        SceneDocumentResult<SceneDocumentJson>.Fail(code, message, stage, detail);
}
