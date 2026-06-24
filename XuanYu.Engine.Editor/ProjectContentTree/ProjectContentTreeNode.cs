namespace XuanYu.Engine.Editor.ProjectContentTreeModel;

/// <summary>
/// 项目内容树节点。
/// NodeId 格式：project:&lt;Name&gt; / folder:&lt;FolderId&gt; / file:&lt;NormalizedRelativePath&gt;
/// 文件节点身份使用标准化相对路径，不使用显示名称。
/// </summary>
public sealed record ProjectContentTreeNode(
    string NodeId,
    ProjectContentTreeNodeKind Kind,
    string DisplayName,
    string? RelativePath,
    string? ContentType,
    string? IconKind,
    bool IsSelectable,
    IReadOnlyList<ProjectContentTreeNode> Children);
