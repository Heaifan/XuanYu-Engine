namespace XuanYu.Engine.Editor.Windows.Panels.ProjectContentTree;

/// <summary>项目内容树展开/折叠状态管理。</summary>
sealed class ProjectContentTreeExpansion
{
    readonly HashSet<string> _expandedNodeIds;

    public ProjectContentTreeExpansion(HashSet<string> expandedNodeIds) => _expandedNodeIds = expandedNodeIds;

    public void Toggle(ProjectContentNodeView node)
    {
        node.IsExpanded = !node.IsExpanded;
        if (node.IsExpanded)
            _expandedNodeIds.Add(node.NodeId);
        else
            _expandedNodeIds.Remove(node.NodeId);
    }

    public void Restore(ProjectContentTreeIndex index)
    {
        if (_expandedNodeIds.Count == 0)
        {
            foreach (var view in index.NodeViewsById.Values)
                if (view.HasChildren)
                    _expandedNodeIds.Add(view.NodeId);
        }

        foreach (var view in index.NodeViewsById.Values)
            view.IsExpanded = view.HasChildren && _expandedNodeIds.Contains(view.NodeId);
    }
}
