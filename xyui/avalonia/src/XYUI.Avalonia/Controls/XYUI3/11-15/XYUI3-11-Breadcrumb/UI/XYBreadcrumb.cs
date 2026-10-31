using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Metadata;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYBreadcrumb : Border
{
    readonly StackPanel _panel = new() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
    [Content] public ObservableCollection<XYBreadcrumbItem> Items { get; } = [];
    public Popup DropdownPopup { get; } = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };
    public XYBreadcrumb() { Classes.Add("xyui-breadcrumb"); Child = _panel; Items.CollectionChanged += OnItemsChanged; DetachedFromVisualTree += (_, _) => DropdownPopup.IsOpen = false; }
    public XYBreadcrumb(params XYBreadcrumbItem[] items) : this() { foreach (var item in items) Items.Add(item); }
    void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) => Build();
    void Build()
    {
        _panel.Children.Clear();
        for (var index = 0; index < Items.Count; index++)
        { if (index > 0) _panel.Children.Add(new XYIcon { Icon = XyuiVectorIcon.ChevronRight, Size = XyuiIconSize.Tiny, Classes = { "xyui-breadcrumb-separator" }, Margin = new global::Avalonia.Thickness(4, 0) }); Attach(Items[index]); _panel.Children.Add(Items[index]); }
        _panel.Children.Add(DropdownPopup);
    }
}
