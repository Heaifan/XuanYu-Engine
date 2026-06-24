using FluidWarfare.Editor.Windows.Panels.HierarchyVisual;
using XuanYu.Engine.Editor.WorldHierarchy;

namespace FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

public sealed class WorldHierarchyTreeIndex
{
    public Dictionary<string, WorldHierarchyNodeView> NodeViewsById { get; } = [];
    public Dictionary<string, WorldHierarchyNodeView> EntityViewsById { get; } = [];
    public Dictionary<string, List<WorldHierarchyNodeView>> AncestorViewsByEntityId { get; } = [];

    public static (List<WorldHierarchyNodeView> Roots, WorldHierarchyTreeIndex Index) Build(WorldHierarchyTree tree)
    {
        var index = new WorldHierarchyTreeIndex();
        var root = BuildView(tree.Root, null, index, [], 0, true, []);
        return ([root], index);
    }

    static WorldHierarchyNodeView BuildView(WorldHierarchyNode node, WorldHierarchyNodeView? parent,
        WorldHierarchyTreeIndex index, List<WorldHierarchyNodeView> ancestorPath,
        int depth, bool isLast, bool[] ancestorHasNext)
    {
        var view = new WorldHierarchyNodeView(node, parent)
        { BranchInfo = new HierarchyBranchInfo(depth, isLast, ancestorHasNext) };
        index.NodeViewsById[node.NodeId] = view;
        if (view.EntityId is not null)
        { index.EntityViewsById[view.EntityId] = view; index.AncestorViewsByEntityId[view.EntityId] = [.. ancestorPath]; }
        AddChildren(node, view, index, ancestorPath, depth, isLast, ancestorHasNext);
        return view;
    }

    static void AddChildren(WorldHierarchyNode node, WorldHierarchyNodeView view,
        WorldHierarchyTreeIndex index, List<WorldHierarchyNodeView> ancestorPath,
        int depth, bool isLast, bool[] ancestorHasNext)
    {
        for (var i = 0; i < node.Children.Count; i++)
        {
            var childIsLast = i == node.Children.Count - 1;
            var childLines = new bool[depth + 1];
            Array.Copy(ancestorHasNext, childLines, Math.Min(depth, ancestorHasNext.Length));
            childLines[depth] = !isLast;
            var childPath = new List<WorldHierarchyNodeView>(ancestorPath) { view };
            view.Children.Add(BuildView(node.Children[i], view, index, childPath, depth + 1, childIsLast, childLines));
        }
    }
}
