using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Vector;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Controls;

// XYUI-2-04 Split Button（方案 2 · Soft Partition · R2）。
// 一个完整 Button Chrome + 两个独立 Hit Zone：
//   Main 区执行主命令；Menu 区只触发菜单命令（二者互不串发）。
// R2（AMEND-C）：菜单区不再整块高亮（避免"第二颗按钮"感），独立反馈只落在 Chevron；
// Divider 更短更淡，仅作软分区提示；Action Edge 为跨全宽的单条共享边。
public class XYSplitButton : ContentControl
{
    // Canonical COMPONENT_SPECIFIC 尺寸（数值真源 = XYUI-2.canonical.md · AMEND-C）。
    public const double MenuZoneWidth = 36; // XY.SplitButton.MenuZone.Width
    public const double DividerHeight = 12; // XY.SplitButton.Divider.Height（R2：18→12，软分区提示）

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

    // 模板：Border(Chrome) → Grid[* | 1 | MenuZone]，Main/Divider/Menu/共享 Edge。
    // 内部按钮 Focusable=false，焦点由 SplitButton 整体承载（FocusRing）。
    static FuncControlTemplate<XYSplitButton> CreateTemplate() => new((control, scope) =>
    {
        var root = new Border { ClipToBounds = true };
        root[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty];
        root[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty];
        root[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty];
        root[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];

        var grid = new Grid { Name = "PART_Grid" };
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1)));
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(MenuZoneWidth)));
        scope?.Register("PART_Grid", grid);

        var main = new Button
        {
            Name = "PART_MainZone",
            Focusable = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment = VerticalAlignment.Center,
        };
        main.Classes.Add("xyui-split-main");
        main[!ContentControl.ContentProperty] = control[!ContentControl.ContentProperty];
        main[!Button.CommandProperty] = control[!MainCommandProperty];
        main[!Button.CommandParameterProperty] = control[!MainCommandParameterProperty];
        main[!TemplatedControl.ForegroundProperty] = control[!TemplatedControl.ForegroundProperty];
        main[!TemplatedControl.PaddingProperty] = control[!TemplatedControl.PaddingProperty];
        Grid.SetColumn(main, 0);
        grid.Children.Add(main);
        scope?.Register("PART_MainZone", main);

        // 短 Divider：垂直居中，不贯穿顶部到底部；不参与命中测试。
        var divider = new Border
        {
            Name = "PART_Divider",
            Width = 1,
            Height = DividerHeight,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsHitTestVisible = false,
        };
        divider.Classes.Add("xyui-split-divider");
        Grid.SetColumn(divider, 1);
        grid.Children.Add(divider);
        scope?.Register("PART_Divider", divider);

        var menu = new Button
        {
            Name = "PART_MenuZone",
            Focusable = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Content = new XYIcon { Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Medium },
        };
        menu.Classes.Add("xyui-split-menu");
        menu[!Button.CommandProperty] = control[!MenuCommandProperty];
        menu[!Button.CommandParameterProperty] = control[!MenuCommandParameterProperty];
        // Soft Partition（R2）：Chevron 描边跟随菜单按钮 Foreground（默认 Text.Secondary），
        // 由 MenuZone 状态样式驱动独立 Hover/Pressed/Disabled 反馈，无需 Descendant 选择器。
        var chevron = (XYIcon)menu.Content!;
        chevron[!VectorPath.StrokeProperty] = menu[!TemplatedControl.ForegroundProperty];
        // 光学对齐（R2.1，最终依据——非"居中"，而是"Chevron 最低点 ≈ 文字字形底边"）：
        // Source Han Sans SC @ FontSize 14 / LineHeight 20 实测（Light + Fluent 等价环境）：
        //   字形底边（TextLayout.Baseline=11.826）相对按钮顶 ≈ 20.83 DIP
        //   Chevron 图形最低点（geometry 12×6 → Uniform 缩放 16×8 居中 + 描边外扩 0.75）相对按钮顶 ≈ 20.75 DIP
        //   差值 +0.08 → TranslateTransform Y=1 DIP，最低点与字形底边持平/微低。
        // 用 RenderTransform 而非 Padding/Margin，不改布局 —— 34 DIP 高度、Hit Area、Divider、Action Edge 均不变；
        // RenderTransform 是控件级属性，Default/MainHover/MenuHover/Disabled 与 Light/Dark 统一生效。
        chevron.RenderTransform = new TranslateTransform(0, 1);
        Grid.SetColumn(menu, 2);
        grid.Children.Add(menu);
        scope?.Register("PART_MenuZone", menu);

        // 整条共享 Action Edge：跨三列铺满 Chrome 底边。
        var edge = new XyuiActionEdge();
        Grid.SetColumnSpan(edge, 3);
        grid.Children.Add(edge);

        root.Child = grid;
        return root;
    });
}
