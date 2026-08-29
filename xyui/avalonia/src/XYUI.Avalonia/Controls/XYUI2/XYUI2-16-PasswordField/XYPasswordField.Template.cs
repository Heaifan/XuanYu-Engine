using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
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
        var text = new Grid { Children = { placeholder, presenter } };
        var eyeIcon = new XYIcon { Name = "PART_EyeGlyph", Icon = XyuiVectorIcon.Eye, Size = XyuiIconSize.Small }; var eye = new Button { Name = "PART_RevealCell", Width = 34, MinWidth = 34, MaxWidth = 34, Padding = new Thickness(0), BorderThickness = new Thickness(0), Content = eyeIcon, Focusable = true };
        var content = new Grid { ColumnDefinitions = new ColumnDefinitions { new(1, GridUnitType.Star), new(34, GridUnitType.Pixel) }, Children = { text, eye } }; Grid.SetColumn(eye, 1);
        var chrome = new Border { Name = "PART_Chrome", Child = content }; chrome[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty]; chrome[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty]; chrome[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty]; chrome[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
        var edge = new Border { Name = "PART_FocusEdge", Height = 3, VerticalAlignment = VerticalAlignment.Bottom, IsHitTestVisible = false };
        scope?.Register("PART_TextPresenter", presenter); scope?.Register("PART_Placeholder", placeholder); scope?.Register("PART_EyeGlyph", eyeIcon); scope?.Register("PART_RevealCell", eye); scope?.Register("PART_Chrome", chrome); scope?.Register("PART_FocusEdge", edge);
        return new Grid { Children = { chrome, edge } };
    });

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachRevealHandlers(); base.OnApplyTemplate(e); PasswordPresenterPart = e.NameScope.Find<TextPresenter>("PART_TextPresenter"); RevealPart = e.NameScope.Find<Button>("PART_RevealCell");
        AttachRevealHandlers(); SyncPresentation();
    }
    void AttachRevealHandlers() { if (RevealPart is null) return; RevealPart.PointerPressed += OnRevealPointerPressed; RevealPart.PointerReleased += OnRevealPointerReleased; RevealPart.PointerCaptureLost += OnRevealPointerCaptureLost; RevealPart.KeyDown += OnRevealKeyDown; RevealPart.KeyUp += OnRevealKeyUp; RevealPart.LostFocus += OnRevealLostFocus; }
    void DetachRevealHandlers() { if (RevealPart is null) return; RevealPart.PointerPressed -= OnRevealPointerPressed; RevealPart.PointerReleased -= OnRevealPointerReleased; RevealPart.PointerCaptureLost -= OnRevealPointerCaptureLost; RevealPart.KeyDown -= OnRevealKeyDown; RevealPart.KeyUp -= OnRevealKeyUp; RevealPart.LostFocus -= OnRevealLostFocus; }
}
