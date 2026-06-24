using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.World;

namespace FluidWarfare.Editor.WorldHierarchy;

/// <summary>从 WorldState 构建 WorldHierarchyTree。排序稳定：分组按语义顺序，实体按 DisplayName+EntityId。</summary>
public static class WorldHierarchyTreeBuilder
{
    static readonly string[] GroupOrder = ["地形", "单位", "工事", "触发器", "其他"];

    public static WorldHierarchyTree Build(WorldState worldState, IReadOnlyDictionary<EntityId, string>? entityGroupLookup = null)
    {
        var entities = worldState.ListEntities();
        if (entities.Count == 0) return WorldHierarchyTree.Empty;
        var groups = new Dictionary<string, List<WorldEntityInfo>>();
        foreach (var entity in entities)
        { var g = entityGroupLookup?.GetValueOrDefault(entity.EntityId) ?? "其他"; if (!groups.ContainsKey(g)) groups[g] = []; groups[g].Add(entity); }
        var sortedGroups = groups.OrderBy(g => Array.IndexOf(GroupOrder, g.Key) >= 0 ? Array.IndexOf(GroupOrder, g.Key) : int.MaxValue).ThenBy(g => g.Key).ToList();
        var entityNodes = new Dictionary<string, WorldHierarchyNode>();
        var ancestorMap = new Dictionary<string, IReadOnlyList<string>>();
        var groupNodes = new List<WorldHierarchyNode>();
        var totalNodes = 1; var entityCount = 0;
        foreach (var (groupName, items) in sortedGroups)
        {
            if (items.Count == 0) continue;
            var groupNodeId = $"group:{GetGroupKey(groupName)}";
            var childNodes = new List<WorldHierarchyNode>();
            var sortedItems = items.OrderBy(x => x.DisplayName).ThenBy(x => x.EntityId.Value).ToList();
            foreach (var info in sortedItems)
            {
                var node = new WorldHierarchyNode($"entity:{info.EntityId.Value}", WorldHierarchyNodeKind.Entity, info.DisplayName, info.Source?.RelativePath, info.EntityId, "entity", true, []);
                childNodes.Add(node); entityNodes[info.EntityId.Value.ToString()] = node; entityCount++;
                ancestorMap[info.EntityId.Value.ToString()] = new List<string> { "world:root", groupNodeId };
            }
            groupNodes.Add(new WorldHierarchyNode(groupNodeId, WorldHierarchyNodeKind.EntityGroup, $"{groupName} ({items.Count})", null, null, "group", false, childNodes.AsReadOnly()));
            totalNodes += 1 + childNodes.Count;
        }
        foreach (var gn in groupNodes) UpdateDescendantCount(gn);
        var root = new WorldHierarchyNode("world:root", WorldHierarchyNodeKind.WorldRoot, "世界", null, null, "world", false, groupNodes.AsReadOnly());
        UpdateDescendantCount(root);
        return new WorldHierarchyTree(root, totalNodes, entityCount, entityNodes, ancestorMap);
    }

    static void UpdateDescendantCount(WorldHierarchyNode node)
    { var c = 1; foreach (var ch in node.Children) { UpdateDescendantCount(ch); c += ch.DescendantCount; } node.DescendantCount = c; }

    static string GetGroupKey(string g) => g switch
    { "单位" => "units", "地形" => "terrain", "工事" => "fortifications", "触发器" => "triggers", _ => "other" };
}
