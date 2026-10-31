using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;
using Avalonia.Metadata;
using System.Collections.ObjectModel;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed class XYToolGroup : Border
{
    public static readonly StyledProperty<bool> IsCollapsedProperty = AvaloniaProperty.Register<XYToolGroup, bool>(nameof(IsCollapsed));
    public static readonly StyledProperty<bool> IsCompactProperty = AvaloniaProperty.Register<XYToolGroup, bool>(nameof(IsCompact));
    public bool IsCollapsed { get => GetValue(IsCollapsedProperty); set => SetValue(IsCollapsedProperty, value); }
    public bool IsCompact { get => GetValue(IsCompactProperty); set => SetValue(IsCompactProperty, value); }
    [Content] public ObservableCollection<Control> Items { get; } = [];
    public string? GroupLabel { get; set; }
    public bool OwnsSeparator { get; set; } = true;
    public XYIconButton CollapsedTrigger { get; } = new() { Content = new XYIcon { Icon = XyuiVectorIcon.Section, Size = XyuiIconSize.Small } };
    public string? ActiveToolId => Items.OfType<XYToolbarTool>().FirstOrDefault(x => x.IsSelected)?.ToolId;
    public XYToolGroup() : this(Array.Empty<Control>()) { }
    public XYToolGroup(params Control[] items) { foreach (var item in items) Items.Add(item); Items.CollectionChanged += (_, _) => { AttachItems(); Build(); }; Classes.Add("xyui-tool-group"); CollapsedTrigger.Classes.Add("xyui-tool-group-collapsed-trigger"); CollapsedTrigger.Click += (_, _) => IsCollapsed = false; AttachItems(); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == IsCollapsedProperty || e.Property == IsCompactProperty) Build(); }
    void AttachItems() { foreach (var tool in Items.OfType<XYToolbarTool>()) { tool.SelectionRequested -= OnToolSelected; tool.SelectionRequested += OnToolSelected; } }
    void OnToolSelected(object? sender, EventArgs e) { RefreshCollapsedTrigger(); if (IsCollapsed) Build(); }
    void RefreshCollapsedTrigger() { var active = Items.OfType<XYToolbarTool>().FirstOrDefault(x => x.IsSelected); if (active?.Icon is { } icon) { CollapsedTrigger.Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small }; ToolTip.SetTip(CollapsedTrigger, active.Label); } }
    void Build() { foreach (var tool in Items.OfType<XYToolbarTool>()) tool.ShowLabel = !IsCompact; foreach (var item in Items) if (item.GetVisualParent() is Panel panel) panel.Children.Remove(item); if (CollapsedTrigger.GetVisualParent() is Panel triggerPanel) triggerPanel.Children.Remove(CollapsedTrigger); Child = null; if (IsCollapsed) { RefreshCollapsedTrigger(); Child = CollapsedTrigger; } else { var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2 }; if (OwnsSeparator) panel.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Height = 24 }); if (!string.IsNullOrWhiteSpace(GroupLabel)) panel.Children.Add(new TextBlock { Text = GroupLabel, VerticalAlignment = VerticalAlignment.Center }); foreach (var item in Items) panel.Children.Add(item); Child = panel; } Classes.Set("xyui-tool-group-collapsed", IsCollapsed); }
}
