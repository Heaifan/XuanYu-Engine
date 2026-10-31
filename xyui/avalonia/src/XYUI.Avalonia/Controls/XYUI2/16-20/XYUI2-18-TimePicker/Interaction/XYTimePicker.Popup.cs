using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYTimePicker
{
    TimeOnly _popupStartTime;
    internal void ToggleTimePopup() { if (!IsEnabled) return; if (IsTimePopupOpen) CloseTimePopupForLifecycle(); else OpenTimePopup(ActiveSegment); }
    internal void OpenTimePopup(XYTimeSegment segment)
    {
        if (!IsEnabled || TimePopupPart is null || segment == XYTimeSegment.Second && !ShowSeconds) return;
        if (!IsTimePopupOpen) _popupStartTime = Time; ActivateSegment(segment); IsTimePopupOpen = true; RefreshTimePopup(); TimePopupPart.Height = double.NaN; TimePopupPart.IsVisible = true; TimePopupPart.PlacementTarget = this; TimePopupPart.IsOpen = true;
    }
    internal void CloseTimePopupForLifecycle() { if (!IsTimePopupOpen && TimePopupPart?.IsOpen != true) return; CommitSegmentEdit(); IsTimePopupOpen = false; if (TimePopupPart is not null) { TimePopupPart.IsOpen = false; TimePopupPart.IsVisible = false; TimePopupPart.Height = 0; } }
    void CancelTimePopup() { if (!IsTimePopupOpen) return; CancelSegmentEdit(); Time = _popupStartTime; IsTimePopupOpen = false; if (TimePopupPart is not null) { TimePopupPart.IsOpen = false; TimePopupPart.IsVisible = false; TimePopupPart.Height = 0; } }
    internal void OnTimePopupClosed() { if (IsTimePopupOpen) CloseTimePopupForLifecycle(); }
    internal void RefreshTimePopup() { if (TimePopupSurfacePart is not null) TimePopupSurfacePart.Child = BuildTimePopup(); }
    Control BuildTimePopup()
    {
        var panel = new StackPanel { Spacing = 6, Margin = new Thickness(10), MinWidth = 220 };
        panel.Children.Add(new TextBlock { Text = "调整时间", Classes = { "xyui-text-section" } });
        panel.Children.Add(TimeRow(XYTimeSegment.Hour, "时")); panel.Children.Add(TimeRow(XYTimeSegment.Minute, "分")); if (ShowSeconds) panel.Children.Add(TimeRow(XYTimeSegment.Second, "秒"));
        var actions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 6, Children = { PopupButton("取消", CancelTimePopup), PopupButton("完成", CloseTimePopupForLifecycle) } }; panel.Children.Add(actions); return new Border { Child = panel };
    }
    Control TimeRow(XYTimeSegment segment, string label)
    {
        var down = PopupButton("−", () => AdjustPopupSegment(segment, -1)); down.Tag = $"{segment}-减少"; var up = PopupButton("+", () => AdjustPopupSegment(segment, 1)); up.Tag = $"{segment}-增加"; var value = new TextBlock { Text = SegmentText(segment), Width = 42, TextAlignment = TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center }; return new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, Children = { new TextBlock { Text = label, Width = 24, VerticalAlignment = VerticalAlignment.Center }, down, value, up } };
    }
    void AdjustPopupSegment(XYTimeSegment segment, int amount) { CommitSegmentEdit(); ActiveSegment = segment; SetSegment(segment, GetSegmentValue() + amount); SyncSegmentClasses(); RefreshTimePopup(); }
    static Button PopupButton(string text, Action action) { var button = new Button { Content = text, MinWidth = 28, MinHeight = 28, Padding = new Thickness(4, 0), Template = XyuiControlStyles.ActionCellTemplate(), Background = Brushes.Transparent, BorderThickness = new Thickness(0) }; button.Click += (_, _) => action(); return button; }
}
