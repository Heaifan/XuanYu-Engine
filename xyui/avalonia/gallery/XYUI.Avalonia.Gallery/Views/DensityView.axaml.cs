using Avalonia.Controls;
using Avalonia.Interactivity;
using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Gallery.Views;

public partial class DensityView : UserControl
{
    public DensityView()
    {
        InitializeComponent();
        Loaded += (_, _) => UpdateScopeStatus();
    }

    void OnToggleLocalDensityClick(object? sender, RoutedEventArgs e)
    {
        var current = XyuiDensityScope.GetMode(DynamicScopeContainer);
        var next = current == XyuiDensityMode.Compact ? XyuiDensityMode.Comfortable : XyuiDensityMode.Compact;
        XyuiDensityScope.SetMode(DynamicScopeContainer, next);
        UpdateScopeStatus();
    }

    void UpdateScopeStatus()
    {
        var mode = XyuiDensityScope.GetMode(DynamicScopeContainer);
        ScopeStatusText.Text = $"当前 Scope 模式: {mode}";
    }
}
