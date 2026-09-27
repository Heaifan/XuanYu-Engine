using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI4GalleryCatalog
{
    static Control MultiSelectionPreview() => new StackPanel { Spacing = 8, Children =
    {
        MultiList("Primary + Secondary"),
        new TextBlock { Text = "Primary 更强；Secondary 保持 Selected 身份。" }
    } };

    static Control SelectionGroupPreview() => GroupSurface("选择组 · 图层 A / 图层 B", MultiList("组成员"));
    static Control MultiSelectionLiveExample() => MultiList("可操作多选");
    static Control SelectionGroupLiveExample() => GroupSurface("地图对象组", MultiList("成员 1 / 成员 2"));

    static Control MultiList(string label)
    {
        var list = new ListBox { SelectionMode = SelectionMode.Multiple, Width = 220, Height = 92, Classes = { "xyui-multi-selection" } };
        foreach (var text in new[] { "Primary 对象", "Secondary 对象 A", "Secondary 对象 B" }) list.Items.Add(new ListBoxItem { Content = text, Classes = { "xyui-interactive", "xyui-selectable" } });
        list.SelectedItems!.Add(list.Items[0]!); list.SelectedItems!.Add(list.Items[1]!);
        return new StackPanel { Spacing = 4, Children = { new TextBlock { Text = label }, list } };
    }

    static Control GroupSurface(string header, Control content) => new Border
    {
        BorderBrush = Application.Current?.FindResource("XY.Brush.Editor.Selection") as IBrush,
        BorderThickness = new Thickness(2), Padding = new Thickness(8), Child = new StackPanel { Spacing = 6, Children = { new TextBlock { Text = header }, content } }
    };
}
