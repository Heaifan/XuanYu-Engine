using System.ComponentModel;

namespace XuanYu.Engine.Editor.Windows.Panels.HierarchyVisual;

public interface IHierarchyNodeView : INotifyPropertyChanged
{
    string NodeId { get; }
    string DisplayName { get; }
    string? SecondaryText { get; }
    string ToolTipText { get; }
    string NodeIconPath { get; }
    string ToggleIconPath { get; }
    bool HasChildren { get; }
    bool HasSecondaryText { get; }
    bool IsExpanded { get; set; }
    bool IsSelectable { get; }
    double BranchGuideWidth { get; }
    HierarchyBranchInfo? BranchInfo { get; }
    IEnumerable<IHierarchyNodeView> ChildNodes { get; }
}

public static class HierarchyVisibleRows
{
    public static List<TNode> Build<TNode>(IEnumerable<TNode> roots)
        where TNode : class, IHierarchyNodeView
    {
        var rows = new List<TNode>();

        foreach (var root in roots)
            Append(root, rows);

        return rows;
    }

    private static void Append<TNode>(TNode node, List<TNode> rows)
        where TNode : class, IHierarchyNodeView
    {
        rows.Add(node);

        if (!node.IsExpanded)
            return;

        foreach (var child in node.ChildNodes)
        {
            if (child is TNode typedChild)
                Append(typedChild, rows);
        }
    }
}
