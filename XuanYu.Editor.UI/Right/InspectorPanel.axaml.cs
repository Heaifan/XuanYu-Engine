using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4-F1：检查器只读键值行始终单行双列（ReadonlyKeyValueRow），
// 无模式切换（可编辑表单的 <360 上下布局仅适用于真实输入控件，见 EditableFormLayoutModel）。
public partial class InspectorPanel : UserControl
{
    public static FuncValueConverter<string?, bool> IsTechnicalField { get; } =
        new(label => label is "数据集 ID" or "实体编号" or "路径");

    public static FuncValueConverter<string?, bool> IsStandardField { get; } =
        new(label => label is not ("数据集 ID" or "实体编号" or "路径"));

    public InspectorPanel()
    {
        InitializeComponent();
        AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
    }

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
