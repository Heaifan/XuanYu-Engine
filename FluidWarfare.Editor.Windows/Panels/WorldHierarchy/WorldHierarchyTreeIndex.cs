using FluidWarfare.Editor.Windows.Panels.HierarchyVisual;
using FluidWarfare.Editor.WorldHierarchy;

namespace FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

/// <summary>
/// 树索引：EntityId → NodeView（O(1)）+ EntityId → 祖先路径（O(1)）。
/// 构建时一次性生成，选择定位不遍历整棵树。
/// </summary>
public sealed class WorldHierarchyTreeIndex
{
    public Dictionary<string, WorldHierarchyNodeView> EntityViewsById { get; } = [];
    public Dictionary<string, List<WorldHierarchyNodeView>> AncestorViewsByEntityId { get; } = [];

    /// <summary>
    /// 从 WorldHierarchyTree 构建索引与根节点列表。
    /// </summary>
    public static (List<WorldHierarchyNodeView> Roots, WorldHierarchyTreeIndex Index) Build(WorldHierarchyTree tree)
    {
        var index = new WorldHierarchyTreeIndex();
        var roots = new List<WorldHierarchyNodeView>();

        var rootView = BuildView(tree.Root, null, index, [], 0, true, []);
        roots.Add(rootView);

        return (roots, index);
    }

    private static WorldHierarchyNodeView BuildView(
        WorldHierarchyNode node,
        WorldHierarchyNodeView? parent,
        WorldHierarchyTreeIndex index,
        List<WorldHierarchyNodeView> ancestorPath,
        int depth,
        bool isLast,
        bool[] ancestorHasNext)
    {
        var view = new WorldHierarchyNodeView(node, parent);

        view.BranchInfo = new HierarchyBranchInfo(depth, isLast, ancestorHasNext);

        if (node.EntityId?.Value is not null)
        {
            var entityId = node.EntityId.Value.ToString();
            index.EntityViewsById[entityId] = view;
            index.AncestorViewsByEntityId[entityId] = [.. ancestorPath];
        }

        var childCount = node.Children.Count;
        for (var i = 0; i < childCount; i++)
        {
            var child = node.Children[i];
            var childIsLast = i == childCount - 1;
            var childAncestorHasNext = new bool[depth + 1];
            Array.Copy(ancestorHasNext, childAncestorHasNext, depth);
            childAncestorHasNext[depth] = !isLast;

            var childPath = new List<WorldHierarchyNodeView>(ancestorPath) { view };
            var childView = BuildView(child, view, index, childPath,
                depth + 1, childIsLast, childAncestorHasNext);
            view.Children.Add(childView);
        }

        return view;
    }
}
