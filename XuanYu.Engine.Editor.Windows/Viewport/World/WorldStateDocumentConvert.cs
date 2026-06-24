using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Project.World.Documents;
using XuanYu.Engine.World;

namespace XuanYu.Engine.Editor.Windows.Viewport.World;

/// <summary>WorldState 与 WorldDocument 之间的转换帮助方法。</summary>
static class WorldStateDocumentConvert
{
    public static WorldDocument ToDocument(WorldState world, string worldId, string displayName)
    {
        var entities = world.ListEntities();
        var entityDocs = new List<WorldEntityDocument>(entities.Count);

        foreach (var entity in entities)
        {
            var pos = world.FindPosition(entity.EntityId);
            var rot = world.FindRotation(entity.EntityId);
            var scale = world.FindScale(entity.EntityId);

            entityDocs.Add(new WorldEntityDocument(
                entity.EntityId.ToString(),
                entity.DisplayName,
                [new TransformComponentDocument
                {
                    Position = ToDocVector(pos?.Value ?? Vector3d.Zero),
                    RotationDegrees = ToDocVector(rot?.Value ?? Vector3d.Zero),
                    Scale = ToDocVector(scale?.Value ?? new Vector3d(1, 1, 1)),
                }]));
        }

        return new WorldDocument(
            1, worldId, displayName,
            entityDocs, WorldMetadataDocument.Default);
    }

    public static WorldState ToWorldState(WorldDocument doc)
    {
        var world = new WorldState();
        foreach (var entity in doc.Entities)
        {
            var t = entity.Components is not null
                ? entity.Components.OfType<TransformComponentDocument>().FirstOrDefault()
                : null;

            var position = ToEngineVector(t?.GetPositionOrDefault());
            var rotation = ToEngineVector(t?.GetRotationDegreesOrDefault());
            var scale = ToEngineVector(t?.GetScaleOrDefault());

            world.CreateEntity(entity.DisplayName, position, rotation, scale);
        }
        return world;
    }

    static WorldVector3Document ToDocVector(Vector3d v) =>
        new((float)v.X, (float)v.Y, (float)v.Z);

    static Vector3d ToEngineVector(WorldVector3Document? v) =>
        v is null ? Vector3d.Zero : new Vector3d(v.X, v.Y, v.Z);

}
