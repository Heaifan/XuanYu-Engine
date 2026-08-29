using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYSearchField
{
    internal static FuncControlTemplate<XYSearchField> CreateTemplate() => new((control, scope) =>
    {
        var glyph = new XYIcon { Name = "PART_SearchGlyph", Icon = XyuiVectorIcon.Search, Size = XyuiIconSize.Small };
        var presenter = new TextPresenter { Name = "PART_TextPresenter", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch };
        presenter[!TextPresenter.TextProperty] = control[!TextBox.TextProperty]; presenter[!TextPresenter.CaretIndexProperty] = control[!TextBox.CaretIndexProperty]; presenter[!TextPresenter.SelectionStartProperty] = control[!TextBox.SelectionStartProperty]; presenter[!TextPresenter.SelectionEndProperty] = control[!TextBox.SelectionEndProperty]; presenter[!TextPresenter.SelectionBrushProperty] = control[!TextBox.SelectionBrushProperty]; presenter[!TextPresenter.SelectionForegroundBrushProperty] = control[!TextBox.SelectionForegroundBrushProperty]; presenter[!TextPresenter.CaretBrushProperty] = control[!TextBox.CaretBrushProperty]; presenter[!TextPresenter.TextWrappingProperty] = control[!TextBox.TextWrappingProperty];
        var placeholder = new TextBlock { Name = "PART_Placeholder", IsHitTestVisible = false, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch };
        placeholder[!TextBlock.TextProperty] = control[!XYSearchField.PlaceholderProperty]; placeholder[!TextBlock.IsVisibleProperty] = new Binding("Text") { Mode = BindingMode.OneWay, Converter = new FuncValueConverter<string?, bool>(string.IsNullOrEmpty), Source = control };
        var text = new Grid { Children = { placeholder, presenter } };
        var clearIcon = new XYIcon { Name = "PART_ClearGlyph", Icon = XyuiVectorIcon.Clear, Size = XyuiIconSize.Small }; var clear = new Button { Name = "PART_ClearAction", Width = 28, Focusable = false, Padding = new Thickness(0), BorderThickness = new Thickness(0), Content = clearIcon, IsVisible = false };
        var filterIcon = new XYIcon { Name = "PART_FilterGlyph", Icon = XyuiVectorIcon.Filter, Size = XyuiIconSize.Small }; var filter = new Button { Name = "PART_FilterCell", Width = 35, MinWidth = 35, MaxWidth = 35, Padding = new Thickness(0), BorderThickness = new Thickness(0), Content = filterIcon, Focusable = true };
        var content = new Grid { ColumnDefinitions = new ColumnDefinitions { new(28, GridUnitType.Pixel), new(1, GridUnitType.Star), new(28, GridUnitType.Pixel), new(35, GridUnitType.Pixel) }, Children = { glyph, text, clear, filter } }; Grid.SetColumn(glyph, 0); Grid.SetColumn(text, 1); Grid.SetColumn(clear, 2); Grid.SetColumn(filter, 3);
        var chrome = new Border { Name = "PART_Chrome", Child = content }; chrome[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty]; chrome[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty]; chrome[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty]; chrome[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
        var edge = new Border { Name = "PART_FocusEdge", Height = 3, VerticalAlignment = VerticalAlignment.Bottom, IsHitTestVisible = false };
        scope?.Register("PART_SearchGlyph", glyph); scope?.Register("PART_TextPresenter", presenter); scope?.Register("PART_Placeholder", placeholder); scope?.Register("PART_ClearAction", clear); scope?.Register("PART_ClearGlyph", clearIcon); scope?.Register("PART_FilterCell", filter); scope?.Register("PART_FilterGlyph", filterIcon); scope?.Register("PART_Chrome", chrome); scope?.Register("PART_FocusEdge", edge);
        return new Grid { Children = { chrome, edge } };
    });

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (ClearActionPart is not null) ClearActionPart.Click -= OnClearClick;
        if (FilterPart is not null) FilterPart.Click -= OnFilterClick;
        base.OnApplyTemplate(e); ClearActionPart = e.NameScope.Find<Button>("PART_ClearAction"); FilterPart = e.NameScope.Find<Button>("PART_FilterCell");
        if (ClearActionPart is not null) ClearActionPart.Click += OnClearClick;
        if (FilterPart is not null) FilterPart.Click += OnFilterClick;
        SyncState();
    }
    void OnClearClick(object? sender, RoutedEventArgs e) { ClearSearch(); e.Handled = true; }
    void OnFilterClick(object? sender, RoutedEventArgs e) { RequestFilter(); e.Handled = true; }
}
