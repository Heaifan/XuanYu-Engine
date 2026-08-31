using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYBreadcrumbItem : Border
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYBreadcrumbItem, string>(nameof(Label), "");
    public static readonly StyledProperty<bool> IsCurrentProperty = AvaloniaProperty.Register<XYBreadcrumbItem, bool>(nameof(IsCurrent));
    public static readonly StyledProperty<bool> IsCollapsedProperty = AvaloniaProperty.Register<XYBreadcrumbItem, bool>(nameof(IsCollapsed));
    public static readonly StyledProperty<bool> HasDropdownProperty = AvaloniaProperty.Register<XYBreadcrumbItem, bool>(nameof(HasDropdown));
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public bool IsCurrent { get => GetValue(IsCurrentProperty); set => SetValue(IsCurrentProperty, value); }
    public bool IsCollapsed { get => GetValue(IsCollapsedProperty); set => SetValue(IsCollapsedProperty, value); }
    public bool HasDropdown { get => GetValue(HasDropdownProperty); set => SetValue(HasDropdownProperty, value); }
    public IReadOnlyList<string> DropdownOptions { get; set; } = [];
    public IReadOnlyList<string> HiddenPathOptions { get; set; } = [];

    public XYBreadcrumbItem() { Classes.Add("xyui-breadcrumb-item"); Focusable = true; Build(); InitializeInteraction(); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    { base.OnPropertyChanged(change); if (change.Property != ChildProperty) Build(); }

    void Build()
    {
        Classes.Set("xyui-breadcrumb-current", IsCurrent);
        Classes.Set("xyui-breadcrumb-collapsed", IsCollapsed);
        if (IsCollapsed)
        {
            Child = new XYIcon { Icon = XyuiVectorIcon.MoreHorizontal, Size = XyuiIconSize.Small, Classes = { "xyui-breadcrumb-ellipsis" } };
            return;
        }
        var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };
            panel.Children.Add(new TextBlock { Text = Label, Classes = { "xyui-breadcrumb-label" }, VerticalAlignment = VerticalAlignment.Center });
        if (HasDropdown) panel.Children.Add(new XYIcon { Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Tiny, Classes = { "xyui-breadcrumb-dropdown" } });
        Child = panel;
    }
}
