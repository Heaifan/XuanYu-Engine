using Avalonia.Controls;
using Avalonia.Media;
using FluidWarfare.Editor.ProjectContentTreeModel;

namespace FluidWarfare.Editor.Windows.Panels.ProjectContentTree;

public sealed partial class ProjectContentTreePanel : UserControl
{
    private TreeView? _treeView;
    private ProjectContentTreeModel.ProjectContentTree? _currentTree;

    /// <summary>文件选择请求事件（相对路径）。</summary>
    public event Action<string?>? ContentSelectionRequested;

    public ProjectContentTreePanel()
    {
        InitializeComponent();
        _treeView = this.FindControl<TreeView>("ContentTree");
        if (_treeView is not null)
            _treeView.SelectionChanged += OnSelectionChanged;
    }

    public void ShowContentTree(ProjectContentTreeModel.ProjectContentTree tree)
    {
        _currentTree = tree;
        Rebuild(tree.Root);
    }

    public void ApplySearchFilter(ProjectContentTreeModel.ProjectContentTreeNode? filteredRoot)
    {
        Rebuild(filteredRoot ?? _currentTree?.Root);
    }

    private void Rebuild(ProjectContentTreeModel.ProjectContentTreeNode? root)
    {
        _treeView?.Items.Clear();
        if (root is null) return;

        var rootItem = BuildTreeItem(root);
        _treeView?.Items.Add(rootItem);
        rootItem.IsExpanded = true;
    }

    private TreeViewItem BuildTreeItem(ProjectContentTreeModel.ProjectContentTreeNode node)
    {
        var item = new TreeViewItem();

        var iconText = node.IconKind switch
        {
            "project" => "\U0001F4C1", // 📁
            "folder" => "\U0001F4C2",  // 📂
            "file" => "\U0001F4C4",    // 📄
            _ => ""
        };

        var displayPanel = new StackPanel { Margin = new Avalonia.Thickness(4, 2) };
        displayPanel.Children.Add(new TextBlock
        {
            Text = string.IsNullOrEmpty(iconText) ? node.DisplayName : $"{iconText} {node.DisplayName}",
            FontSize = 13
        });

        if (!string.IsNullOrEmpty(node.RelativePath))
        {
            displayPanel.Children.Add(new TextBlock
            {
                Text = node.RelativePath,
                Foreground = new SolidColorBrush(Color.FromRgb(0x88, 0x88, 0x88)),
                FontSize = 11
            });
        }

        item.Header = displayPanel;
        item.Tag = node.NodeId;

        foreach (var child in node.Children)
            item.Items.Add(BuildTreeItem(child));

        return item;
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedItem = _treeView?.SelectedItem;
        if (selectedItem is TreeViewItem tvi && tvi.Tag is string nodeId)
        {
            if (!nodeId.StartsWith("file:"))
                return;

            var relativePath = nodeId["file:".Length..];
            System.Diagnostics.Debug.WriteLine($"[ProjectTree] file selected: {relativePath}");
            ContentSelectionRequested?.Invoke(relativePath);
        }
    }
}
