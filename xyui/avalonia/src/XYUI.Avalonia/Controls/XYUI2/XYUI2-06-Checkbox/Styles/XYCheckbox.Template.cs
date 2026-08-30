using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Controls;

public partial class XYCheckbox
{
    internal static FuncControlTemplate<XYCheckbox> CreateTemplate() => new((control, _) =>
    {
        var indicator = new Grid { Name = "PART_IndicatorHost", Width = 18, Height = 22, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        indicator.Classes.Add("xyui-checkbox-host"); Grid.SetColumn(indicator, 0);
        var box = new Border { Name = "PART_Box", ClipToBounds = true, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        box.Classes.Add("xyui-checkbox-box");
        var glyphHost = new Grid { Name = "PART_GlyphHost", Width = 14, Height = 14, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        glyphHost.Classes.Add("xyui-checkbox-glyph-host");
        var check = new VectorPath { Name = "PART_Check", Width = 14, Height = 14, Data = StreamGeometry.Parse("M3 6.8 L5.8 9.6 L10.8 4.4"), StrokeThickness = 1.25, StrokeLineCap = PenLineCap.Round, StrokeJoin = PenLineJoin.Round, Stretch = Stretch.None, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        check.Classes.Add("xyui-checkbox-check");
        var mixed = new Border { Name = "PART_Mixed", Width = 7, Height = 1.25, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        mixed.Classes.Add("xyui-checkbox-mixed");
        glyphHost.Children.Add(check); glyphHost.Children.Add(mixed); box.Child = glyphHost; indicator.Children.Add(box);
        var content = new ContentPresenter { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left };
        content[!ContentPresenter.ContentProperty] = control[!ContentControl.ContentProperty];
        content[!TextElement.ForegroundProperty] = control[!TemplatedControl.ForegroundProperty];
        Grid.SetColumn(content, 2);
        var root = new Grid { MinHeight = 22, VerticalAlignment = VerticalAlignment.Center, ColumnDefinitions = new ColumnDefinitions("18,7,*") };
        root.Children.Add(indicator); root.Children.Add(content);
        return root;
    });
}
