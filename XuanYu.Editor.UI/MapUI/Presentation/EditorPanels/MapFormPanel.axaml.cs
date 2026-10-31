using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4-F1（纠偏 v2）/D5：地图属性 Property Grid。
// 固定 96,* 单行布局；字段级校验接线（失焦校验 + 提交后聚焦第一处错误）。
public partial class MapFormPanel : UserControl
{
    public MapFormPanel()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (DataContext is UiVm vm)
                vm.PropertyChanged += OnVmPropertyChanged;
        };
    }

    void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(UiVm.FirstInvalidField)) return;
        if (DataContext is not UiVm vm) return;
        var target = vm.FirstInvalidField switch
        {
            "宽度" => WidthBoxWide,
            "深度" => DepthBoxWide,
            "基础高度" => HeightBoxWide,
            _ => null
        };
        target?.Focus();
    }

    // ValidateOnLostFocus：失焦校验单个字段（不干扰其他字段）
    void Field_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is not TextBox box || DataContext is not UiVm vm) return;
        var field = box == WidthBoxWide ? "宽度"
            : box == DepthBoxWide ? "深度" : "基础高度";
        var text = box.Text ?? "";
        vm.ValidateMapField(field, text, out _);
    }
}
