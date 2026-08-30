using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYMenuBarItem : Border
{
    bool _building;
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYMenuBarItem, string>(nameof(Label), "");
    public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<XYMenuBarItem, bool>(nameof(IsActive));
    public static readonly StyledProperty<bool> IsHoveredProperty = AvaloniaProperty.Register<XYMenuBarItem, bool>(nameof(IsHovered));
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public bool IsActive { get => GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }
    public bool IsHovered { get => GetValue(IsHoveredProperty); set => SetValue(IsHoveredProperty, value); }
    public XYMenuBarItem() { Classes.Add("xyui-menu-bar-item"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (!_building && change.Property != ChildProperty) Build(); }
    void Build()
    {
        _building = true; Classes.Set("xyui-menu-active", IsActive); Classes.Set("xyui-menu-hover", IsHovered);
        var grid = new Grid { RowDefinitions = new RowDefinitions("32,3") };
        grid.Children.Add(new TextBlock { Text = Label, Classes = { "xyui-menu-bar-label" }, HorizontalAlignment = HorizontalAlignment.Center });
        var indicator = new Border { Classes = { "xyui-menu-bar-indicator" }, Width = Math.Max(28, Label.Length * 14), HorizontalAlignment = HorizontalAlignment.Center };
        grid.Children.Add(indicator); Grid.SetRow(indicator, 1); Child = grid; _building = false;
    }
}
