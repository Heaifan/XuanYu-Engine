using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Utilities;

namespace XYUI.Avalonia.Controls;

// XYUI-2-05 DropDown Button（方案 4 · Chevron Track）。
// 整钮只有唯一点击区（PART_OpenZone 横跨全钮）：任何位置单击都只触发 OpenCommand；
// 右侧 Chevron Track 是纯装饰视觉槽（IsHitTestVisible=false，不产生第二套行为）；
// 与 SplitButton 的正式分界是无 Divider。菜单本体不属本组件（未造 Popup 系统）。
public partial class XYDropDownButton : ContentControl
{
    // 视觉槽宽度：家族节奏实现常量；Canonical 05 仅定义 Height=34 DIP，
    // 未定义槽宽 token——按用户指示不新增 Component-Specific Token，验收后如有裁定再入 Canonical。
    public const double ChevronTrackWidth = XyuiComponentTokens.DropDownButtonChevronTrackWidth;

    public XYDropDownButton()
    {
        Classes.Add("xyui-dropdown-button");
        Focusable = true;
        Template = CreateTemplate();
    }

    public static readonly StyledProperty<ICommand?> OpenCommandProperty =
        AvaloniaProperty.Register<XYDropDownButton, ICommand?>(nameof(OpenCommand));

    public ICommand? OpenCommand
    {
        get => GetValue(OpenCommandProperty);
        set => SetValue(OpenCommandProperty, value);
    }

    public static readonly StyledProperty<object?> OpenCommandParameterProperty =
        AvaloniaProperty.Register<XYDropDownButton, object?>(nameof(OpenCommandParameter));

    public object? OpenCommandParameter
    {
        get => GetValue(OpenCommandParameterProperty);
        set => SetValue(OpenCommandParameterProperty, value);
    }

    // Chevron 颜色契约：值由样式层按状态供给（Normal=Text.Secondary / Disabled=Disabled.Text），
    // 模板仅负责向图标绑定，避免装饰色参与选择器竞争。
    public static readonly StyledProperty<IBrush?> ChevronBrushProperty =
        AvaloniaProperty.Register<XYDropDownButton, IBrush?>(nameof(ChevronBrush));

    public IBrush? ChevronBrush
    {
        get => GetValue(ChevronBrushProperty);
        set => SetValue(ChevronBrushProperty, value);
    }

    // 自管理禁用态伪类：装饰槽与 chevron 的衰减样式锚定 :ddb-off，
    // 规避「基线样式与禁用变体同时命中时的规则竞争」。
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsEnabledProperty)
            PseudoClasses.Set(":ddb-off", e.GetNewValue<bool>());
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if ((e.Key == Key.Enter || e.Key == Key.Space) && OpenCommand?.CanExecute(OpenCommandParameter) == true)
        {
            OpenCommand.Execute(OpenCommandParameter);
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }
}
