using XuanYu.Core.Math;

namespace XuanYu.Editor.SceneDocument;

static class SceneDocumentValidator
{
    public static SceneDocumentResult<SceneDocumentJson> Validate(SceneDocumentJson? doc)
    {
        if (doc is null) return Fail("BrokenJson", "场景文件结构损坏。");
        if (doc.Format != "XuanYuScene") return Fail("UnsupportedFormat", "不是玄域场景文件。");
        if (doc.SchemaVersion > 1) return Fail("SchemaTooHigh", "场景文件版本高于当前编辑器支持范围。");
        if (string.IsNullOrWhiteSpace(doc.Scene.Id)) return Fail("InvalidSceneId", "场景ID不能为空。");
        var ids = new HashSet<int>();
        foreach (var entity in doc.Entities)
        {
            if (entity.Id <= 0 || !ids.Add(entity.Id)) return Fail("DuplicateEntityId", "实体ID重复或非法。");
            if (string.IsNullOrWhiteSpace(entity.Name)) return Fail("InvalidEntityName", "实体名称不能为空。");
            if (!Finite(entity.Position) || !Finite(entity.Rotation) || !Finite(entity.Scale))
                return Fail("InvalidTransform", "实体Transform包含非法数值。");
            if (entity.Scale.X <= 0 || entity.Scale.Y <= 0 || entity.Scale.Z <= 0)
                return Fail("InvalidTransform", "实体Scale必须大于0。");
        }
        if (doc.Entities.Any(e => e.ParentId is not null && !ids.Contains(e.ParentId.Value)))
            return Fail("MissingParent", "实体父节点不存在。");
        if (HasCycle(doc.Entities)) return Fail("HierarchyCycle", "实体层级存在循环。");
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

    static SceneDocumentResult<SceneDocumentJson> Fail(string code, string message) =>
        SceneDocumentResult<SceneDocumentJson>.Fail(code, message);
}
