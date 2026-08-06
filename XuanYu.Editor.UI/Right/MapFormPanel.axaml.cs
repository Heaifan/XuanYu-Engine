using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4-F1（纠偏 v2）/D5：地图属性输入表单。
// EditableFormRow 方向切换（<360 整组上下）；字段级校验接线（失焦校验 + 提交后聚焦第一处错误）。
// 只控制输入表单方向与校验接线，不参与面板密度（MapEditorLayoutModel）。
public partial class MapFormPanel : UserControl
{
    public MapFormPanel()
    {
        InitializeComponent();
        SizeChanged += (_, _) => ApplyMode();
        DataContextChanged += (_, _) => HookVm();
        ApplyMode();
    }

    void HookVm()
    {
        if (DataContext is UiVm vm)
            vm.PropertyChanged += OnVmPropertyChanged;
    }

    void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(UiVm.FirstInvalidField)) return;
        if (DataContext is not UiVm vm) return;
        var target = vm.FirstInvalidField switch
        {
            "宽度" => PropsWide.IsVisible ? WidthBoxWide : WidthBoxNarrow,
            "深度" => PropsWide.IsVisible ? DepthBoxWide : DepthBoxNarrow,
            "基础高度" => PropsWide.IsVisible ? HeightBoxWide : HeightBoxNarrow,
            _ => null
        };
        target?.Focus();
    }

    // ValidateOnLostFocus：失焦校验单个字段（不干扰其他字段）
    void Field_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (sender is not TextBox box || DataContext is not UiVm vm) return;
        var field = box == WidthBoxWide || box == WidthBoxNarrow ? "宽度"
            : box == DepthBoxWide || box == DepthBoxNarrow ? "深度" : "基础高度";
        var text = box.Text ?? "";
        vm.ValidateMapField(field, text, out _);
    }

    void ApplyMode()
    {
        var narrow = EditableFormLayoutModel.ModeFor(Bounds.Width) == EditableFormMode.Narrow;
        PropsWide.IsVisible = !narrow;
        PropsNarrow.IsVisible = narrow;
    }
}
