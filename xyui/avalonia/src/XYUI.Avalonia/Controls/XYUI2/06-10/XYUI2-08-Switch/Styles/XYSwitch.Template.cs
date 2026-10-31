using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public partial class XYSwitch
{
    internal static FuncControlTemplate<XYSwitch> CreateTemplate() => new((control, _) =>
    {
        var track = new Border { Name = "PART_Track" };
        track.Classes.Add("xyui-switch-track");
        var thumb = new Ellipse { Width = 14, Height = 14, Name = "PART_Thumb", Margin = new Thickness(2, 0, 0, 0), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
        thumb.Classes.Add("xyui-switch-thumb");
        var trackHost = new Grid { Name = "PART_TrackHost" }; trackHost.Classes.Add("xyui-switch-host"); trackHost.Children.Add(track); trackHost.Children.Add(thumb);
        var content = new ContentPresenter { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
        content[!ContentPresenter.ContentProperty] = control[!ContentControl.ContentProperty];
        content[!TextElement.ForegroundProperty] = control[!TemplatedControl.ForegroundProperty];
        var root = new StackPanel { Orientation = Orientation.Horizontal, MinHeight = 22, VerticalAlignment = VerticalAlignment.Center };
        root.Children.Add(trackHost); root.Children.Add(content);
        return root;
    });
}
