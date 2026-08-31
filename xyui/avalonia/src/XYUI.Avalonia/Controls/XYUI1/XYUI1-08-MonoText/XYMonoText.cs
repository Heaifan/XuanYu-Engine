using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMonoText : Grid
{
    public const double LabelValueGap = XyuiSpatialTokens.Space4 + XyuiSpatialTokens.Space1;
    public const double ValueUnitGap = XyuiSpatialTokens.Space2;
    public ObservableCollection<XYMonoDataRow> Rows { get; } = [];
    public string CanonicalId => "XYUI-1-08";

    public XYMonoText()
    {
        HorizontalAlignment = HorizontalAlignment.Left;
        Classes.Add("xyui-1-component");
        Classes.Add("xyui-mono-text");
        ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        ColumnDefinitions.Add(new ColumnDefinition(new GridLength(LabelValueGap)));
        ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        ColumnDefinitions.Add(new ColumnDefinition(new GridLength(ValueUnitGap)));
        ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        Rows.CollectionChanged += OnRowsChanged;
    }

    void OnRowsChanged(object? sender, NotifyCollectionChangedEventArgs e) => RebuildRows();
}
