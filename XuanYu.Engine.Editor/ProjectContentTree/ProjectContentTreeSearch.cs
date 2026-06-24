namespace FluidWarfare.Editor.ProjectContentTreeModel;

/// <summary>
/// 在项目内容树中搜索匹配节点。
/// 搜索范围：目录显示名、文件显示名、相对路径、内容类型。
/// 不区分大小写，匹配时保留祖先路径。
/// </summary>
public static class ProjectContentTreeSearch
{
    /// <summary>
    /// 搜索项目内容树，返回保留祖先路径的子树。
    /// 未命中返回仅含 Root 的树。清空查询返回 null（恢复完整树）。
    /// </summary>
    public static ProjectContentTreeNode? Search(ProjectContentTree tree, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return null;

        var lowerQuery = query.Trim().ToLowerInvariant();
        var matchedFolders = new List<(ProjectContentTreeNode Folder, List<ProjectContentTreeNode> Hits)>();

        foreach (var folder in tree.Root.Children)
        {
            if (folder.Kind != ProjectContentTreeNodeKind.ContentFolder) continue;

            var hits = new List<ProjectContentTreeNode>();
            foreach (var file in folder.Children)
            {
                if (Matches(file, lowerQuery))
                    hits.Add(file);
            }

            // 目录名本身匹配也显示该目录
            if (hits.Count > 0 || MatchesFolder(folder, lowerQuery))
            {
                matchedFolders.Add((folder, hits));
            }
        }

        if (matchedFolders.Count == 0)
        {
            return new ProjectContentTreeNode(
                "project:empty", ProjectContentTreeNodeKind.ProjectRoot,
                tree.Root.DisplayName, null, null, "project", false, []);
        }

        var folderNodes = matchedFolders.Select(m =>
        {
            var children = m.Hits.Count > 0
                ? m.Hits.OrderBy(f => f.DisplayName).ToList().AsReadOnly()
                : m.Folder.Children;
            var displayName = m.Hits.Count > 0 && m.Hits.Count != m.Folder.Children.Count
                ? $"{GetBaseDisplayName(m.Folder.DisplayName)} ({m.Hits.Count})"
                : m.Folder.DisplayName;
            return new ProjectContentTreeNode(
                m.Folder.NodeId, ProjectContentTreeNodeKind.ContentFolder,
                displayName, null, m.Folder.ContentType, "folder", false, children);
        }).ToList().AsReadOnly();

        return new ProjectContentTreeNode(
            tree.Root.NodeId, ProjectContentTreeNodeKind.ProjectRoot,
            tree.Root.DisplayName, null, null, "project", false, folderNodes);
    }

    private static bool Matches(ProjectContentTreeNode node, string lowerQuery)
    {
        if (node.DisplayName.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase))
            return true;
        if (node.RelativePath is not null &&
            node.RelativePath.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase))
            return true;
        if (node.ContentType is not null &&
            node.ContentType.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }

    private static bool MatchesFolder(ProjectContentTreeNode folder, string lowerQuery)
    {
        return GetBaseDisplayName(folder.DisplayName).Contains(lowerQuery, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetBaseDisplayName(string displayName)
    {
        var parenIndex = displayName.LastIndexOf('(');
        return parenIndex > 0 ? displayName[..parenIndex].Trim() : displayName;
    }
}
