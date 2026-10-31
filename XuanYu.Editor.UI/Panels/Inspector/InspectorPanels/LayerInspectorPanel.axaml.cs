using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D4：图层检查器（名称 Enter/失焦提交；开关/按钮走绑定，无额外逻辑）。
public partial class LayerInspectorPanel : UserControl
{
    public LayerInspectorPanel()
    {
        InitializeComponent();
    }

    async void NameBox_LostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => await CommitNameAsync();

    void NameBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { _ = CommitNameAsync(); e.Handled = true; }
    }

    async Task CommitNameAsync()
    {
        if (DataContext is UiVm vm) await vm.CommitLayerRenameAsync(vm.LayerInspectorNameText);
    }
}
