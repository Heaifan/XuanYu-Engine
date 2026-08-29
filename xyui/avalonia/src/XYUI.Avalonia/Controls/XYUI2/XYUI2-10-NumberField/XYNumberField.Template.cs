using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYNumberField
{
    internal static FuncControlTemplate<XYNumberField> CreateNumberTemplate() => new((control, scope) =>
    {
        var presenter = new TextPresenter { Name = "PART_TextPresenter", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch };
        presenter[!TextPresenter.TextProperty] = control[!TextBox.TextProperty]; presenter[!TextPresenter.CaretIndexProperty] = control[!TextBox.CaretIndexProperty];
        presenter[!TextPresenter.SelectionStartProperty] = control[!TextBox.SelectionStartProperty]; presenter[!TextPresenter.SelectionEndProperty] = control[!TextBox.SelectionEndProperty];
        presenter[!TextPresenter.SelectionBrushProperty] = control[!TextBox.SelectionBrushProperty]; presenter[!TextPresenter.SelectionForegroundBrushProperty] = control[!TextBox.SelectionForegroundBrushProperty];
        presenter[!TextPresenter.CaretBrushProperty] = control[!TextBox.CaretBrushProperty]; presenter[!TextPresenter.TextAlignmentProperty] = control[!TextBox.TextAlignmentProperty];
        var valueHost = new Border { Name = "PART_ValueHost", Padding = new Thickness(8, 0), ClipToBounds = true, Child = presenter };
        scope?.Register("PART_TextPresenter", presenter); scope?.Register("PART_ValueHost", valueHost);
        var suffix = new TextBlock { Name = "PART_Suffix", HorizontalAlignment = HorizontalAlignment.Stretch, TextAlignment = TextAlignment.Right, VerticalAlignment = VerticalAlignment.Center, IsHitTestVisible = false };
        suffix[!TextBlock.TextProperty] = control[!SuffixProperty]; scope?.Register("PART_Suffix", suffix);
        var suffixHost = new Border { Name = "PART_SuffixHost", MinWidth = 24, HorizontalAlignment = HorizontalAlignment.Stretch, Child = suffix };
        scope?.Register("PART_SuffixHost", suffixHost);
        var up = StepperButton(control, 1, true); var down = StepperButton(control, -1, false);
        var zones = new Grid { RowDefinitions = { new RowDefinition(1, GridUnitType.Star), new RowDefinition(1, GridUnitType.Star) }, Children = { up, down } };
        Grid.SetRow(down, 1);
        var stepper = new Border { Name = "PART_StepperCell", Width = 32, Opacity = 0, IsHitTestVisible = false, Background = Brushes.Transparent, Child = zones };
        scope?.Register("PART_StepperCell", stepper);
        var content = new Grid { ColumnDefinitions = { new ColumnDefinition(1, GridUnitType.Star), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(new GridLength(32)) }, Children = { valueHost, suffixHost, stepper } };
        Grid.SetColumn(suffixHost, 1); Grid.SetColumn(stepper, 2);
        var border = new Border { Child = content }; border[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty]; border[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty];
        border[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty]; border[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
        var edge = new Border { Name = "PART_FocusEdge", Height = 3, VerticalAlignment = VerticalAlignment.Bottom, IsHitTestVisible = false }; scope?.Register("PART_FocusEdge", edge);
        return new Grid { Children = { border, edge } };
    });

    static RepeatButton StepperButton(XYNumberField control, double direction, bool up)
    {
        var icon = new XYIcon { Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Small, RenderTransform = up ? new RotateTransform(180) : null };
        var button = new RepeatButton { Name = up ? "PART_UpButton" : "PART_DownButton", Padding = new Thickness(0), BorderThickness = new Thickness(0), Background = Brushes.Transparent, Focusable = false, Content = icon };
        icon[!XYIcon.StrokeProperty] = button[!TemplatedControl.ForegroundProperty]; button.Click += (_, _) => control.Adjust(direction * control.PrecisionStep); return button;
    }
}
