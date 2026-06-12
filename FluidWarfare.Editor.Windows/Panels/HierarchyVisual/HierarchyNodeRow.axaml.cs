using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using FluidWarfare.Editor.Windows.Panels.HierarchyVisual;

namespace FluidWarfare.Editor.Windows.Panels.HierarchyVisual;

/// <summary>
/// 树节点行控件。被世界树和项目树共同复用。
/// 布局：树干线 | 展开箭头 | 类型图标 | 名称 + 副文字
/// </summary>
public sealed partial class HierarchyNodeRow : UserControl, INotifyPropertyChanged
{
    private HierarchyBranchCanvas? _branchCanvas;
    private Image? _arrowImage;
    private Image? _iconImage;
    private TextBlock? _primaryText;
    private TextBlock? _secondaryText;

    private HierarchyBranchInfo? _branchInfo;
    private string? _iconName;
    private bool _canExpand;
    private bool _isExpanded;
    private string _primary = string.Empty;
    private string? _secondary;

    public new event PropertyChangedEventHandler? PropertyChanged;

    public HierarchyNodeRow()
    {
        InitializeComponent();
        _branchCanvas = this.FindControl<HierarchyBranchCanvas>("BranchCanvas");
        _arrowImage = this.FindControl<Image>("ArrowImage");
        _iconImage = this.FindControl<Image>("IconImage");
        _primaryText = this.FindControl<TextBlock>("PrimaryText");
        _secondaryText = this.FindControl<TextBlock>("SecondaryText");
    }

    /// <summary>树干分支信息。</summary>
    public HierarchyBranchInfo? BranchInfo
    {
        get => _branchInfo;
        set
        {
            _branchInfo = value;
            if (_branchCanvas is not null)
                _branchCanvas.BranchInfo = value;
            UpdateBranchWidth();
        }
    }

    /// <summary>图标名称（不含 .svg）。</summary>
    public string? IconName
    {
        get => _iconName;
        set
        {
            _iconName = value;
            UpdateIcons();
        }
    }

    /// <summary>是否有子节点（控制展开箭头显示）。</summary>
    public bool CanExpand
    {
        get => _canExpand;
        set
        {
            _canExpand = value;
            UpdateIcons();
        }
    }

    /// <summary>是否展开（控制箭头方向 + 文件夹图标）。</summary>
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value) return;
            _isExpanded = value;
            UpdateIcons();
        }
    }

    /// <summary>主文字。</summary>
    public string Primary
    {
        get => _primary;
        set
        {
            _primary = value;
            if (_primaryText is not null)
                _primaryText.Text = value;
        }
    }

    /// <summary>副文字。</summary>
    public string? Secondary
    {
        get => _secondary;
        set
        {
            _secondary = value;
            if (_secondaryText is not null)
            {
                _secondaryText.Text = value;
                _secondaryText.IsVisible = value is not null;
            }
        }
    }

    /// <summary>树干缩进总宽度。</summary>
    public double BranchCanvasWidth
    {
        get
        {
            var depth = _branchInfo?.Depth ?? 0;
            return depth * 18.0;
        }
    }

    private void UpdateBranchWidth()
    {
        if (_branchCanvas is not null)
            _branchCanvas.Width = BranchCanvasWidth;
    }

    private void UpdateIcons()
    {
        // 展开箭头
        if (_arrowImage is not null)
        {
            if (_canExpand)
            {
                var arrowName = _isExpanded ? "chevron-down" : "chevron-right";
                _arrowImage.Source = HierarchySvgIcon.GetIcon(arrowName);
                _arrowImage.IsVisible = true;
            }
            else
            {
                _arrowImage.Source = null;
                _arrowImage.IsVisible = true; // 留占位宽
            }
        }

        // 类型图标
        if (_iconImage is not null && _iconName is not null)
        {
            var resolved = HierarchySvgIcon.ResolveIconName(_iconName, _isExpanded);
            _iconImage.Source = HierarchySvgIcon.GetIcon(resolved);
        }
    }

    public void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
