namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationRail
{
    void Attach(XYNavigationItem item) { item.Classes.Add("xyui-rail-item"); item.Selected += OnSelected; _panel.Children.Add(item); }
    void OnSelected(object? sender, EventArgs e) { if (ContextItems.Count > 0) _contextFlyout.Open(); }
}
