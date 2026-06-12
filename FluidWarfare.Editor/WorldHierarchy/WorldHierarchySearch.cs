namespace FluidWarfare.Editor.WorldHierarchy;

/// <summary>
/// 在层级树中搜索匹配节点。
/// 搜索范围：DisplayName、EntityId、SecondaryText（Source）。
/// 不区分大小写，匹配时保留祖先路径。
/// </summary>
public static class WorldHierarchySearch
{
    /// <summary>
    /// 在树中搜索匹配的实体节点，返回保留祖先路径的子树。
    /// 未命中时返回仅含 Root 的空树。
    /// 清空查询返回 null（表示恢复完整树）。
    /// </summary>
    public static WorldHierarchyNode? Search(WorldHierarchyTree tree, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return null; // 恢复完整树

        var lowerQuery = query.Trim().ToLowerInvariant();
        var matched = new List<WorldHierarchyNode>();

        foreach (var (entityId, node) in tree.EntityNodes)
        {
            if (Matches(node, lowerQuery))
                matched.Add(node);
        }

        if (matched.Count == 0)
        {
            // 返回仅含 Root 的空状态
            return new WorldHierarchyNode(
                "world:root", WorldHierarchyNodeKind.WorldRoot,
                "World", null, null, "world", false, []);
        }

        // 按祖先分组：每个命中的实体展开其祖先链
        var ancestorGroups = new Dictionary<string, List<WorldHierarchyNode>>();
        foreach (var node in matched)
        {
            var ancestors = tree.GetAncestorNodeIds(entityId: GetEntityId(node));
            var parentId = ancestors is { Count: > 1 } ? ancestors[^1] : "world:root";

            if (!ancestorGroups.ContainsKey(parentId))
                ancestorGroups[parentId] = [];
            ancestorGroups[parentId].Add(node);
        }

        // 构建搜索子树
        var groupChildren = new List<WorldHierarchyNode>();
        foreach (var (parentId, children) in ancestorGroups)
        {
            // 从原始树中找分组节点信息
            var originalGroup = FindNode(tree.Root, parentId);
            var groupName = originalGroup?.DisplayName ?? parentId;
            // 更新数量
            var countStr = groupName.Contains('(')
                ? groupName[..(groupName.LastIndexOf('(') + 1)] + $"{children.Count})"
                : $"{groupName} ({children.Count})";

            var groupNode = new WorldHierarchyNode(
                parentId, WorldHierarchyNodeKind.EntityGroup,
                countStr, null, null, "group", false,
                children.OrderBy(c => c.DisplayName).ToList().AsReadOnly());
            groupChildren.Add(groupNode);
        }

        return new WorldHierarchyNode(
            "world:root", WorldHierarchyNodeKind.WorldRoot,
            "World", null, null, "world", false,
            groupChildren.AsReadOnly());
    }

    private static bool Matches(WorldHierarchyNode node, string lowerQuery)
    {
        if (node.DisplayName.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase))
            return true;
        if (node.SecondaryText is not null &&
            node.SecondaryText.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase))
            return true;
        if (node.EntityId?.Value.ToString().Contains(lowerQuery) == true)
            return true;
        return false;
    }

    private static string GetEntityId(WorldHierarchyNode node) =>
        node.EntityId?.Value.ToString() ?? "";

    private static WorldHierarchyNode? FindNode(WorldHierarchyNode root, string nodeId)
    {
        if (root.NodeId == nodeId) return root;
        foreach (var child in root.Children)
        {
            var found = FindNode(child, nodeId);
            if (found is not null) return found;
        }
        return null;
    }
}
