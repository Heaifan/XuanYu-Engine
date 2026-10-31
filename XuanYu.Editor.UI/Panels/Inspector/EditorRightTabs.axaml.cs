using System.ComponentModel;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class EditorRightTabs : UserControl
{
    UiVm? _vm;
    readonly XYContextMenu _allTabsMenu;

    public EditorRightTabs()
    {
        InitializeComponent();
        _allTabsMenu = new XYContextMenu { ContextType = "页签", ContextName = "右侧面板" };
        DataContextChanged += (_, _) => SyncDataContext();
    }

    void AllTabsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var items = new[] { ("inspector", "检查器"), ("hierarchy", "层级"), ("debug", "调试") }
            .Select(x => new XYMenuItem { Id = x.Item1, Label = x.Item2, IsChecked = x.Item1 == SideTabs.SelectedTabId });
        _allTabsMenu.Menu = new XYMenu(items.Select(item =>
        {
            item.Action = () => SideTabs.Select(item.Id);
            return item;
        }).ToArray());
        _allTabsMenu.Open(AllTabsButton);
        e.Handled = true;
    }

    void SideTabs_SelectionChanged(object? sender, XYTab tab)
    {
        ApplyTab(tab.Id);
        if (_vm is not null) _vm.RightTabIndex = IndexOf(tab.Id);
    }

    void SyncDataContext()
    {
        if (_vm is not null) _vm.PropertyChanged -= OnVmPropertyChanged;
        _vm = DataContext as UiVm;
        if (_vm is not null) { _vm.PropertyChanged += OnVmPropertyChanged; SelectIndex(_vm.RightTabIndex); }
    }

    void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UiVm.RightTabIndex) && _vm is not null) SelectIndex(_vm.RightTabIndex);
    }

    void SelectIndex(int index)
    {
        var id = index switch { 1 => "hierarchy", 2 => "debug", _ => "inspector" };
        SideTabs.SelectedTabId = id; ApplyTab(id);
    }

    void ApplyTab(string id)
    {
        InspectorWorkspace.IsVisible = id == "inspector";
        HierarchyWorkspace.IsVisible = id == "hierarchy";
        RightWorkspaceHost.IsVisible = true;
        DebugWorkspace.IsVisible = id == "debug";
    }

    static int IndexOf(string id) => id switch { "hierarchy" => 1, "debug" => 2, _ => 0 };
}
