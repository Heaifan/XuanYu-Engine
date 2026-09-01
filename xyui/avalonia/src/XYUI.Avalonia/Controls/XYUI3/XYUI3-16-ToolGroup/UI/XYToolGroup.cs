using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;
using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Controls;

public sealed class XYToolGroup : Border
{
    public static readonly StyledProperty<bool> IsCollapsedProperty = AvaloniaProperty.Register<XYToolGroup, bool>(nameof(IsCollapsed));
    public bool IsCollapsed { get => GetValue(IsCollapsedProperty); set => SetValue(IsCollapsedProperty, value); }
    public IReadOnlyList<Control> Items { get; }
    public XYIconButton CollapsedTrigger { get; } = new() { Content = new XYIcon { Icon = XyuiVectorIcon.Section, Size = XyuiIconSize.Small } };
    public string? ActiveToolId => Items.OfType<XYToolbarTool>().FirstOrDefault(x => x.IsSelected)?.ToolId;
    public XYToolGroup(params Control[] items) { Items = items; Classes.Add("xyui-tool-group"); XyuiSizingScope.Attach(this); CollapsedTrigger.Click += (_, _) => IsCollapsed = false; Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == IsCollapsedProperty) Build(); }
    void Build() { if (Child is Panel old) old.Children.Clear(); if (IsCollapsed) { var active = Items.OfType<XYToolbarTool>().FirstOrDefault(x => x.IsSelected); if (active?.Icon is { } icon) CollapsedTrigger.Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small }; Child = CollapsedTrigger; } else { var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2 }; panel.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Height = 24 }); foreach (var item in Items) panel.Children.Add(item); Child = panel; } Classes.Set("xyui-tool-group-collapsed", IsCollapsed); }
}
