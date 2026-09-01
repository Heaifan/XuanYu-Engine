using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed record XYTocSection(string Id, string Label, int Level = 1, string? ParentId = null, bool IsEnabled = true);
public sealed class XYTableOfContentsState
{
    public IReadOnlyList<XYTocSection> Sections { get; } public string? CurrentSectionId { get; private set; } public event EventHandler? Changed;
    public XYTableOfContentsState(IEnumerable<XYTocSection> sections, string? current = null) { Sections = sections.Where(s => s.Level is 1 or 2).ToArray(); CurrentSectionId = current ?? Sections.FirstOrDefault()?.Id; }
    public bool Commit(string id) { if (CurrentSectionId == id || !Sections.Any(s => s.Id == id && s.IsEnabled)) return false; CurrentSectionId = id; Changed?.Invoke(this, EventArgs.Empty); return true; }
}
public sealed class XYTocSectionRequest : EventArgs
{
    public XYTocSection Section { get; } public bool IsAccepted { get; private set; } public bool IsRejected { get; private set; }
    public XYTocSectionRequest(XYTocSection section) => Section = section;
    public void Accept() { if (!IsRejected) IsAccepted = true; } public void Reject() { IsAccepted = false; IsRejected = true; }
}
public enum XYTableOfContentsVariant { Hierarchical, Compact }
public sealed partial class XYTableOfContents : Border
{
    readonly Border _surface = new() { Classes = { "xyui-toc-surface" } }; readonly Grid _grid = new();
    readonly XYMenu _menu = new() { Classes = { "xyui-toc-menu" } }; readonly Popup _popup = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };
    readonly Dictionary<XYMenuItem, string> _menuIds = new(); IActivatableLifetime? _applicationLifetime; WindowBase? _hostWindow;
    public XYTableOfContentsState State { get; } public XYTableOfContentsVariant Variant { get; } public XYMenu Menu => _menu; public Popup Popup => _popup; public string? CurrentSectionId => State.CurrentSectionId;
    public event EventHandler<XYTocSectionRequest>? SectionRequested; public event EventHandler<string>? SectionChanged;
    public XYTableOfContents(XYTableOfContentsState state, XYTableOfContentsVariant variant = XYTableOfContentsVariant.Hierarchical) { State = state; Variant = variant; State.Changed += (_, _) => Refresh(); Classes.Add("xyui-table-of-contents"); _popup.Child = _menu; _menu.Closed += (_, _) => _popup.IsOpen = false; Child = _surface; Refresh(); }
    public XYTableOfContents(IEnumerable<XYTocSection> sections, XYTableOfContentsVariant variant = XYTableOfContentsVariant.Hierarchical) : this(new XYTableOfContentsState(sections), variant) { }
    void Refresh() { _grid.Children.Clear(); _grid.RowDefinitions.Clear(); if (Variant == XYTableOfContentsVariant.Compact) { _grid.Children.Add(BuildCompact()); } else { _grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); _grid.Children.Add(new TextBlock { Text = "本页目录", Classes = { "xyui-toc-header" }, VerticalAlignment = VerticalAlignment.Center }); _grid.RowDefinitions.Add(new RowDefinition(1, GridUnitType.Star)); var rows = BuildDesktopRows(); Grid.SetRow(rows, 1); _grid.Children.Add(rows); } _surface.Child = _grid; }
    Control BuildDesktopRows() { var host = new StackPanel { Classes = { "xyui-toc-section-host" } }; foreach (var section in State.Sections) host.Children.Add(BuildDesktopItem(section)); return host; }
    XYButton BuildDesktopItem(XYTocSection section) { var parentActive = section.Level == 1 && State.Sections.Any(x => x.ParentId == section.Id && x.Id == CurrentSectionId); var current = section.Id == CurrentSectionId; var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("3,1,*"), Height = 30, VerticalAlignment = VerticalAlignment.Center }; var accent = new Border { Classes = { "xyui-toc-current-accent" }, IsVisible = current }; var guide = new Border { Classes = { "xyui-toc-level-guide" }, IsVisible = section.Level == 2 }; var text = new TextBlock { Text = section.Label, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(section.Level == 2 ? 6 : 0, 0, 0, 0) }; Grid.SetColumn(guide, 1); Grid.SetColumn(text, 2); if (section.Level == 1) Grid.SetColumn(text, 1); grid.Children.Add(accent); grid.Children.Add(guide); grid.Children.Add(text); var item = new XYButton { Content = grid, Height = 30, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch, IsEnabled = section.IsEnabled, Classes = { "xyui-toc-item", section.Level == 2 ? "xyui-toc-child" : "xyui-toc-parent", current ? "xyui-toc-current" : "", parentActive ? "xyui-toc-parent-active" : "" } }; item.Click += (_, _) => SelectSection(section.Id); return item; }
    XYButton BuildCompact() { var current = State.Sections.FirstOrDefault(s => s.Id == CurrentSectionId); var parent = current?.ParentId is null ? null : State.Sections.FirstOrDefault(s => s.Id == current.ParentId); var path = parent is null ? current?.Label ?? "选择章节" : $"{parent.Label} / {current!.Label}"; var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"), VerticalAlignment = VerticalAlignment.Center }; var label = new TextBlock { Text = "本页目录", Classes = { "xyui-toc-trigger-label" }, VerticalAlignment = VerticalAlignment.Center }; var value = new TextBlock { Text = path, Classes = { "xyui-toc-trigger-current" }, TextTrimming = TextTrimming.CharacterEllipsis, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 0, 0) }; var chevron = new XYIcon { Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Small, VerticalAlignment = VerticalAlignment.Center }; Grid.SetColumn(value, 1); Grid.SetColumn(chevron, 2); grid.Children.Add(label); grid.Children.Add(value); grid.Children.Add(chevron); var button = new XYButton { Content = grid, Height = 34, Width = 256, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Stretch, Variant = XyuiButtonVariant.Secondary, Classes = { "xyui-toc-trigger" } }; button.Click += (_, _) => OpenCompactPopup(button); return button; }
    void OpenCompactPopup(Control trigger) { _menuIds.Clear(); var items = State.Sections.Select(section => { var item = new XYMenuItem { Label = section.Label, IsEnabled = section.IsEnabled, IsChecked = section.Id == CurrentSectionId, CheckKind = XyuiMenuCheckKind.Check, Classes = { "xyui-toc-menu-item", section.Level == 2 ? "xyui-toc-menu-child" : "xyui-toc-menu-parent" } }; _menuIds[item] = section.Id; item.SelectionRequested += (_, _) => SelectSection(_menuIds[item]); item.Invoked += (_, _) => CloseCompactPopup(); return item; }).ToArray(); _menu.Items = items; _menu.Open(); foreach (var item in items) item.IsSelected = _menuIds[item] == CurrentSectionId; _popup.PlacementTarget = trigger; var width = trigger.Bounds.Width > 0 ? trigger.Bounds.Width : trigger.Width; _popup.Width = Math.Max(1, width); _popup.IsOpen = true; }
    public void SelectSection(string id) { var section = State.Sections.FirstOrDefault(s => s.Id == id); if (section is null || !section.IsEnabled || section.Id == CurrentSectionId) return; var request = new XYTocSectionRequest(section); SectionRequested?.Invoke(this, request); if (request.IsAccepted && State.Commit(id)) SectionChanged?.Invoke(this, id); }
    public void CommitCurrentSection(string id) => State.Commit(id); public void CloseCompactPopup() { _menu.Close(); _popup.IsOpen = false; }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) { base.OnAttachedToVisualTree(e); _applicationLifetime = Application.Current?.ApplicationLifetime as IActivatableLifetime; if (_applicationLifetime is not null) _applicationLifetime.Deactivated += OnApplicationDeactivated; _hostWindow = e.RootVisual as WindowBase; if (_hostWindow is not null) { _hostWindow.Deactivated += OnHostWindowDeactivated; _hostWindow.Closed += OnHostWindowClosed; } }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { CloseCompactPopup(); if (_applicationLifetime is not null) _applicationLifetime.Deactivated -= OnApplicationDeactivated; if (_hostWindow is not null) { _hostWindow.Deactivated -= OnHostWindowDeactivated; _hostWindow.Closed -= OnHostWindowClosed; } _applicationLifetime = null; _hostWindow = null; base.OnDetachedFromVisualTree(e); }
    void OnApplicationDeactivated(object? sender, ActivatedEventArgs e) => CloseCompactPopup(); void OnHostWindowDeactivated(object? sender, EventArgs e) => CloseCompactPopup(); void OnHostWindowClosed(object? sender, EventArgs e) => CloseCompactPopup();
}
