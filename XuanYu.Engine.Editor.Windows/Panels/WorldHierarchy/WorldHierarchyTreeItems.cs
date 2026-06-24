using XuanYu.Engine.Editor.Windows.Panels.HierarchyVisual;
using XuanYu.Engine.Editor.WorldHierarchy;

namespace XuanYu.Engine.Editor.Windows.Panels.WorldHierarchy;

/// <summary>世界层级树视图节点和可见行构造。</summary>
internal static class WorldHierarchyTreeItems
{
    public static (List<WorldHierarchyNodeView> Roots, WorldHierarchyTreeIndex Index) Build(
        WorldHierarchyTree tree) => WorldHierarchyTreeIndex.Build(tree);

    public static List<WorldHierarchyNodeView> BuildVisibleRows(
        List<WorldHierarchyNodeView> roots) => HierarchyVisibleRows.Build(roots);
}
