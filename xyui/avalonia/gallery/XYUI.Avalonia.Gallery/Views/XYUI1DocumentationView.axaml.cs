using Avalonia.Controls;
using Avalonia.Input;

namespace XYUI.Avalonia.Gallery.Views;

public partial class XYUI1DocumentationView : UserControl
{
    public XYUI1DocumentationView()
    {
        InitializeComponent();
        DataContext = new XYUI1DocumentationViewModel();
    }

    // G0-R1 · 整行标题点击 = 展开/折叠（不影响右侧当前页面）
    private void OnToggleXyui1(object? sender, TappedEventArgs e)
    {
        if (DataContext is XYUI1DocumentationViewModel vm)
            vm.IsXYUI1Expanded = !vm.IsXYUI1Expanded;
    }

    private void OnToggleXyui2(object? sender, TappedEventArgs e)
    {
        if (DataContext is XYUI1DocumentationViewModel vm)
            vm.IsXYUI2Expanded = !vm.IsXYUI2Expanded;
    }
}
