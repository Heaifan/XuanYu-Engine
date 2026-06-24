namespace XuanYu.Engine.Editor.Windows.Panels.WorldHierarchy;

/// <summary>
/// 保存节点树的展开/选择状态，用于树重建时恢复。
/// </summary>
public sealed class WorldHierarchyTreeViewState
{
    /// <summary>已展开的 NodeId 集合。</summary>
    public HashSet<string> ExpandedNodeIds { get; } = [];

    /// <summary>当前选中实体的 EntityId。</summary>
    public string? SelectedEntityId { get; set; }

    /// <summary>当前搜索文本。</summary>
    public string SearchText { get; set; } = string.Empty;
}
