using FluidWarfare.Core.Identity;
using FluidWarfare.Engine.World;

namespace FluidWarfare.Editor.WorldHierarchy;

/// <summary>
/// 从 WorldState 构建 WorldHierarchyTree。
/// 需要外部提供 EntityId → 分组名的映射（由 Editor 协调层从 RenderScene 生成）。
/// 排序稳定：分组按语义顺序，实体按 DisplayName + EntityId。
/// </summary>
public static class WorldHierarchyTreeBuilder
{
    private static readonly string[] GroupOrder = ["地形", "单位", "工事", "触发器", "其他"];

    /// <summary>
    /// 构建层级树。
    /// </summary>
    /// <param name="worldState">World 状态。</param>
    /// <param name="entityGroupLookup">EntityId → 分组中文名映射（如"单位""地形"），缺失则归为"其他"。</param>
    public static WorldHierarchyTree Build(
        WorldState worldState,
        IReadOnlyDictionary<EntityId, string>? entityGroupLookup = null)
    {
        var entities = worldState.ListEntities();
        if (entities.Count == 0)
            return WorldHierarchyTree.Empty;

        // 按分组归类
        var groups = new Dictionary<string, List<WorldEntityInfo>>();
        foreach (var entity in entities)
        {
            var groupName = entityGroupLookup?.GetValueOrDefault(entity.EntityId) ?? "其他";
            if (!groups.ContainsKey(groupName))
                groups[groupName] = [];
            groups[groupName].Add(entity);
        }

        // 排序分组
        var sortedGroups = groups
            .OrderBy(g => Array.IndexOf(GroupOrder, g.Key) >= 0 ? Array.IndexOf(GroupOrder, g.Key) : int.MaxValue)
            .ThenBy(g => g.Key)
            .ToList();

        var entityNodes = new Dictionary<string, WorldHierarchyNode>();
        var ancestorMap = new Dictionary<string, IReadOnlyList<string>>();
        var groupNodes = new List<WorldHierarchyNode>();
        var totalNodes = 1; // Root
        var entityCount = 0;

        foreach (var (groupName, items) in sortedGroups)
        {
            if (items.Count == 0) continue;

            var groupNodeId = $"group:{GetGroupKey(groupName)}";
            var childNodes = new List<WorldHierarchyNode>();

            // 实体排序：DisplayName + EntityId 稳定
            var sortedItems = items
                .OrderBy(x => x.DisplayName, StringComparer.Ordinal)
                .ThenBy(x => x.EntityId.Value)
                .ToList();

            foreach (var info in sortedItems)
            {
                var entityNodeId = $"entity:{info.EntityId.Value}";
                var node = new WorldHierarchyNode(
                    entityNodeId,
                    WorldHierarchyNodeKind.Entity,
                    info.DisplayName,
                    info.Source?.RelativePath,
                    info.EntityId,
                    "entity",
                    true,
                    []);
                childNodes.Add(node);
                entityNodes[info.EntityId.Value.ToString()] = node;
                entityCount++;

                // 祖先路径：[root, group]
                ancestorMap[info.EntityId.Value.ToString()] = new List<string> { "world:root", groupNodeId };
            }

            var groupNode = new WorldHierarchyNode(
                groupNodeId,
                WorldHierarchyNodeKind.EntityGroup,
                $"{groupName} ({items.Count})",
                null, null, "group", false,
                childNodes.AsReadOnly());
            groupNodes.Add(groupNode);
            totalNodes += 1 + childNodes.Count;
        }

        // 更新 DescendantCount
        foreach (var gn in groupNodes)
            UpdateDescendantCount(gn);

        var root = new WorldHierarchyNode(
            "world:root", WorldHierarchyNodeKind.WorldRoot,
            "World", null, null, "world", false,
            groupNodes.AsReadOnly());
        UpdateDescendantCount(root);

        return new WorldHierarchyTree(root, totalNodes, entityCount,
            entityNodes, ancestorMap);
    }

    private static void UpdateDescendantCount(WorldHierarchyNode node)
    {
        var count = 1;
        foreach (var child in node.Children)
        {
            UpdateDescendantCount(child);
            count += child.DescendantCount;
        }
        node.DescendantCount = count;
    }

    private static string GetGroupKey(string groupName) => groupName switch
    {
        "单位" => "units",
        "地形" => "terrain",
        "工事" => "fortifications",
        "触发器" => "triggers",
        _ => "other"
    };
}
