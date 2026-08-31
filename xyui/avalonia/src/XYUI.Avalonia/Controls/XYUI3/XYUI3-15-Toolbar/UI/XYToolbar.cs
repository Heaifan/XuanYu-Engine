using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYToolbar : Border
{
    public static readonly StyledProperty<bool> IsCompactProperty = AvaloniaProperty.Register<XYToolbar, bool>(nameof(IsCompact), true);
    public bool IsCompact { get => GetValue(IsCompactProperty); set => SetValue(IsCompactProperty, value); }
    public IReadOnlyList<Control> Items { get; }
    public string? ActiveToolId { get; private set; }
    public XYToolbar(params Control[] items) { Items = items; Classes.Add("xyui-toolbar"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == IsCompactProperty) Build(); }
    void Build() { var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = IsCompact ? 2 : 4, VerticalAlignment = VerticalAlignment.Center }; foreach (var item in Items) { if (item is XYToolbarTool tool) { tool.ShowLabel = !IsCompact; tool.SelectionRequested -= OnToolSelected; tool.SelectionRequested += OnToolSelected; } panel.Children.Add(item); } Child = panel; }
    void OnToolSelected(object? sender, EventArgs e) { if (sender is not XYToolbarTool tool) return; ActiveToolId = tool.ToolId; foreach (var item in Items.OfType<XYToolbarTool>()) item.IsSelected = ReferenceEquals(item, tool); }
}
