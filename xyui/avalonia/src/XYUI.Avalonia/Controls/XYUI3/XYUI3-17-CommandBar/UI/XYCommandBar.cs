using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYCommandItem : XYButton
{
    public string CommandId { get; }
    public event EventHandler? ExecuteRequested;
    public XYCommandItem(string label, string? commandId = null) { Content = label; CommandId = commandId ?? label; Variant = label == "新建" ? XyuiButtonVariant.Primary : label == "删除" ? XyuiButtonVariant.Danger : XyuiButtonVariant.Secondary; Classes.Add("xyui-command-item"); Click += (_, _) => { if (IsEnabled) ExecuteRequested?.Invoke(this, EventArgs.Empty); }; }
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
        Items = items; Width = 660; Height = 58; Classes.Add("xyui-command-bar"); MoreButton = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.MoreHorizontal, Size = XyuiIconSize.Small }, Classes = { "xyui-command-more" } }; _popup.Closed += (_, _) => CloseMore();
        foreach (var item in Items) item.ExecuteRequested += (_, _) => CommandExecuted?.Invoke(this, item); MoreButton.Click += (_, _) => ToggleMore(); Child = Build();
    }
    Control Build()
    {
        var surface = new Border { Margin = new Thickness(8, 10), Classes = { "xyui-command-bar-surface" } }; var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*,28") }; var commands = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2, Margin = new Thickness(5, 3, 0, 3) };
        foreach (var item in Items) { item.Height = 28; commands.Children.Add(item); } grid.Children.Add(commands); var divider = new Border { Width = 1, Height = 20, Classes = { "xyui-command-divider" } }; Grid.SetColumn(divider, 1); grid.Children.Add(divider); MoreButton.Width = 28; MoreButton.Height = 28; Grid.SetColumn(MoreButton, 2); grid.Children.Add(MoreButton); _popup.Child = MoreMenu; surface.Child = grid; return surface;
    }
    void ToggleMore() { if (_popup.IsOpen) CloseMore(); else { _popup.PlacementTarget = MoreButton; _popup.IsOpen = true; } }
    public void CloseMore() { if (_popup.IsOpen) _popup.IsOpen = false; }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { CloseMore(); base.OnDetachedFromVisualTree(e); }
}
