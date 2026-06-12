using FluidWarfare.Editor.ProjectContentTreeModel;
using FluidWarfare.Editor.Windows.Panels.HierarchyVisual;

namespace FluidWarfare.Editor.Windows.Panels.ProjectContentTree;

/// <summary>
/// 项目内容树索引，提供 RelativePath → 节点视图的 O(1) 查找。
/// 构建时同时生成树干分支信息。
/// </summary>
public sealed class ProjectContentTreeIndex
{
    public Dictionary<string, ProjectContentNodeView> FileViewsByPath { get; } = [];

    public static (List<ProjectContentNodeView> Roots, ProjectContentTreeIndex Index) Build(ProjectContentTreeModel.ProjectContentTree tree)
    {
        var index = new ProjectContentTreeIndex();
        var roots = new List<ProjectContentNodeView>();

        var rootView = BuildView(tree.Root, index, 0, true, []);
        roots.Add(rootView);

        return (roots, index);
    }

    private static ProjectContentNodeView BuildView(
        ProjectContentTreeModel.ProjectContentTreeNode node,
        ProjectContentTreeIndex index,
        int depth, bool isLast, bool[] ancestorHasNext)
    {
        var view = new ProjectContentNodeView(node);
        view.BranchInfo = new HierarchyBranchInfo(depth, isLast, ancestorHasNext);

        if (node.RelativePath is not null)
            index.FileViewsByPath[node.RelativePath] = view;

        var childCount = node.Children.Count;
        for (var i = 0; i < childCount; i++)
        {
            var child = node.Children[i];
            var childIsLast = i == childCount - 1;
            var childAncestorHasNext = new bool[depth + 1];
            Array.Copy(ancestorHasNext, childAncestorHasNext, depth);
            childAncestorHasNext[depth] = !isLast;

            var childView = BuildView(child, index, depth + 1, childIsLast, childAncestorHasNext);
            view.Children.Add(childView);
        }

        return view;
    }
}
