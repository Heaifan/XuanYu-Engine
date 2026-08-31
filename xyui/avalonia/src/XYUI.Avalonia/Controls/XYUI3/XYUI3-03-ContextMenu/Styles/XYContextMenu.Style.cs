using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYContextMenu
{
    Control Header() => new Border { Classes = { "xyui-context-header" }, Child = new StackPanel { Children = { new TextBlock { Text = ContextType, Classes = { "xyui-context-type" }, VerticalAlignment = VerticalAlignment.Center }, new TextBlock { Text = ContextName, Classes = { "xyui-context-name" }, VerticalAlignment = VerticalAlignment.Center } } } };
}
