using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public partial class XYTextField
{
    internal static FuncControlTemplate<XYTextField> CreateTemplate() => new((control, scope) =>
    {
        var presenter = new TextPresenter { Name = "PART_TextPresenter", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch };
        presenter[!TextPresenter.TextProperty] = control[!TextBox.TextProperty];
        presenter[!TextPresenter.CaretIndexProperty] = control[!TextBox.CaretIndexProperty];
        presenter[!TextPresenter.SelectionStartProperty] = control[!TextBox.SelectionStartProperty];
        presenter[!TextPresenter.SelectionEndProperty] = control[!TextBox.SelectionEndProperty];
        presenter[!TextPresenter.SelectionBrushProperty] = control[!TextBox.SelectionBrushProperty];
        presenter[!TextPresenter.SelectionForegroundBrushProperty] = control[!TextBox.SelectionForegroundBrushProperty];
        presenter[!TextPresenter.CaretBrushProperty] = control[!TextBox.CaretBrushProperty];
        presenter[!TextPresenter.TextAlignmentProperty] = control[!TextBox.TextAlignmentProperty];
        presenter[!TextPresenter.TextWrappingProperty] = control[!TextBox.TextWrappingProperty];
        scope?.Register("PART_TextPresenter", presenter);
        var placeholder = new TextBlock { Name = "PART_Placeholder", IsHitTestVisible = false, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch };
        placeholder[!TextBlock.TextProperty] = control[!XYTextField.PlaceholderProperty];
        placeholder[!TextBlock.TextAlignmentProperty] = control[!TextBox.TextAlignmentProperty];
        placeholder[!TextBlock.IsVisibleProperty] = new Binding("Text") { Mode = BindingMode.OneWay,
            Converter = new FuncValueConverter<string?, bool>(string.IsNullOrEmpty), Source = control };
        var content = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        content.Children.Add(placeholder); content.Children.Add(presenter);
        var inner = new Border { Child = content, Padding = new Thickness(8, 0) };
        inner[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty];
        var root = new Border { Child = inner };
        root[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty];
        root[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty];
        root[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
        var edge = new Border { Name = "PART_FocusEdge", Height = 3, VerticalAlignment = VerticalAlignment.Bottom, IsHitTestVisible = false };
        scope?.Register("PART_FocusEdge", edge);
        var chrome = new Grid { Children = { root, edge } };
        return chrome;
    });
}
