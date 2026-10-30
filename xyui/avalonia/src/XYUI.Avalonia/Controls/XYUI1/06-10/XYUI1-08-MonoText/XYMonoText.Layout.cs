using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMonoText
{
    void RebuildRows()
    {
        Children.Clear();
        RowDefinitions.Clear();
        for (var rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
        {
            RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            AddCell(Label(Rows[rowIndex].Label), rowIndex, 0);
            AddCell(Value(Rows[rowIndex].Value), rowIndex, 2);
            AddCell(Unit(Rows[rowIndex].Unit), rowIndex, 4);
        }
    }

    void AddCell(TextBlock cell, int row, int column)
    {
        SetRow(cell, row);
        SetColumn(cell, column);
        Children.Add(cell);
    }

    static TextBlock Label(string text) => Cell(text, "xyui-mono-data-label", TextAlignment.Left,
        HorizontalAlignment.Left, TextTrimming.None);

    static TextBlock Value(string text) => Cell(text, "xyui-mono-data-value", TextAlignment.Right,
        HorizontalAlignment.Right, TextTrimming.None);

    static TextBlock Unit(string text) => Cell(text, "xyui-mono-data-unit", TextAlignment.Left,
        HorizontalAlignment.Left, TextTrimming.None);

    static TextBlock Cell(string text, string styleClass, TextAlignment alignment,
        HorizontalAlignment horizontalAlignment, TextTrimming trimming) => new()
    {
        Text = text,
        Classes = { styleClass },
        TextAlignment = alignment,
        HorizontalAlignment = horizontalAlignment,
        VerticalAlignment = VerticalAlignment.Center,
        TextWrapping = TextWrapping.NoWrap,
        TextTrimming = trimming
    };
}
