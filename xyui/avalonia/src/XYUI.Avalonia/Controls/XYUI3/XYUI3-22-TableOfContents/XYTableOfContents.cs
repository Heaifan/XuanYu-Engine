using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed record XYTocSection(string Id, string Label, int Level = 1, string? ParentId = null, bool IsEnabled = true);
public sealed class XYTableOfContentsState
{
    public IReadOnlyList<XYTocSection> Sections { get; } public string? CurrentSectionId { get; private set; } public event EventHandler? Changed;
    public XYTableOfContentsState(IEnumerable<XYTocSection> sections, string? current = null) { Sections = sections.Where(s => s.Level is 1 or 2).ToArray(); CurrentSectionId = current ?? Sections.FirstOrDefault()?.Id; }
    public void Commit(string id) { if (Sections.Any(s => s.Id == id && s.IsEnabled)) { CurrentSectionId = id; Changed?.Invoke(this, EventArgs.Empty); } }
}
public sealed class XYTocSectionRequest : EventArgs
{
    public XYTocSection Section { get; } public bool IsAccepted { get; private set; } public bool IsRejected { get; private set; }
    public XYTocSectionRequest(XYTocSection section) => Section = section; public void Accept() { IsAccepted = true; IsRejected = false; } public void Reject() { IsRejected = true; IsAccepted = false; }
}
public enum XYTableOfContentsVariant { Hierarchical, Compact }
public sealed class XYTableOfContents : Border
{
    readonly XYMenu _menu = new() { Classes = { "xyui-toc-menu" } }; readonly Popup _popup = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };
    public XYTableOfContentsState State { get; } public XYTableOfContentsVariant Variant { get; set; } public XYMenu Menu => _menu; public Popup Popup => _popup; public string? CurrentSectionId => State.CurrentSectionId;
    public event EventHandler<XYTocSectionRequest>? SectionRequested; public event EventHandler<string>? SectionChanged;
    public XYTableOfContents(XYTableOfContentsState state, XYTableOfContentsVariant variant = XYTableOfContentsVariant.Hierarchical) { State = state; Variant = variant; State.Changed += (_, _) => Refresh(); Classes.Add("xyui-table-of-contents"); _popup.Child = _menu; Child = new StackPanel(); Refresh(); }
    public XYTableOfContents(IEnumerable<XYTocSection> sections, XYTableOfContentsVariant variant = XYTableOfContentsVariant.Hierarchical) : this(new XYTableOfContentsState(sections), variant) { }
    void Refresh() { var host = (StackPanel)Child!; host.Children.Clear(); if (Variant == XYTableOfContentsVariant.Compact) host.Children.Add(BuildCompact()); else foreach (var section in State.Sections) host.Children.Add(BuildSection(section)); }
    XYButton BuildCompact() { var current = State.Sections.FirstOrDefault(s => s.Id == CurrentSectionId); var button = new XYButton { Content = current?.Label ?? "目录", Variant = XyuiButtonVariant.Secondary, Classes = { "xyui-toc-trigger" }, Height = 34 }; button.Click += (_, _) => Open(); return button; }
    XYMenuItem BuildSection(XYTocSection section) { var item = new XYMenuItem { Label = section.Label, Icon = section.Level == 1 ? XyuiVectorIcon.Section : XyuiVectorIcon.Browse, IsEnabled = section.IsEnabled, Classes = { "xyui-toc-item", section.Level == 2 ? "xyui-toc-child" : "xyui-toc-parent" } }; item.IsSelected = section.Id == CurrentSectionId; item.SelectionRequested += (_, _) => SelectSection(section.Id); return item; }
    void Open() { _menu.Items = State.Sections.Select(BuildSection).ToArray(); _popup.IsOpen = true; }
    public void SelectSection(string id) { var section = State.Sections.FirstOrDefault(s => s.Id == id); if (section is null || !section.IsEnabled) return; var request = new XYTocSectionRequest(section); SectionRequested?.Invoke(this, request); if (!request.IsRejected) { State.Commit(id); SectionChanged?.Invoke(this, id); } }
    public void CommitCurrentSection(string id) => State.Commit(id);
}
