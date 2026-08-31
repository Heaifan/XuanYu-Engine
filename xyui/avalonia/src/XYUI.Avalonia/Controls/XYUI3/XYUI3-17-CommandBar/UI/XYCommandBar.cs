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
        Items = items; Width = 660; Height = 58; Classes.Add("xyui-command-bar"); MoreButton = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.MoreHorizontal, Size = XyuiIconSize.Small }, Classes = { "xyui-command-more" } };
        foreach (var item in Items) item.ExecuteRequested += (_, _) => CommandExecuted?.Invoke(this, item); MoreButton.Click += (_, _) => ToggleMore(); Child = Build();
    }
    Control Build()
    {
        var canvas = new Canvas { Width = 660, Height = 58 }; var surface = new Border { Width = 644, Height = 34, Classes = { "xyui-command-bar-surface" } }; Canvas.SetLeft(surface, 8); Canvas.SetTop(surface, 10); canvas.Children.Add(surface);
        for (var i = 0; i < Items.Count; i++) { Items[i].Width = i == 0 ? 72 : 46; Items[i].Height = 28; Canvas.SetLeft(Items[i], i == 0 ? 14 : i < 5 ? 96 + (i - 1) * 46 : 304); Canvas.SetTop(Items[i], 13); canvas.Children.Add(Items[i]); }
        var divider = new Border { Width = 1, Height = 20, Classes = { "xyui-command-divider" } }; Canvas.SetLeft(divider, 288); Canvas.SetTop(divider, 17); canvas.Children.Add(divider);
        MoreButton.Width = 28; MoreButton.Height = 28; Canvas.SetLeft(MoreButton, 616); Canvas.SetTop(MoreButton, 13); canvas.Children.Add(MoreButton); _popup.Child = MoreMenu; canvas.Children.Add(_popup); return canvas;
    }
    void ToggleMore() { _popup.IsVisible = !_popup.IsVisible; _popup.PlacementTarget = MoreButton; _popup.IsOpen = _popup.IsVisible; }
    public void CloseMore() { _popup.IsOpen = false; _popup.IsVisible = false; }
}
