using System.ComponentModel;
using XuanYu.Engine.Editor.Windows.Panels.HierarchyVisual;
using XuanYu.Engine.Editor.WorldHierarchy;

namespace XuanYu.Engine.Editor.Windows.Panels.WorldHierarchy;

public sealed class WorldHierarchyNodeView : IHierarchyNodeView
{
    private bool _isExpanded;

    public WorldHierarchyNodeView(
        WorldHierarchyNode node,
        WorldHierarchyNodeView? parent)
    {
        Node = node;
        Parent = parent;
        _isExpanded = node.Children.Count > 0;
    }

    public WorldHierarchyNode Node { get; }
    public WorldHierarchyNodeView? Parent { get; }
    public string NodeId => Node.NodeId;
    public string? EntityId => Node.EntityId?.Value.ToString();
    public string DisplayName => Node.DisplayName;
    public string? SecondaryText => null;
    public string ToolTipText => Node.SecondaryText ?? Node.DisplayName;
    public bool HasSecondaryText => false;
    public bool IsSelectable => Node.IsSelectable;
    public bool HasChildren => Children.Count > 0;
    public bool IsLeaf => !HasChildren;

    public string NodeIconPath =>
        Node.Kind switch
        {
            WorldHierarchyNodeKind.WorldRoot =>
                "/Assets/Icons/Hierarchy/world.svg",
            WorldHierarchyNodeKind.EntityGroup =>
                ResolveGroupIcon(),
            WorldHierarchyNodeKind.Entity =>
                "/Assets/Icons/Hierarchy/unit-entity.svg",
            _ => "/Assets/Icons/Hierarchy/file.svg"
        };

    public string ToggleIconPath =>
        IsExpanded
            ? "/Assets/Icons/Hierarchy/toggle-minus.svg"
            : "/Assets/Icons/Hierarchy/toggle-plus.svg";

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value)
                return;

            _isExpanded = value;
            OnPropertyChanged(nameof(IsExpanded));
            OnPropertyChanged(nameof(NodeIconPath));
            OnPropertyChanged(nameof(ToggleIconPath));
        }
    }

    public List<WorldHierarchyNodeView> Children { get; } = [];
    public IEnumerable<IHierarchyNodeView> ChildNodes => Children;
    public HierarchyBranchInfo? BranchInfo { get; set; }

    public double BranchGuideWidth =>
        BranchInfo is { Depth: > 0 } info
            ? info.Depth * 18.0
            : 0.0;

    public event PropertyChangedEventHandler? PropertyChanged;

    private string ResolveGroupIcon() =>
        Node.NodeId switch
        {
            "group:units" => "/Assets/Icons/Hierarchy/units.svg",
            "group:terrain" => "/Assets/Icons/Hierarchy/map.svg",
            "group:fortifications" => "/Assets/Icons/Hierarchy/folder-closed.svg",
            "group:triggers" => "/Assets/Icons/Hierarchy/rule.svg",
            _ => IsExpanded
                ? "/Assets/Icons/Hierarchy/folder-open.svg"
                : "/Assets/Icons/Hierarchy/folder-closed.svg"
        };

    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(name));
}
