using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

// XYUI-2-04 Split Button（方案 2 · Compact Icon Well）。
// 一个完整 Button Chrome + 两个独立 Hit Zone：
//   Main 区执行主命令；Menu 区只触发菜单命令（二者互不串发）。
// 菜单区是固定宽度的紧凑图标槽；两区共享一层 Chrome，不使用永久 Action Edge。
public partial class XYSplitButton : ContentControl
{
    // Canonical COMPONENT_SPECIFIC 尺寸（数值真源 = XYUI-2.canonical.md · AMEND-C）。
    public const double MenuZoneWidth = 34; // XY.SplitButton.MenuZone.Width
    public const double DividerHeight = 18; // XY.SplitButton.Divider.Height

    public XYSplitButton()
    {
        Classes.Add("xyui-split-button");
        Focusable = true;
        Template = CreateTemplate();
    }

    public static readonly StyledProperty<ICommand?> MainCommandProperty =
        AvaloniaProperty.Register<XYSplitButton, ICommand?>(nameof(MainCommand));

    public ICommand? MainCommand
    {
        get => GetValue(MainCommandProperty);
        set => SetValue(MainCommandProperty, value);
    }

    public static readonly StyledProperty<object?> MainCommandParameterProperty =
        AvaloniaProperty.Register<XYSplitButton, object?>(nameof(MainCommandParameter));

    public object? MainCommandParameter
    {
        get => GetValue(MainCommandParameterProperty);
        set => SetValue(MainCommandParameterProperty, value);
    }

    public static readonly StyledProperty<ICommand?> MenuCommandProperty =
        AvaloniaProperty.Register<XYSplitButton, ICommand?>(nameof(MenuCommand));

    public ICommand? MenuCommand
    {
        get => GetValue(MenuCommandProperty);
        set => SetValue(MenuCommandProperty, value);
    }

    public static readonly StyledProperty<object?> MenuCommandParameterProperty =
        AvaloniaProperty.Register<XYSplitButton, object?>(nameof(MenuCommandParameter));

    public object? MenuCommandParameter
    {
        get => GetValue(MenuCommandParameterProperty);
        set => SetValue(MenuCommandParameterProperty, value);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if ((e.Key == Key.Enter || e.Key == Key.Space) && MainCommand?.CanExecute(MainCommandParameter) == true)
        {
            MainCommand.Execute(MainCommandParameter);
            e.Handled = true;
            return;
        }

        base.OnKeyDown(e);
    }
}
