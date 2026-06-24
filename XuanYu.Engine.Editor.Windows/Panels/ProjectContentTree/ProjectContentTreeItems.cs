using XuanYu.Engine.Editor.Windows.Panels.HierarchyVisual;
using ProjectContentTreeModel = XuanYu.Engine.Editor.ProjectContentTreeModel;

namespace XuanYu.Engine.Editor.Windows.Panels.ProjectContentTree;

/// <summary>项目内容树视图节点和可见行构造。</summary>
internal static class ProjectContentTreeItems
{
    public static (List<ProjectContentNodeView> Roots, ProjectContentTreeIndex Index) Build(
        ProjectContentTreeModel.ProjectContentTree tree) => ProjectContentTreeIndex.Build(tree);

    public static List<ProjectContentNodeView> BuildVisibleRows(
        List<ProjectContentNodeView> roots) => HierarchyVisibleRows.Build(roots);
}
