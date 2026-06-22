namespace FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

/// <summary>世界层级树展开/折叠状态管理。</summary>
sealed class WorldHierarchyTreeExpansion
{
    readonly WorldHierarchyTreeViewState _state;

    public WorldHierarchyTreeExpansion(WorldHierarchyTreeViewState state) => _state = state;

    public void Toggle(WorldHierarchyNodeView node)
    {
        node.IsExpanded = !node.IsExpanded;
        if (node.IsExpanded)
            _state.ExpandedNodeIds.Add(node.NodeId);
        else
            _state.ExpandedNodeIds.Remove(node.NodeId);
    }

    public void Restore(WorldHierarchyTreeIndex index)
    {
        if (_state.ExpandedNodeIds.Count == 0)
        {
            foreach (var view in index.NodeViewsById.Values)
                if (view.HasChildren)
                    _state.ExpandedNodeIds.Add(view.NodeId);
        }

        foreach (var view in index.NodeViewsById.Values)
            view.IsExpanded = view.HasChildren && _state.ExpandedNodeIds.Contains(view.NodeId);
    }

    public void ExpandAncestors(string entityId, WorldHierarchyTreeIndex? index)
    {
        if (index?.AncestorViewsByEntityId.TryGetValue(entityId, out var ancestors) == true)
        {
            foreach (var ancestor in ancestors)
            {
                ancestor.IsExpanded = true;
                _state.ExpandedNodeIds.Add(ancestor.NodeId);
            }
        }
    }
}
