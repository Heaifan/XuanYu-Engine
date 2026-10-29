using XuanYu.Core.Identity;

namespace XuanYu.World;

internal static class WorldInvariantValidator
{
    internal static void Validate(IReadOnlyList<WorldEntitySnapshot> entities)
    {
        var byId = new Dictionary<EntityId, WorldEntitySnapshot>();
        foreach (var entity in entities)
        {
            WorldCandidateEntityValidator.Validate(entity);
            if (!byId.TryAdd(entity.EntityKey, entity))
                throw Invalid("候选实体包含重复的 EntityId。");
        }

        WorldParentGraphValidator.Validate(entities, byId);
    }

    internal static ArgumentException Invalid(string message) =>
        new(message, "entities");
}
