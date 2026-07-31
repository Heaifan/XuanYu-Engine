using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.Editor.SceneDocument;

static class SceneDocumentMapper
{
    public static SceneDocumentJson ToJson(SceneDocumentSnapshot snapshot) =>
        new("XuanYuScene", 1, new SceneInfoJson(snapshot.SceneId, snapshot.SceneName),
            snapshot.Entities.OrderBy(e => e.SiblingOrder).ThenBy(e => e.Id.Value).Select(ToJson).ToArray());

    public static SceneEntityJson ToJson(SceneDocumentEntity entity)
    {
        var t = entity.Transform;
        return new SceneEntityJson(entity.Id.Value, entity.Name,
            entity.ParentId.IsValid ? entity.ParentId.Value : null, entity.SiblingOrder,
            ToJson(t.Position), ToJson(t.Rotation), ToJson(t.Scale));
    }

    public static SceneDocumentSnapshot ToSnapshot(SceneDocumentJson doc) =>
        new(doc.Scene.Id, doc.Scene.Name, doc.Entities.Select(ToEntity).ToArray());

    static SceneDocumentEntity ToEntity(SceneEntityJson entity) =>
        new(EntityId.FromInt(entity.Id), entity.Name,
            entity.ParentId is > 0 ? EntityId.FromInt(entity.ParentId.Value) : EntityId.None,
            entity.SiblingOrder,
            new CommittedTransform(ToVector(entity.Position), ToVector(entity.Rotation), ToVector(entity.Scale)));

    static Vector3Json ToJson(Vector3d v) => new(v.X, v.Y, v.Z);

    static Vector3d ToVector(Vector3Json v) => new(v.X, v.Y, v.Z);
}
