using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationItem : Border
{
    public static readonly StyledProperty<string> IdProperty = AvaloniaProperty.Register<XYNavigationItem, string>(nameof(Id), "");
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYNavigationItem, string>(nameof(Label), "");
    public static readonly StyledProperty<XyuiVectorIcon> IconProperty = AvaloniaProperty.Register<XYNavigationItem, XyuiVectorIcon>(nameof(Icon), XyuiVectorIcon.Info);
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<XYNavigationItem, bool>(nameof(IsSelected));
    public string Id { get => GetValue(IdProperty); set => SetValue(IdProperty, value); }
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public XyuiVectorIcon Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public XYNavigationItem() { Classes.Add("xyui-navigation-item"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (change.Property != ChildProperty) Build(); }
    void Build()
    {
        Classes.Set("xyui-navigation-selected", IsSelected);
        Child = new Grid { ColumnDefinitions = new ColumnDefinitions("3,Auto,*"), Children =
        { new Border { Classes = { "xyui-navigation-accent" } }, new XYIcon { Icon = Icon, Size = XyuiIconSize.Small, Classes = { "xyui-navigation-icon" }, [Grid.ColumnProperty] = 1 }, new TextBlock { Text = Label, Classes = { "xyui-navigation-label" }, [Grid.ColumnProperty] = 2, Margin = new Thickness(8, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center } } };
        HookInteraction();
    }
}
