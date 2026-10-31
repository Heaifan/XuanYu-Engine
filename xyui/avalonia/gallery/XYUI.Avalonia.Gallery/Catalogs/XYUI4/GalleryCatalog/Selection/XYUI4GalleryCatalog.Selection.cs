using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI4GalleryCatalog
{
    static Control SelectedPreview() => new StackPanel { Spacing = 8, Children =
    {
        StateItem("普通对象", false), StateItem("当前选中对象", true),
        new TextBlock { Text = "Selected 在指针离开后保持，不等同 Hover。" }
    } };

    static Control ActivePreview() => new StackPanel { Spacing = 8, Children =
    {
        new ToggleButton { Content = "当前工具：移动", IsChecked = true, Classes = { "xyui-active" }, Width = 180 },
        new Button { Content = "按压反馈", Classes = { "xyui-interactive" }, Width = 180 },
        new TextBlock { Text = "Persistent Active 与 Pressed 分离。" }
    } };

    static Control FocusPreview() => new StackPanel { Spacing = 8, Children =
    {
        new Button { Content = "键盘焦点入口", Classes = { "xyui-interactive", "xyui-focusable" }, Width = 180 },
        FocusField("输入焦点"),
        new TextBlock { Text = "Focus Ring 独立于 Selected Surface。" }
    } };

    static Control SelectedLiveExample() => new StackPanel { Spacing = 8, Children = { StateItem("图层 A", true), StateItem("图层 B", false) } };
    static Control ActiveLiveExample() => new ToggleButton { Content = "移动工具已激活", IsChecked = true, Classes = { "xyui-active" }, Width = 180 };
    static Control FocusLiveExample() => new StackPanel { Spacing = 8, Children = { new Button { Content = "Tab 到这里", Classes = { "xyui-interactive", "xyui-focusable" }, Width = 180 }, FocusField("可编辑字段") } };

    static TextBox FocusField(string text)
    {
        var field = new TextBox { Text = text, Classes = { "xyui-interactive", "xyui-focusable" }, Width = 180 };
        field.GotFocus += (_, _) => field.SelectAll();
        return field;
    }

    static ListBoxItem StateItem(string text, bool selected) => new()
    {
        Content = text, IsSelected = selected, Width = 180,
        Classes = { "xyui-interactive", "xyui-focusable", "xyui-selectable" }
    };
}
