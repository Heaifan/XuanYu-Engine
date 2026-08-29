using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public partial class XYReferenceProperty
{
    internal void UpdateLayoutMode()
    {
        if (RowPart is null || ReferenceFieldPart is null || ContentPart is null || ActionsPart is null || IdentityPart is null) return;
        var width = Bounds.Width; var narrow = width > 0 && width < XYPropertyLayoutMetrics.CompactBreakpoint; var wide = width >= XYPropertyLayoutMetrics.WideBreakpoint;
        Classes.Set("xyui-reference-wide", wide); Classes.Set("xyui-reference-compact", !wide && !narrow); Classes.Set("xyui-reference-narrow", narrow);
        XYPropertyLayoutMetrics.ConfigureRow(RowPart, LabelPart!, ReferenceFieldPart, width); IdentityPart.IsVisible = wide;
        ContentPart.ColumnDefinitions.Clear(); ContentPart.RowDefinitions.Clear();
        if (narrow)
        {
            ContentPart.ColumnDefinitions.Add(new ColumnDefinition(25, GridUnitType.Pixel)); ContentPart.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star)); ContentPart.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); ContentPart.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetColumn(NamePart!, 1); Grid.SetRow(NamePart!, 0); Grid.SetColumn(ActionsPart, 0); Grid.SetColumnSpan(ActionsPart, 2); Grid.SetRow(ActionsPart, 1);
        }
        else
        {
            ContentPart.ColumnDefinitions.Add(new ColumnDefinition(25, GridUnitType.Pixel)); ContentPart.ColumnDefinitions.Add(new ColumnDefinition(1, GridUnitType.Star)); ContentPart.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            Grid.SetColumn(NamePart!, 1); Grid.SetRow(NamePart!, 0); Grid.SetColumn(ActionsPart, 2); Grid.SetRow(ActionsPart, 0);
        }
    }
}
