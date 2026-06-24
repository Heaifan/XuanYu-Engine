using XuanYu.Engine.Editor.Windows.Panels.HierarchyVisual;
using ProjectContentTreeType = XuanYu.Engine.Editor.ProjectContentTreeModel.ProjectContentTree;
using ProjectContentTreeNodeType = XuanYu.Engine.Editor.ProjectContentTreeModel.ProjectContentTreeNode;

namespace XuanYu.Engine.Editor.Windows.Panels.ProjectContentTree;

public sealed class ProjectContentTreeIndex
{
    public Dictionary<string, ProjectContentNodeView>
        NodeViewsById { get; } = [];

    public Dictionary<string, ProjectContentNodeView>
        FileViewsByPath { get; } = [];

    public static (
        List<ProjectContentNodeView> Roots,
        ProjectContentTreeIndex Index)
        Build(ProjectContentTreeType tree)
    {
        var index = new ProjectContentTreeIndex();

        var root = BuildView(
            tree.Root,
            index,
            depth: 0,
            isLast: true,
            []);

        return ([root], index);
    }

    private static ProjectContentNodeView BuildView(
        ProjectContentTreeNodeType node,
        ProjectContentTreeIndex index,
        int depth,
        bool isLast,
        bool[] ancestorHasNext)
    {
        var view = new ProjectContentNodeView(node)
        {
            BranchInfo = new HierarchyBranchInfo(
                depth,
                isLast,
                ancestorHasNext)
        };

        index.NodeViewsById[node.NodeId] = view;

        if (node.RelativePath is not null)
            index.FileViewsByPath[node.RelativePath] = view;

        AddChildren(
            node,
            view,
            index,
            depth,
            isLast,
            ancestorHasNext);

        return view;
    }

    private static void AddChildren(
        ProjectContentTreeNodeType node,
        ProjectContentNodeView view,
        ProjectContentTreeIndex index,
        int depth,
        bool isLast,
        bool[] ancestorHasNext)
    {
        for (var i = 0; i < node.Children.Count; i++)
        {
            var childIsLast = i == node.Children.Count - 1;
            var childLines = new bool[depth + 1];

            Array.Copy(
                ancestorHasNext,
                childLines,
                Math.Min(depth, ancestorHasNext.Length));

            childLines[depth] = !isLast;

            view.Children.Add(
                BuildView(
                    node.Children[i],
                    index,
                    depth + 1,
                    childIsLast,
                    childLines));
        }
    }
}
