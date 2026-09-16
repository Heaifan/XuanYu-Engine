using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.ComponentModel;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4-F1：检查器只读键值行始终单行双列（ReadonlyKeyValueRow），
// 无模式切换（可编辑表单的 <360 上下布局仅适用于真实输入控件，见 EditableFormLayoutModel）。
public partial class InspectorPanel : UserControl
{
    XYNavigationState? _inspectorRailState;
    public static FuncValueConverter<string?, bool> IsTechnicalField { get; } =
        new(label => label is "数据集 ID" or "实体编号" or "路径");

    public static FuncValueConverter<string?, bool> IsStandardField { get; } =
        new(label => label is not ("数据集 ID" or "实体编号" or "路径"));

    public InspectorPanel()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => AttachInspectorViewModel();
        AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
    }

    void AttachInspectorViewModel()
    {
        if (DataContext is UiVm vm) { vm.PropertyChanged += OnInspectorViewModelChanged; RefreshInspectorRail(vm); }
    }

    void OnInspectorViewModelChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is UiVm vm && e.PropertyName is nameof(UiVm.InspectorCategories) or nameof(UiVm.InspectorCategory))
            RefreshInspectorRail(vm);
    }

    void RefreshInspectorRail(UiVm vm)
    {
        var entries = vm.InspectorCategories.Select(category => new XYNavigationEntry(category, category, IconFor(category))).ToArray();
        if (_inspectorRailState is null || !_inspectorRailState.Entries.Select(x => x.Id).SequenceEqual(entries.Select(x => x.Id)))
        {
            _inspectorRailState = new XYNavigationState(entries, vm.InspectorCategory);
            _inspectorRailState.NavigationRequested += OnRailNavigationRequested;
            InspectorNavigationRail.NavigationState = _inspectorRailState;
        }
        else _inspectorRailState.Select(vm.InspectorCategory);
    }

    void OnRailNavigationRequested(object? sender, XYNavigationRequest request)
    {
        if (DataContext is UiVm vm) vm.SelectInspectorCategoryCommand.Execute(request.Destination.Id);
    }

    static XyuiVectorIcon IconFor(string category) => category switch
    {
        "最近" => XyuiVectorIcon.Clock, "基础" => XyuiVectorIcon.Section,
        "几何" => XyuiVectorIcon.Move, "状态" => XyuiVectorIcon.StatusDot,
        "关联" => XyuiVectorIcon.Tag, "其他" => XyuiVectorIcon.MoreHorizontal,
        _ => XyuiVectorIcon.Section
    };

    void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not UiVm vm) return;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Control) && e.Key == Key.F)
        { SearchBox.Focus(); SearchBox.SelectAll(); e.Handled = true; return; }
        if (e.Key == Key.Escape && vm.IsInspectorSearchMode) { vm.InspectorSearchText = ""; e.Handled = true; return; }
        if (vm.IsInspectorSearchMode && e.Key is Key.Up or Key.Down or Key.Enter)
        {
            if (e.Key == Key.Enter) vm.ActivateInspectorSearchResult();
            else vm.MoveInspectorSearch(e.Key == Key.Up ? -1 : 1);
            e.Handled = true;
        }
    }

    void PropertyEditor_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is XYUI.Avalonia.Controls.XYTextField field && field.DataContext is InspectorPropertyRow row && DataContext is UiVm vm)
            vm.CommitInspectorProperty(row.Key, field.Text ?? "");
    }

    void PropertyEditor_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        PropertyEditor_LostFocus(sender, new RoutedEventArgs());
        e.Handled = true;
    }

    public static FuncValueConverter<int, bool> IsZero { get; } = new(value => value == 0);
}
