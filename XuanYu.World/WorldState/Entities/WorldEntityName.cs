using XuanYu.Core.Identity;

namespace XuanYu.World;

public static class WorldEntityName
{
    public static string Unique(
        string requested,
        IEnumerable<WorldEntitySnapshot> entities,
        EntityId except = default)
    {
        var basis = requested.Trim();
        if (basis.Length == 0) throw new ArgumentException("Entity name cannot be empty.", nameof(requested));
        var used = entities
            .Where(x => x.EntityKey != except)
            .Select(x => x.Name)
            .ToHashSet(StringComparer.Ordinal);
        if (!used.Contains(basis)) return basis;
        for (var suffix = 1; suffix < int.MaxValue; suffix++)
        {
            var candidate = $"{basis} {suffix:000}";
            if (!used.Contains(candidate)) return candidate;
        }
        throw new InvalidOperationException("No unique entity name is available.");
    }
}
