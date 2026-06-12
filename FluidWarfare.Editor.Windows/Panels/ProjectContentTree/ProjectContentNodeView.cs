using System.ComponentModel;
using FluidWarfare.Editor.ProjectContentTreeModel;

namespace FluidWarfare.Editor.Windows.Panels.ProjectContentTree;

/// <summary>
/// 项目内容树的数据绑定节点视图模型。
/// </summary>
public sealed class ProjectContentNodeView : INotifyPropertyChanged
{
    private bool _isExpanded;

    public ProjectContentNodeView(ProjectContentTreeNode node)
    {
        Node = node;
    }

    public ProjectContentTreeNode Node { get; }
    public string NodeId => Node.NodeId;
    public string DisplayName => Node.DisplayName;

    public string DisplayNameWithBranch
    {
        get
        {
            if (BranchInfo is null || BranchInfo.Depth <= 0) return DisplayName;
            var prefix = BranchInfo.IsLastSibling ? "└─ " : "├─ ";
            return prefix + DisplayName;
        }
    }

    public string? RelativePath => Node.RelativePath;
    public bool IsSelectable => Node.IsSelectable;
    public bool IsFile => Node.Kind == ProjectContentTreeNodeKind.ContentFile;

    /// <summary>从 NodeId 提取相对路径。</summary>
    public string? FileRelativePath =>
        IsFile && NodeId.StartsWith("file:") ? NodeId["file:".Length..] : null;

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value) return;
            _isExpanded = value;
            OnPropertyChanged(nameof(IsExpanded));
        }
    }

    public List<ProjectContentNodeView> Children { get; } = [];

    /// <summary>树干分支信息（由树索引构建时设置）。</summary>
    public FluidWarfare.Editor.Windows.Panels.HierarchyVisual.HierarchyBranchInfo? BranchInfo { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
