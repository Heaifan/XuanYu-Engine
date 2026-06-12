using System.ComponentModel;
using FluidWarfare.Editor.WorldHierarchy;

namespace FluidWarfare.Editor.Windows.Panels.WorldHierarchy;

/// <summary>
/// TreeView 数据绑定的节点视图模型。
/// 持有平台无关 WorldHierarchyNode 的引用，不重复存储数据。
/// 实现 INotifyPropertyChanged 以支持 Avalonia 数据绑定。
/// </summary>
public sealed class WorldHierarchyNodeView : INotifyPropertyChanged
{
    private bool _isExpanded;
    private bool _isSelected;

    public WorldHierarchyNodeView(WorldHierarchyNode node, WorldHierarchyNodeView? parent)
    {
        Node = node;
        Parent = parent;
    }

    public WorldHierarchyNode Node { get; }

    public WorldHierarchyNodeView? Parent { get; }

    public string NodeId => Node.NodeId;

    public string? EntityId => Node.EntityId?.Value.ToString();

    public string DisplayName => Node.DisplayName;

    /// <summary>带分支前缀的显示名称（如 "└─ sample_unit"）。</summary>
    public string DisplayNameWithBranch
    {
        get
        {
            if (BranchInfo is null || BranchInfo.Depth <= 0) return DisplayName;
            var prefix = BranchInfo.IsLastSibling ? "└─ " : "├─ ";
            return prefix + DisplayName;
        }
    }

    public string? SecondaryText => Node.SecondaryText;

    public bool IsSelectable => Node.IsSelectable;

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

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        }
    }

    /// <summary>
    /// 子节点视图（由树索引构建时填充）。
    /// </summary>
    public List<WorldHierarchyNodeView> Children { get; } = [];

    /// <summary>树干分支信息（由树索引构建时设置）。</summary>
    public FluidWarfare.Editor.Windows.Panels.HierarchyVisual.HierarchyBranchInfo? BranchInfo { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
