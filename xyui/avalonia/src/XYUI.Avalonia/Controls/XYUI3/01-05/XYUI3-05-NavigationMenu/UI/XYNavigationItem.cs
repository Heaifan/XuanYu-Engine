using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationItem : Border
{
    public static readonly StyledProperty<string> IdProperty = AvaloniaProperty.Register<XYNavigationItem, string>(nameof(Id), "");
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYNavigationItem, string>(nameof(Label), "");
    public static readonly StyledProperty<XyuiVectorIcon> IconProperty = AvaloniaProperty.Register<XYNavigationItem, XyuiVectorIcon>(nameof(Icon), XyuiVectorIcon.Info);
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<XYNavigationItem, bool>(nameof(IsSelected));
    public static readonly StyledProperty<bool> IsIconOnlyProperty = AvaloniaProperty.Register<XYNavigationItem, bool>(nameof(IsIconOnly));
    public static readonly StyledProperty<XyuiNavigationLayoutVariant> LayoutVariantProperty = AvaloniaProperty.Register<XYNavigationItem, XyuiNavigationLayoutVariant>(nameof(LayoutVariant));
    public static readonly StyledProperty<string?> BadgeProperty = AvaloniaProperty.Register<XYNavigationItem, string?>(nameof(Badge));
    public static readonly StyledProperty<XyuiStatusState> StatusProperty = AvaloniaProperty.Register<XYNavigationItem, XyuiStatusState>(nameof(Status));
    public string Id { get => GetValue(IdProperty); set => SetValue(IdProperty, value); }
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public XyuiVectorIcon Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public bool IsIconOnly { get => GetValue(IsIconOnlyProperty); set => SetValue(IsIconOnlyProperty, value); }
    public XyuiNavigationLayoutVariant LayoutVariant { get => GetValue(LayoutVariantProperty); set => SetValue(LayoutVariantProperty, value); }
    public string? Badge { get => GetValue(BadgeProperty); set => SetValue(BadgeProperty, value); }
    public XyuiStatusState Status { get => GetValue(StatusProperty); set => SetValue(StatusProperty, value); }
    public XYNavigationItem() { Classes.Add("xyui-navigation-item"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (change.Property != ChildProperty) Build(); }
    void Build()
    {
        Classes.Set("xyui-navigation-selected", IsSelected);
        Classes.Set("xyui-navigation-workspace-item", LayoutVariant == XyuiNavigationLayoutVariant.Workspace);
        Child = LayoutVariant == XyuiNavigationLayoutVariant.Workspace ? WorkspaceVisual() : IsIconOnly ? CompactVisual() : FullVisual();
        HookInteraction();
    }
    Grid CompactVisual() => new() { ColumnDefinitions = new ColumnDefinitions("3,*,3"), Children = { new Border { Classes = { "xyui-navigation-accent" } }, IconView() } };
    Grid FullVisual() => new() { ColumnDefinitions = new ColumnDefinitions("3,Auto,*,Auto"), Children = { new Border { Classes = { "xyui-navigation-accent" } }, IconView(), Content(Label, false), StatusView() } };
    XYIcon IconView() => new() { Icon = Icon, Size = XyuiIconSize.Small, Classes = { "xyui-navigation-icon" }, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, [Grid.ColumnProperty] = 1 };
    TextBlock Content(string? text, bool centered) => new() { Text = text, Classes = { "xyui-navigation-label" }, [Grid.ColumnProperty] = centered ? 1 : 2, HorizontalAlignment = centered ? HorizontalAlignment.Center : HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Center };
    Grid WorkspaceVisual()
    {
        var body = new StackPanel { Spacing = 2, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Children = { WorkspaceIcon(), WorkspaceLabel() } };
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("3,*") };
        var mark = new Border { Classes = { "xyui-navigation-accent" }, VerticalAlignment = VerticalAlignment.Stretch };
        var status = StatusView(); status.HorizontalAlignment = HorizontalAlignment.Right; status.VerticalAlignment = VerticalAlignment.Top;
        grid.Children.Add(mark); grid.Children.Add(body); grid.Children.Add(status); Grid.SetColumn(body, 1); Grid.SetColumn(status, 1);
        return grid;
    }
    XYIcon WorkspaceIcon() => new() { Icon = Icon, Size = XyuiIconSize.Small, Classes = { "xyui-navigation-icon" }, HorizontalAlignment = HorizontalAlignment.Center };
    TextBlock WorkspaceLabel() => new() { Text = Label, Classes = { "xyui-navigation-label" }, HorizontalAlignment = HorizontalAlignment.Center, TextAlignment = TextAlignment.Center };
    Control StatusView() => Badge is not null ? new XYStatusBadge { Text = Badge, State = Status, Classes = { "xyui-navigation-badge" }, [Grid.ColumnProperty] = 3 } : new Border { [Grid.ColumnProperty] = 3 };
}
