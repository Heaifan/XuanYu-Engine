using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public partial class XYTextArea
{
    internal static FuncControlTemplate<XYTextArea> CreateTemplate() => new((control, scope) =>
    {
        var presenter = new TextPresenter { Name = "PART_TextPresenter", VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Stretch };
        presenter.Bind(TextPresenter.TextProperty, new Binding(nameof(TextBox.Text)) { Source = control, Mode = BindingMode.TwoWay }); presenter.Bind(TextPresenter.CaretIndexProperty, new Binding(nameof(TextBox.CaretIndex)) { Source = control, Mode = BindingMode.TwoWay }); presenter.Bind(TextPresenter.SelectionStartProperty, new Binding(nameof(TextBox.SelectionStart)) { Source = control, Mode = BindingMode.TwoWay }); presenter.Bind(TextPresenter.SelectionEndProperty, new Binding(nameof(TextBox.SelectionEnd)) { Source = control, Mode = BindingMode.TwoWay }); presenter[!TextPresenter.SelectionBrushProperty] = control[!TextBox.SelectionBrushProperty]; presenter[!TextPresenter.SelectionForegroundBrushProperty] = control[!TextBox.SelectionForegroundBrushProperty]; presenter[!TextPresenter.CaretBrushProperty] = control[!TextBox.CaretBrushProperty]; presenter[!TextPresenter.TextWrappingProperty] = control[!TextBox.TextWrappingProperty];
        var placeholder = new TextBlock { Name = "PART_Placeholder", IsHitTestVisible = false, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Stretch };
        placeholder[!TextBlock.TextProperty] = control[!XYTextArea.PlaceholderProperty]; placeholder[!TextBlock.IsVisibleProperty] = new Binding("Text") { Mode = BindingMode.OneWay, Converter = new FuncValueConverter<string?, bool>(string.IsNullOrEmpty), Source = control };
        var content = new Grid { Children = { placeholder, presenter } }; var inner = new Border { Padding = new Thickness(8), Child = content };
        var scroll = new ScrollViewer { Name = "PART_ScrollViewer", VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, Content = inner };
        var barType = new TextBlock { Name = "PART_EditorType", Text = control.EditorType }; var lines = new TextBlock { Name = "PART_LineCount" }; var chars = new TextBlock { Name = "PART_CharacterCount" }; var bar = new Border { Name = "PART_EditorBar", Height = 24, Child = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12, Children = { barType, lines, chars, new TextBlock { Text = "Modified" } } } };
        var barRow = new RowDefinition { Height = new GridLength(0) }; barRow.Bind(RowDefinition.HeightProperty, new Binding(nameof(XYTextArea.Mode)) { Source = control, Converter = new FuncValueConverter<XYTextAreaMode, GridLength>(mode => mode == XYTextAreaMode.Editor ? new GridLength(24) : new GridLength(0)) });
        var body = new Grid { RowDefinitions = new RowDefinitions { barRow, new(1, GridUnitType.Star) }, Children = { bar, scroll } }; Grid.SetRow(scroll, 1);
        var chrome = new Border { Name = "PART_Chrome", Child = body }; chrome[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty]; chrome[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty]; chrome[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty]; chrome[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
        var edge = new Border { Name = "PART_FocusEdge", Height = 3, VerticalAlignment = VerticalAlignment.Bottom, IsHitTestVisible = false }; scope?.Register("PART_TextPresenter", presenter); scope?.Register("PART_Placeholder", placeholder); scope?.Register("PART_ScrollViewer", scroll); scope?.Register("PART_EditorBar", bar); scope?.Register("PART_EditorType", barType); scope?.Register("PART_LineCount", lines); scope?.Register("PART_CharacterCount", chars); scope?.Register("PART_FocusEdge", edge);
        return new Grid { Children = { chrome, edge } };
    });

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e); TextPresenterPart = e.NameScope.Find<TextPresenter>("PART_TextPresenter"); ScrollViewerPart = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer"); EditorBarPart = e.NameScope.Find<Border>("PART_EditorBar"); LineCountPart = e.NameScope.Find<TextBlock>("PART_LineCount"); CharacterCountPart = e.NameScope.Find<TextBlock>("PART_CharacterCount");
        UpdateEditorBar(); QueueLayout();
    }
}
