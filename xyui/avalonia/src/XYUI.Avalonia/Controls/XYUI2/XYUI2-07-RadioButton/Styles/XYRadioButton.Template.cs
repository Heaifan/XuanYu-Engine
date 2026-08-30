using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public partial class XYRadioButton
{
    internal static FuncControlTemplate<XYRadioButton> CreateTemplate() => new((control, _) =>
    {
        var halo = new Ellipse { Name = "PART_Halo" };
        halo.Classes.Add("xyui-radio-halo");
        var circle = new Ellipse { Name = "PART_Circle" };
        circle.Classes.Add("xyui-radio-circle");
        var dot = new Ellipse { Width = 6, Height = 6, Name = "PART_Dot" };
        dot.Classes.Add("xyui-radio-dot");
        var marks = new Grid { Name = "PART_IndicatorHost", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        marks.Classes.Add("xyui-radio-host");
        marks.Children.Add(halo); marks.Children.Add(circle); marks.Children.Add(dot);
        var content = new ContentPresenter { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
        content[!ContentPresenter.ContentProperty] = control[!ContentControl.ContentProperty];
        content[!TextElement.ForegroundProperty] = control[!TemplatedControl.ForegroundProperty];
        var root = new StackPanel { Orientation = Orientation.Horizontal, MinHeight = 22, VerticalAlignment = VerticalAlignment.Center };
        root.Children.Add(marks); root.Children.Add(content);
        return root;
    });
}
