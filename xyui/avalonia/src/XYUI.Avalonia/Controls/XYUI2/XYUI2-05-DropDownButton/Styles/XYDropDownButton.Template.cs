using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Utilities;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYDropDownButton
{
    internal static FuncControlTemplate<XYDropDownButton> CreateTemplate() => new((control, scope) =>
    {
        var root = new Border { ClipToBounds = true };
        root[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty];
        root[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty];
        root[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty];
        root[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
        var grid = new Grid { Name = "PART_Grid" };
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(ChevronTrackWidth)));
        scope?.Register("PART_Grid", grid);

        // 唯一命中区横跨两列：Chevron 槽区域点击同样落在本区，保证单语义。
        var zone = new Button { Name = "PART_OpenZone", Focusable = false,
            HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
        Grid.SetColumnSpan(zone, 2);
        zone.Classes.Add("xyui-ddb-zone");
        zone[!ContentControl.ContentProperty] = control[!ContentControl.ContentProperty];
        zone[!Button.CommandProperty] = control[!OpenCommandProperty];
        zone[!Button.CommandParameterProperty] = control[!OpenCommandParameterProperty];
        zone[!TemplatedControl.ForegroundProperty] = control[!TemplatedControl.ForegroundProperty];
        zone[!TemplatedControl.PaddingProperty] = control[!TemplatedControl.PaddingProperty];
        grid.Children.Add(zone); scope?.Register("PART_OpenZone", zone);

        // 装饰槽叠于命中区之上且不可命中；chevron 颜色完全交给样式层
        //（Text.Secondary 基线 / Disabled 衰减），避免本地绑定压制样式优先级。
        var track = new Border { Name = "PART_ChevronTrack",
            HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Stretch,
            IsHitTestVisible = false };
        Grid.SetColumn(track, 1);
        track.Classes.Add("xyui-ddb-track");
        var chevron = new XYIcon { Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Medium,
            VerticalAlignment = VerticalAlignment.Center };
        chevron[!XYIcon.StrokeProperty] = control[!ChevronBrushProperty];
        track.Child = chevron;
        grid.Children.Add(track); scope?.Register("PART_ChevronTrack", track);

        // 控件级伪类接线：命中区覆盖全钮，pointer 状态由 zone 驱动、按下经隧道广播；
        // Track 自身不可命中，样式端据此以控件级伪类同步槽底色。
        zone.PointerEntered += (_, _) => control.PseudoClasses.Set(":pointerover", true);
        zone.PointerExited += (_, _) => control.PseudoClasses.Set(":pointerover", false);
        grid.AddHandler(InputElement.PointerPressedEvent,
            (_, _) => control.PseudoClasses.Set(":pressed", true), RoutingStrategies.Tunnel);
        grid.AddHandler(InputElement.PointerReleasedEvent,
            (_, _) => control.PseudoClasses.Set(":pressed", false), RoutingStrategies.Tunnel);

        var edge = new XyuiActionEdge();
        Grid.SetColumnSpan(edge, 2);
        grid.Children.Add(edge);
        root.Child = grid;
        return root;
    });
}
