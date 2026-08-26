using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYSplitButton
{
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
        var main = new Button { Name = "PART_MainZone", Focusable = false,
            HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
        main.Classes.Add("xyui-split-main");
        main[!ContentControl.ContentProperty] = control[!ContentControl.ContentProperty];
        main[!Button.CommandProperty] = control[!MainCommandProperty];
        main[!Button.CommandParameterProperty] = control[!MainCommandParameterProperty];
        main[!TemplatedControl.ForegroundProperty] = control[!TemplatedControl.ForegroundProperty];
        main[!TemplatedControl.PaddingProperty] = control[!TemplatedControl.PaddingProperty];
        Grid.SetColumn(main, 0); grid.Children.Add(main); scope?.Register("PART_MainZone", main);
        var divider = new Border { Name = "PART_Divider", Width = 1, Height = DividerHeight,
            HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center,
            IsHitTestVisible = false };
        divider.Classes.Add("xyui-split-divider"); Grid.SetColumn(divider, 1);
        grid.Children.Add(divider); scope?.Register("PART_Divider", divider);
        var menu = new Button { Name = "PART_MenuZone", Focusable = false,
            HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center,
            Content = new XYIcon { Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Medium } };
        menu.Classes.Add("xyui-split-menu");
        menu[!Button.CommandProperty] = control[!MenuCommandProperty];
        menu[!Button.CommandParameterProperty] = control[!MenuCommandParameterProperty];
        var chevron = (XYIcon)menu.Content!;
        chevron[!XYIcon.StrokeProperty] = menu[!TemplatedControl.ForegroundProperty];
        Grid.SetColumn(menu, 2); grid.Children.Add(menu); scope?.Register("PART_MenuZone", menu);
        root.Child = grid;
        return root;
    });
}
