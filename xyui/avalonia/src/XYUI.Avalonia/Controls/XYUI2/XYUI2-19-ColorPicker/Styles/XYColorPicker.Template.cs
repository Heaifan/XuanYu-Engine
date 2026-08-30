using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYColorPicker
{
    internal static FuncControlTemplate<XYColorPicker> CreateTemplate() => new((control, scope) =>
    {
        var color = new TextBlock { Name = "PART_Value", VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
        color[!TextBlock.ForegroundProperty] = control[!TemplatedControl.ForegroundProperty];
        var swatchColor = new Border { Name = "PART_SwatchColor", CornerRadius = new CornerRadius(2) };
        var swatchGrid = new Grid { Name = "PART_Swatch", Width = XyuiComponentTokens.ColorPickerSwatchWidth, Height = XyuiComponentTokens.ColorPickerSwatchHeight };
        var checker = new[] { false, true, true, false }; for (var i = 0; i < checker.Length; i++) { var cell = new Border(); cell.Classes.Add(checker[i] ? "xyui-checker-dark" : "xyui-checker-light"); swatchGrid.Children.Add(cell); Grid.SetColumn(cell, i % 2); Grid.SetRow(cell, i / 2); }
        swatchGrid.RowDefinitions = new RowDefinitions("*,*"); swatchGrid.ColumnDefinitions = new ColumnDefinitions("*,*"); swatchGrid.Children.Add(swatchColor); Grid.SetRowSpan(swatchColor, 2); Grid.SetColumnSpan(swatchColor, 2); control.SwatchPart = swatchColor;
        var icon = new XYIcon { Name = "PART_Chevron", Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Small, VerticalAlignment = VerticalAlignment.Center };
        icon[!XYIcon.StrokeProperty] = control[!TemplatedControl.ForegroundProperty]; control.ChevronPart = icon;
        var content = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"), ColumnSpacing = 8, Children = { swatchGrid, color, icon } }; Grid.SetColumn(color, 1); Grid.SetColumn(icon, 2);
        var button = new Button { Name = "PART_FieldButton", Padding = new Thickness(8, 0), Template = XyuiControlStyles.ActionCellTemplate(), Background = Brushes.Transparent, BorderThickness = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Stretch, Content = content };
        button.Click += (_, _) => { if (control.IsEnabled) { control.Focus(); control.IsOpen = !control.IsOpen; } };
        var chrome = new Border { Name = "PART_Chrome", Child = button }; chrome[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty]; chrome[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty]; chrome[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty]; chrome[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
        var edge = new Border { Name = "PART_FocusEdge", Height = 3, VerticalAlignment = VerticalAlignment.Bottom, IsHitTestVisible = false };
        var popupSurface = new Border { Name = "PART_ColorPanelSurface" }; var popup = new Popup { Name = "PART_Popup", Height = 0, IsVisible = false, Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, Child = popupSurface };
        control.PopupSurfacePart = popupSurface; control.PopupPart = popup; popupSurface.Child = control.BuildColorPanel(); popup.Closed += control.OnPopupClosed;
        scope?.Register("PART_Value", color); scope?.Register("PART_Swatch", swatchGrid); scope?.Register("PART_SwatchColor", swatchColor); scope?.Register("PART_Chevron", icon); scope?.Register("PART_Popup", popup); scope?.Register("PART_ColorPanelSurface", popupSurface); scope?.Register("PART_FocusEdge", edge); return new Grid { Children = { chrome, edge, popup } };
    });
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) { if (PopupPart is not null) PopupPart.Closed -= OnPopupClosed; base.OnApplyTemplate(e); ValuePart = e.NameScope.Find<TextBlock>("PART_Value"); SwatchPart = e.NameScope.Find<Border>("PART_SwatchColor"); ChevronPart = e.NameScope.Find<XYIcon>("PART_Chevron"); PopupPart = e.NameScope.Find<Popup>("PART_Popup"); PopupSurfacePart = e.NameScope.Find<Border>("PART_ColorPanelSurface"); if (PopupPart is not null) PopupPart.Closed += OnPopupClosed; SyncVisuals(); if (IsOpen) OpenPanel(); }
    void SyncVisuals() { if (ValuePart is not null) ValuePart.Text = DisplayValue(); if (SwatchPart is not null) SwatchPart.Background = new SolidColorBrush(Color); if (ChevronPart is not null) ChevronPart.RenderTransform = new RotateTransform(IsOpen ? 180 : 0); SyncPanelValues(); }
}
