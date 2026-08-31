using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class EditorRightTabs : UserControl
{
    XYTabs? _tabs;
    Grid? _viewsContainer;
    Decorator? _tabsContainer;

    public EditorRightTabs()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _tabsContainer = this.FindControl<Decorator>("TabsContainer");
        _viewsContainer = this.FindControl<Grid>("ViewsContainer");

        if (_tabsContainer != null && _tabs == null)
        {
            var vm = DataContext as UiVm;
            var tab1 = new XYTab { Label = "检查器" };
            var tab2 = new XYTab { Label = "调试" };
            _tabs = new XYTabs(tab1, tab2);
            _tabs.SelectionChanged += OnTabSelectionChanged;
            _tabsContainer.Child = _tabs;
            int initialIndex = vm?.RightTabIndex ?? 0;
            if (initialIndex >= 0 && initialIndex < _tabs.Items.Count)
            {
                _tabs.Items[initialIndex].IsSelected = true;
                UpdateContent(initialIndex);
            }
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_tabs != null)
        {
            _tabs.SelectionChanged -= OnTabSelectionChanged;
        }
    }

    private void OnTabSelectionChanged(object? sender, XYTab tab)
    {
        if (_tabs == null) return;
        var index = -1;
        for (int i = 0; i < _tabs.Items.Count; i++)
        {
            if (_tabs.Items[i] == tab)
            {
                index = i;
                break;
            }
        }
        if (index >= 0)
        {
            UpdateContent(index);
            if (DataContext is UiVm vm)
            {
                vm.RightTabIndex = index;
            }
        }
    }

    private void UpdateContent(int index)
    {
        if (_viewsContainer == null) return;
        for (int i = 0; i < _viewsContainer.Children.Count; i++)
        {
            var view = _viewsContainer.Children[i];
            view.IsVisible = (i == index);
        }
    }
}
