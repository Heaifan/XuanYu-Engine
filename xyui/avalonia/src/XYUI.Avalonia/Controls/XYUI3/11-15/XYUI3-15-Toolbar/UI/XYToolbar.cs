using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Metadata;
using System.Collections.ObjectModel;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed class XYToolbar : Border
{
    public static readonly StyledProperty<bool> IsCompactProperty = AvaloniaProperty.Register<XYToolbar, bool>(nameof(IsCompact), true);
    public bool IsCompact { get => GetValue(IsCompactProperty); set => SetValue(IsCompactProperty, value); }
    [Content] public ObservableCollection<Control> Items { get; } = [];
    public ObservableCollection<XYToolbarTool> OverflowItems { get; } = [];
    public string? ActiveToolId { get; private set; }
    public XYIconButton OverflowButton { get; } = new() { Content = new XYIcon { Icon = XYUI.Avalonia.Vector.XyuiVectorIcon.MoreHorizontal, Size = XyuiIconSize.Small }, Classes = { "xyui-toolbar-overflow" } };
    public XYMenu OverflowMenu { get; } = new();
    public Popup OverflowPopup { get; } = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };
    public XYToolbar() : this(Array.Empty<Control>()) { }
    public XYToolbar(params Control[] items) { foreach (var item in items) Items.Add(item); Items.CollectionChanged += (_, _) => Build(); OverflowItems.CollectionChanged += (_, _) => RefreshOverflow(); Classes.Add("xyui-toolbar"); KeyDown += OnKeyDown; OverflowButton.Click += (_, _) => ToggleOverflow(); OverflowPopup.Child = OverflowMenu; Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == IsCompactProperty) Build(); }
    void Build() { foreach (var item in Items) if (item is XYToolGroup group) group.IsCompact = IsCompact; foreach (var item in Items) if (item.GetVisualParent() is Panel oldPanel) oldPanel.Children.Remove(item); if (OverflowButton.GetVisualParent() is Panel overflowPanel) overflowPanel.Children.Remove(OverflowButton); Child = null; var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = IsCompact ? 2 : 4, VerticalAlignment = VerticalAlignment.Center }; foreach (var item in Items) { if (item is XYToolbarTool tool) { tool.ShowLabel = !IsCompact; tool.SelectionRequested -= OnToolSelected; tool.SelectionRequested += OnToolSelected; } panel.Children.Add(item); } if (OverflowItems.Count > 0) panel.Children.Add(OverflowButton); Child = panel; RefreshOverflow(); }
    void OnToolSelected(object? sender, EventArgs e) { if (sender is not XYToolbarTool tool) return; foreach (var item in Items.OfType<XYToolbarTool>()) item.IsSelected = ReferenceEquals(item, tool) ? tool.IsSelected : false; ActiveToolId = tool.IsSelected ? tool.ToolId : null; }
    void RefreshOverflow() { var items = new List<Control>(); foreach (var tool in OverflowItems) { var item = new XYMenuItem { Label = tool.Label, IsEnabled = tool.IsEnabled }; item.SelectionRequested += (_, _) => { tool.Invoke(); OverflowPopup.IsOpen = false; }; items.Add(item); } OverflowMenu.Child = null; OverflowMenu.Items = items; OverflowButton.IsVisible = OverflowItems.Count > 0; }
    void ToggleOverflow() { if (OverflowPopup.IsOpen) { OverflowPopup.IsOpen = false; return; } OverflowPopup.PlacementTarget = OverflowButton; OverflowPopup.IsOpen = true; OverflowMenu.ApplyOverlayStyling(); OverflowMenu.Open(); }
    void OnKeyDown(object? sender, KeyEventArgs e) { var tools = Items.OfType<XYToolbarTool>().ToArray(); if (tools.Length == 0) return; var index = Array.FindIndex(tools, x => x.Button.IsFocused); if (e.Key is Key.Right or Key.Down) tools[Math.Clamp(index + 1, 0, tools.Length - 1)].Button.Focus(); else if (e.Key is Key.Left or Key.Up) tools[Math.Clamp(index < 0 ? 0 : index - 1, 0, tools.Length - 1)].Button.Focus(); else if ((e.Key is Key.Enter or Key.Space) && index >= 0) tools[index].Invoke(); else return; e.Handled = true; }
}
