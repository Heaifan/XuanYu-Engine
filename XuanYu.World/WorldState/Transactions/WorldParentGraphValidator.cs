using XuanYu.Core.Identity;

namespace XuanYu.World;

internal static class WorldParentGraphValidator
{
    internal static void Validate(
        IReadOnlyList<WorldEntitySnapshot> entities,
        IReadOnlyDictionary<EntityId, WorldEntitySnapshot> byId)
    {
        var siblingOrders = new HashSet<(EntityId Parent, int Order)>();
        foreach (var entity in entities)
        {
            if (entity.ParentId.IsValid && !byId.ContainsKey(entity.ParentId))
                throw WorldInvariantValidator.Invalid("候选实体引用了不存在的 ParentId。");
            if (!siblingOrders.Add((entity.ParentId, entity.SiblingOrder)))
                throw WorldInvariantValidator.Invalid("同一父实体下的 SiblingOrder 必须唯一。");
        }

        ValidateAcyclicParents(entities, byId);
    }

    static void ValidateAcyclicParents(
        IReadOnlyList<WorldEntitySnapshot> entities,
        IReadOnlyDictionary<EntityId, WorldEntitySnapshot> byId)
    {
        var complete = new HashSet<EntityId>();
        foreach (var entity in entities)
        {
            var path = new HashSet<EntityId>();
            var current = entity.EntityKey;
            while (byId.TryGetValue(current, out var node) && node.ParentId.IsValid)
            {
                if (complete.Contains(current)) break;
                if (!path.Add(current))
                    throw WorldInvariantValidator.Invalid("候选实体的 ParentId 关系存在环。");
                current = node.ParentId;
            }
            complete.UnionWith(path);
        }
    }
}
