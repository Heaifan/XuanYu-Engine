using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed record XYNavigationEntry(
    string Id,
    string Label,
    XyuiVectorIcon Icon,
    string? Badge = null,
    XyuiStatusState Status = XyuiStatusState.Neutral,
    bool IsEnabled = true);

public sealed class XYNavigationRequest : EventArgs
{
    public XYNavigationEntry Destination { get; }
    public bool IsAccepted { get; private set; } = true;
    public bool IsRejected { get; private set; }
    public XYNavigationRequest(XYNavigationEntry destination) => Destination = destination;
    public void Accept() { IsAccepted = true; IsRejected = false; }
    public void Reject() { IsRejected = true; IsAccepted = false; }
}

public sealed class XYNavigationState
{
    public IReadOnlyList<XYNavigationEntry> Entries { get; }
    public string? SelectedId { get; private set; }
    public string? CurrentDestinationId => SelectedId;
    public event EventHandler? Changed;
    public event EventHandler<XYNavigationRequest>? NavigationRequested;
    public XYNavigationState(IEnumerable<XYNavigationEntry> entries, string? selectedId = null)
    { Entries = entries.ToArray(); SelectedId = selectedId ?? Entries.FirstOrDefault()?.Id; }
    public void Select(string? id)
    { if (SelectedId == id || !Entries.Any(x => x.Id == id)) return; SelectedId = id; Changed?.Invoke(this, EventArgs.Empty); }
    public bool RequestNavigation(string? id)
    {
        var entry = Entries.FirstOrDefault(x => x.Id == id);
        if (entry is null || !entry.IsEnabled || entry.Id == SelectedId) return false;
        var request = new XYNavigationRequest(entry); NavigationRequested?.Invoke(this, request);
        if (request.IsRejected || !request.IsAccepted) return false;
        Select(entry.Id); return true;
    }
    public XYNavigationEntry? Selected => Entries.FirstOrDefault(x => x.Id == SelectedId);
}
