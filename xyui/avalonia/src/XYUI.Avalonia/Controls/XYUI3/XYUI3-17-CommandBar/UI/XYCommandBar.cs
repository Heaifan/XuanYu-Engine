using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYCommandItem : Button
{
    public string CommandId { get; }
    public event EventHandler? ExecuteRequested;
    public XYCommandItem(string label, string? commandId = null) { Content = label; CommandId = commandId ?? label; Classes.Add("xyui-command-item"); Click += (_, _) => { if (IsEnabled) ExecuteRequested?.Invoke(this, EventArgs.Empty); }; }
}

public sealed class XYCommandBar : Border
{
    readonly Popup _popup = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, IsVisible = false };
    public IReadOnlyList<XYCommandItem> Items { get; }
    public XYMenu MoreMenu { get; } = new();
    public Popup MorePopup => _popup;
    public XYIconButton MoreButton { get; }
    public event EventHandler<XYCommandItem>? CommandExecuted;
    public XYCommandBar(params XYCommandItem[] items)
    {
        Items = items; Classes.Add("xyui-command-bar"); MoreButton = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.MoreHorizontal, Size = XyuiIconSize.Small }, Classes = { "xyui-command-more" } };
        foreach (var item in Items) item.ExecuteRequested += (_, _) => CommandExecuted?.Invoke(this, item); MoreButton.Click += (_, _) => ToggleMore(); Child = Build();
    }
    Control Build() { var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2 }; foreach (var item in Items) panel.Children.Add(item); panel.Children.Add(MoreButton); _popup.Child = MoreMenu; panel.Children.Add(_popup); return panel; }
    void ToggleMore() { _popup.IsVisible = !_popup.IsVisible; _popup.PlacementTarget = MoreButton; _popup.IsOpen = _popup.IsVisible; }
    public void CloseMore() { _popup.IsOpen = false; _popup.IsVisible = false; }
}
