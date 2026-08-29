using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYTimePicker
{
    internal static FuncControlTemplate<XYTimePicker> CreateTemplate() => new((control, scope) =>
    {
        var segments = new StackPanel { Name = "PART_Segments", Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
        segments.Children.Add(control.CreateSegment(XYTimeSegment.Hour, scope)); segments.Children.Add(Separator()); segments.Children.Add(control.CreateSegment(XYTimeSegment.Minute, scope)); var secondsSeparator = Separator(); secondsSeparator.Name = "PART_SecondSeparator"; secondsSeparator.IsVisible = control.ShowSeconds; segments.Children.Add(secondsSeparator); control.SecondSeparatorPart = secondsSeparator; scope?.Register("PART_SecondSeparator", secondsSeparator); segments.Children.Add(control.CreateSegment(XYTimeSegment.Second, scope));
        var clock = new XYIcon { Name = "PART_ClockGlyph", Icon = XyuiVectorIcon.Clock, Size = XyuiIconSize.Small, VerticalAlignment = VerticalAlignment.Center }; var clockButton = new Button { Name = "PART_ClockButton", Width = 32, Padding = new Thickness(0), Template = XyuiControlStyles.ActionCellTemplate(), Background = Brushes.Transparent, BorderThickness = new Thickness(0), Content = clock }; clockButton.Click += (_, _) => control.ToggleTimePopup(); control.ClockButtonPart = clockButton; scope?.Register("PART_ClockGlyph", clock); scope?.Register("PART_ClockButton", clockButton); var scrub = new XYIcon { Name = "PART_ScrubIndicator", Icon = XyuiVectorIcon.ScrubLeftRight, Size = XyuiIconSize.Tiny, IsVisible = false, VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 2, 1) }; control.ScrubIndicatorPart = scrub; scope?.Register("PART_ScrubIndicator", scrub); var glyphCell = new Grid { Width = 32, Children = { clockButton, scrub } };
        var surface = new Grid { ColumnDefinitions = new ColumnDefinitions { new(1, GridUnitType.Star), new(32, GridUnitType.Pixel) }, Children = { segments, glyphCell } }; Grid.SetColumn(glyphCell, 1);
        var chrome = new Border { Name = "PART_Chrome", Child = surface }; chrome[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty]; chrome[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty]; chrome[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty]; chrome[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty]; var edge = new Border { Name = "PART_FocusEdge", Height = 3, VerticalAlignment = VerticalAlignment.Bottom, IsHitTestVisible = false }; var popupSurface = new Border { Name = "PART_TimePopupSurface" }; var popup = new Popup { Name = "PART_TimePopup", Height = 0, IsVisible = false, Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, Child = popupSurface }; control.TimePopupSurfacePart = popupSurface; control.TimePopupPart = popup; popupSurface.Child = control.BuildTimePopup(); popup.Closed += (_, _) => control.OnTimePopupClosed(); scope?.Register("PART_Chrome", chrome); scope?.Register("PART_FocusEdge", edge); scope?.Register("PART_TimePopupSurface", popupSurface); scope?.Register("PART_TimePopup", popup); return new Grid { Children = { chrome, edge, popup } };
    });
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) { base.OnApplyTemplate(e); SecondSeparatorPart = e.NameScope.Find<Button>("PART_SecondSeparator"); ClockButtonPart = e.NameScope.Find<Button>("PART_ClockButton"); ScrubIndicatorPart = e.NameScope.Find<XYIcon>("PART_ScrubIndicator"); TimePopupPart = e.NameScope.Find<Popup>("PART_TimePopup"); TimePopupSurfacePart = e.NameScope.Find<Border>("PART_TimePopupSurface"); SyncTimeParts(); RefreshTimePopup(); }
    Button CreateSegment(XYTimeSegment segment, INameScope? scope) { var button = new Button { Name = $"PART_{segment}", Content = SegmentText(segment), Padding = new Thickness(2, 0), Template = XyuiControlStyles.ActionCellTemplate(), Background = Brushes.Transparent, BorderThickness = new Thickness(0), Focusable = true, MinWidth = 26, IsVisible = segment != XYTimeSegment.Second || ShowSeconds }; button.Click += (_, _) => { if (!ConsumePointerAction()) { ActivateSegment(segment); OpenTimePopup(segment); } }; SegmentButtons[segment] = button; scope?.Register(button.Name!, button); return button; }
    static Button Separator() => new() { Content = ":", Padding = new Thickness(0), Template = XyuiControlStyles.ActionCellTemplate(), Background = Brushes.Transparent, BorderThickness = new Thickness(0), Focusable = false, IsHitTestVisible = false };
}
