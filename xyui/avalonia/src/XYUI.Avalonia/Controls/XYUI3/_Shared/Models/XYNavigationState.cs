using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed record XYNavigationEntry(string Id, string Label, XyuiVectorIcon Icon);

public sealed class XYNavigationState
{
    public IReadOnlyList<XYNavigationEntry> Entries { get; }
    public string? SelectedId { get; private set; }
    public event EventHandler? Changed;
    public XYNavigationState(IEnumerable<XYNavigationEntry> entries, string? selectedId = null)
    { Entries = entries.ToArray(); SelectedId = selectedId ?? Entries.FirstOrDefault()?.Id; }
    public void Select(string? id)
    { if (SelectedId == id || !Entries.Any(x => x.Id == id)) return; SelectedId = id; Changed?.Invoke(this, EventArgs.Empty); }
    public XYNavigationEntry? Selected => Entries.FirstOrDefault(x => x.Id == SelectedId);
}
