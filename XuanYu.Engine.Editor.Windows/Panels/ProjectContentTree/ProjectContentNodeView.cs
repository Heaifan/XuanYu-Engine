using System.ComponentModel;
using XuanYu.Engine.Editor.ProjectContentTreeModel;
using FluidWarfare.Editor.Windows.Panels.HierarchyVisual;

namespace FluidWarfare.Editor.Windows.Panels.ProjectContentTree;

public sealed class ProjectContentNodeView : IHierarchyNodeView
{
    bool _isExpanded;
    public ProjectContentNodeView(ProjectContentTreeNode node) { Node = node; _isExpanded = node.Children.Count > 0; }
    public ProjectContentTreeNode Node { get; }
    public string NodeId => Node.NodeId;
    public string DisplayName => Node.DisplayName;
    public string? SecondaryText => null;
    public string ToolTipText => Node.RelativePath ?? Node.DisplayName;
    public bool HasSecondaryText => false;
    public bool IsSelectable => Node.IsSelectable;
    public bool IsFile => Node.Kind == ProjectContentTreeNodeKind.ContentFile;
    public bool HasChildren => Children.Count > 0;
    public string? FileRelativePath => IsFile && NodeId.StartsWith("file:") ? NodeId["file:".Length..] : null;
    public string NodeIconPath => Node.Kind switch
    {
        ProjectContentTreeNodeKind.ProjectRoot => "/Assets/Icons/Hierarchy/project.svg",
        ProjectContentTreeNodeKind.ContentFolder => ResolveFolderIcon(),
        ProjectContentTreeNodeKind.ContentFile => ResolveFileIcon(),
        _ => "/Assets/Icons/Hierarchy/file.svg"
    };
    public string ToggleIconPath => IsExpanded ? "/Assets/Icons/Hierarchy/toggle-minus.svg" : "/Assets/Icons/Hierarchy/toggle-plus.svg";
    public bool IsExpanded { get => _isExpanded; set { if (_isExpanded == value) return; _isExpanded = value; OnPropertyChanged(nameof(IsExpanded)); OnPropertyChanged(nameof(NodeIconPath)); OnPropertyChanged(nameof(ToggleIconPath)); } }
    public List<ProjectContentNodeView> Children { get; } = [];
    public IEnumerable<IHierarchyNodeView> ChildNodes => Children;
    public HierarchyBranchInfo? BranchInfo { get; set; }
    public double BranchGuideWidth => BranchInfo is { Depth: > 0 } info ? info.Depth * 18.0 : 0.0;
    public event PropertyChangedEventHandler? PropertyChanged;

    string ResolveFolderIcon() => Node.NodeId switch
    {
        "folder:factions" => "/Assets/Icons/Hierarchy/faction.svg", "folder:units" => "/Assets/Icons/Hierarchy/units.svg",
        "folder:weapons" => "/Assets/Icons/Hierarchy/weapon.svg", "folder:maps" => "/Assets/Icons/Hierarchy/map.svg",
        "folder:scripts" => "/Assets/Icons/Hierarchy/script.svg", "folder:rules" => "/Assets/Icons/Hierarchy/rule.svg",
        "folder:icons" => "/Assets/Icons/Hierarchy/image.svg",
        _ => IsExpanded ? "/Assets/Icons/Hierarchy/folder-open.svg" : "/Assets/Icons/Hierarchy/folder-closed.svg"
    };
    string ResolveFileIcon() => (Path.GetExtension(Node.RelativePath ?? "")?.ToLowerInvariant()) switch
    {
        ".json" => "/Assets/Icons/Hierarchy/file-json.svg",
        ".svg" or ".png" or ".jpg" or ".jpeg" or ".webp" => "/Assets/Icons/Hierarchy/image.svg",
        ".cs" or ".lua" or ".js" or ".ts" => "/Assets/Icons/Hierarchy/script.svg",
        _ => "/Assets/Icons/Hierarchy/file.svg"
    };
    void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}
