using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYSelect
{
    internal static FuncControlTemplate<XYSelect> CreateTemplate() => new((control, scope) =>
    {
        var value = new TextBlock { Name = "PART_Value", VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
        value[!TextBlock.ForegroundProperty] = control[!TemplatedControl.ForegroundProperty];
        var valueSurface = new Border { Name = "PART_ValueSurface", Padding = new Thickness(8, 0), Child = value };
        var icon = new XYIcon { Name = "PART_Chevron", Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Small, VerticalAlignment = VerticalAlignment.Center };
        icon[!XYIcon.StrokeProperty] = control[!TemplatedControl.ForegroundProperty];
        var chevronCell = new Border { Name = "PART_ChevronCell", Width = XyuiComponentTokens.SelectChevronSurfaceWidth, Child = icon };
        var surface = new Grid { Name = "PART_Surface", ColumnDefinitions = new ColumnDefinitions { new(1, GridUnitType.Star), new(XyuiComponentTokens.SelectChevronSurfaceWidth, GridUnitType.Pixel) }, Children = { valueSurface, chevronCell } };
        Grid.SetColumn(chevronCell, 1);
        var chrome = new Border { Name = "PART_Chrome", Child = surface };
        chrome[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty]; chrome[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty];
        chrome[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty]; chrome[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
        var edge = new Border { Name = "PART_FocusEdge", Height = 3, VerticalAlignment = VerticalAlignment.Bottom, IsHitTestVisible = false };
        var list = new ListBox { Name = "PART_List", MinHeight = 28, MaxHeight = 240 };
        var popupSurface = new Border { Name = "PART_PopupSurface", Child = list };
        var popup = new Popup { Name = "PART_Popup", Height = 0, IsVisible = false, Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, Child = popupSurface };
        scope?.Register("PART_Value", value); scope?.Register("PART_ValueSurface", valueSurface); scope?.Register("PART_ChevronCell", chevronCell); scope?.Register("PART_Chevron", icon); scope?.Register("PART_Popup", popup); scope?.Register("PART_List", list);
        surface.PointerPressed += control.OnSurfacePressed;
        surface.PointerReleased += control.OnSurfaceReleased;
        return new Grid { Children = { chrome, edge, popup } };
    });

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (ListPart is not null) ListPart.SelectionChanged -= OnListSelectionChanged;
        if (PopupPart is not null) PopupPart.Closed -= OnPopupClosed;
        base.OnApplyTemplate(e);
        ValuePart = e.NameScope.Find<TextBlock>("PART_Value"); ValueSurfacePart = e.NameScope.Find<Border>("PART_ValueSurface"); ChevronCellPart = e.NameScope.Find<Border>("PART_ChevronCell"); ChevronPart = e.NameScope.Find<XYIcon>("PART_Chevron"); PopupPart = e.NameScope.Find<Popup>("PART_Popup"); ListPart = e.NameScope.Find<ListBox>("PART_List");
        if (ListPart is null || PopupPart is null) return;
        ListPart.SelectionChanged += OnListSelectionChanged; PopupPart.Closed += OnPopupClosed; SyncParts(); if (IsDropDownOpen) OpenPopup();
    }

    void OnSurfacePressed(object? sender, PointerPressedEventArgs e) { if (!IsEnabled) return; PseudoClasses.Set(":pressed", true); Focus(); IsDropDownOpen = !IsDropDownOpen; e.Handled = true; }
    void OnSurfaceReleased(object? sender, PointerReleasedEventArgs e) => PseudoClasses.Set(":pressed", false);
    void OnListSelectionChanged(object? sender, SelectionChangedEventArgs e) { if (IsKeyboardNavigating || ListPart?.SelectedItem is not object item) return; SelectedItem = item; IsDropDownOpen = false; }
    void OnPopupClosed(object? sender, EventArgs e) => ClosePopupForLifecycle();
}
