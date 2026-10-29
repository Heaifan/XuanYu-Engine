using System.Collections.ObjectModel;

namespace XuanYu.World;

internal sealed class WorldStateCandidate
{
    readonly ReadOnlyCollection<WorldEntitySnapshot> _entities;

    WorldStateCandidate(WorldEntitySnapshot[] entities) =>
        _entities = Array.AsReadOnly(entities);

    internal IReadOnlyList<WorldEntitySnapshot> Entities => _entities;

    internal static WorldStateCandidate Create(
        IReadOnlyList<WorldEntitySnapshot> entities)
    {
        if (entities is null)
            throw new ArgumentException("候选实体集合不能为空。", nameof(entities));
        var ownedEntities = new WorldEntitySnapshot[entities.Count];
        for (var index = 0; index < entities.Count; index++)
            ownedEntities[index] = entities[index];

        WorldInvariantValidator.Validate(ownedEntities);
        return new WorldStateCandidate(ownedEntities);
    }
}
