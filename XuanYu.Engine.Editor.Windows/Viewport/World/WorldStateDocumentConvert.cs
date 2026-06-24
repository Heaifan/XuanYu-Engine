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
            var position = new WorldVector3Document(
                (float)(pos?.Value.X ?? 0),
                (float)(pos?.Value.Y ?? 0),
                (float)(pos?.Value.Z ?? 0));

            entityDocs.Add(new WorldEntityDocument(
                entity.EntityId.ToString(),
                entity.DisplayName,
                [new TransformComponentDocument(position)]));
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
            var pos = entity.Components is not null
                ? entity.Components.OfType<TransformComponentDocument>().FirstOrDefault()?.Position
                : null;
            var position = new Vector3d(pos?.X ?? 0, pos?.Y ?? 0, pos?.Z ?? 0);
            world.CreateEntity(entity.DisplayName, position);
        }
        return world;
    }
}
