using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYPasswordField
{
    internal static FuncControlTemplate<XYPasswordField> CreateTemplate() => new((control, scope) =>
    {
        var presenter = new TextPresenter { Name = "PART_TextPresenter", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch };
        presenter[!TextPresenter.CaretIndexProperty] = control[!TextBox.CaretIndexProperty]; presenter[!TextPresenter.SelectionStartProperty] = control[!TextBox.SelectionStartProperty]; presenter[!TextPresenter.SelectionEndProperty] = control[!TextBox.SelectionEndProperty]; presenter[!TextPresenter.SelectionBrushProperty] = control[!TextBox.SelectionBrushProperty]; presenter[!TextPresenter.SelectionForegroundBrushProperty] = control[!TextBox.SelectionForegroundBrushProperty]; presenter[!TextPresenter.CaretBrushProperty] = control[!TextBox.CaretBrushProperty]; presenter[!TextPresenter.TextWrappingProperty] = control[!TextBox.TextWrappingProperty];
        var placeholder = new TextBlock { Name = "PART_Placeholder", IsHitTestVisible = false, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch };
        placeholder[!TextBlock.TextProperty] = control[!XYPasswordField.PlaceholderProperty]; placeholder[!TextBlock.IsVisibleProperty] = new Binding("Text") { Mode = BindingMode.OneWay, Converter = new FuncValueConverter<string?, bool>(string.IsNullOrEmpty), Source = control };
        var text = new Grid { Children = { placeholder, presenter } }; var textPadding = new Border { Name = "PART_TextPadding", Padding = new Thickness(10, 0, 8, 0), Child = text };
        var eyeIcon = new XYIcon { Name = "PART_EyeGlyph", Icon = XyuiVectorIcon.Eye, Size = XyuiIconSize.Small }; var eye = new Button { Name = "PART_RevealCell", Width = 32, Height = 32, MinWidth = 32, MaxWidth = 32, MinHeight = 32, MaxHeight = 32, HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, Padding = new Thickness(0), BorderThickness = new Thickness(0), CornerRadius = new CornerRadius(0, 3, 3, 0), Content = eyeIcon, Focusable = true };
        var content = new Grid { ColumnDefinitions = new ColumnDefinitions { new(1, GridUnitType.Star), new(32, GridUnitType.Pixel) }, Children = { textPadding, eye } }; Grid.SetColumn(eye, 1);
        var chrome = new Border { Name = "PART_Chrome", Child = content }; chrome[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty]; chrome[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty]; chrome[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty]; chrome[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
        var edge = new Border { Name = "PART_FocusEdge", Height = 3, VerticalAlignment = VerticalAlignment.Bottom, IsHitTestVisible = false };
        scope?.Register("PART_TextPresenter", presenter); scope?.Register("PART_Placeholder", placeholder); scope?.Register("PART_TextPadding", textPadding); scope?.Register("PART_EyeGlyph", eyeIcon); scope?.Register("PART_RevealCell", eye); scope?.Register("PART_Chrome", chrome); scope?.Register("PART_FocusEdge", edge);
        return new Grid { Children = { chrome, edge } };
    });

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachRevealHandlers(); base.OnApplyTemplate(e); PasswordPresenterPart = e.NameScope.Find<TextPresenter>("PART_TextPresenter"); TextPaddingPart = e.NameScope.Find<Border>("PART_TextPadding"); RevealPart = e.NameScope.Find<Button>("PART_RevealCell");
        AttachRevealHandlers(); SyncPresentation();
    }
    void AttachRevealHandlers() { if (RevealPart is null) return; var routes = RoutingStrategies.Tunnel | RoutingStrategies.Bubble; RevealPart.AddHandler(InputElement.PointerPressedEvent, OnRevealPointerPressed, routes, true); RevealPart.AddHandler(InputElement.PointerReleasedEvent, OnRevealPointerReleased, routes, true); RevealPart.AddHandler(InputElement.PointerCaptureLostEvent, OnRevealPointerCaptureLost, routes, true); RevealPart.KeyDown += OnRevealKeyDown; RevealPart.KeyUp += OnRevealKeyUp; RevealPart.LostFocus += OnRevealLostFocus; }
    void DetachRevealHandlers() { if (RevealPart is null) return; RevealPart.RemoveHandler(InputElement.PointerPressedEvent, OnRevealPointerPressed); RevealPart.RemoveHandler(InputElement.PointerReleasedEvent, OnRevealPointerReleased); RevealPart.RemoveHandler(InputElement.PointerCaptureLostEvent, OnRevealPointerCaptureLost); RevealPart.KeyDown -= OnRevealKeyDown; RevealPart.KeyUp -= OnRevealKeyUp; RevealPart.LostFocus -= OnRevealLostFocus; }
}
