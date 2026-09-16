using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenu
{
    Grid BuildVisualHost(StackPanel items)
    {
        var host = new Grid();
        Child = null;
        host.Children.Add(items);
        if (!AutoMountSubMenus) return host;
        foreach (var submenu in Items.OfType<XYMenuItem>()
                     .Select(item => item.SubMenu).OfType<XYSubMenu>())
        {
            if (submenu.Parent is Panel parent) parent.Children.Remove(submenu);
            host.Children.Add(submenu);
        }
        return host;
    }
}
