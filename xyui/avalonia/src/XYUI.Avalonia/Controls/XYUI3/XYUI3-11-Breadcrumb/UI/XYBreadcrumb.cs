using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYBreadcrumb : Border
{
    public IReadOnlyList<XYBreadcrumbItem> Items { get; }
    public Popup DropdownPopup { get; } = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };

    public XYBreadcrumb(params XYBreadcrumbItem[] items)
    {
        Classes.Add("xyui-breadcrumb"); Items = items;
        var panel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
        for (var index = 0; index < items.Length; index++)
        {
            if (index > 0) panel.Children.Add(new XYIcon
            {
                Icon = XyuiVectorIcon.ChevronRight,
                Size = XyuiIconSize.Tiny,
                Classes = { "xyui-breadcrumb-separator" },
                Margin = new global::Avalonia.Thickness(4, 0)
            });
            Attach(items[index]); panel.Children.Add(items[index]);
        }
        panel.Children.Add(DropdownPopup);
        Child = panel;
        DetachedFromVisualTree += (_, _) => DropdownPopup.IsOpen = false;
    }
}
