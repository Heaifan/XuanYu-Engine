namespace FluidWarfare.Editor.ProjectContentTreeModel;

/// <summary>
/// 项目内容树的只读快照。
/// 持有 RelativePath → FileNode 索引，用于 O(1) 查找。
/// </summary>
public sealed record ProjectContentTree(
    ProjectContentTreeNode Root,
    int NodeCount,
    int FolderCount,
    int FileCount,
    IReadOnlyDictionary<string, ProjectContentTreeNode> FileNodes)
{
    public static readonly ProjectContentTree Empty = new(
        new ProjectContentTreeNode(
            "project:empty", ProjectContentTreeNodeKind.ProjectRoot,
            "未加载项目", null, null, "project", false, []),
        1, 0, 0,
        new Dictionary<string, ProjectContentTreeNode>());

    public ProjectContentTreeNode? FindByRelativePath(string relativePath)
    {
        var normalized = relativePath.Replace('\\', '/');
        return FileNodes.TryGetValue(normalized, out var node) ? node : null;
    }
}
