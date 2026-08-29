using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using System.Collections;

namespace XYUI.Avalonia.Controls;

public partial class XYEnumProperty : TemplatedControl
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYEnumProperty, string>(nameof(Label), "枚举");
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty = AvaloniaProperty.Register<XYEnumProperty, IEnumerable?>(nameof(ItemsSource));
    public static readonly StyledProperty<object?> SelectedItemProperty = AvaloniaProperty.Register<XYEnumProperty, object?>(nameof(SelectedItem));
    public static readonly StyledProperty<int> SelectedIndexProperty = AvaloniaProperty.Register<XYEnumProperty, int>(nameof(SelectedIndex), -1);
    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<XYEnumProperty, bool>(nameof(IsReadOnly));
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public IEnumerable? ItemsSource { get => GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public object? SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }
    public int SelectedIndex { get => GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }
    public bool IsReadOnly { get => GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }
    public event EventHandler? SelectionChanged;
    internal TextBlock? LabelPart { get; set; }
    internal XYSelect? SelectPart { get; set; }
    internal Grid? RowPart { get; set; }
    internal bool Syncing { get; set; }

    public XYEnumProperty() { Classes.Add("xyui-enum-property"); Focusable = true; }
    protected override void OnSizeChanged(SizeChangedEventArgs e) { base.OnSizeChanged(e); if (RowPart is not null && SelectPart is not null) XYPropertyLayoutMetrics.ConfigureRow(RowPart, LabelPart!, SelectPart, Bounds.Width); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == LabelProperty || change.Property == ItemsSourceProperty || change.Property == SelectedItemProperty || change.Property == SelectedIndexProperty || change.Property == IsReadOnlyProperty || change.Property == IsEnabledProperty) SyncParts();
    }
    internal void OnSelectChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (Syncing || SelectPart is null) return;
        if (IsReadOnly || !IsEnabled) { SyncParts(); return; }
        Syncing = true; SelectedItem = SelectPart.SelectedItem; SelectedIndex = SelectPart.SelectedIndex; Syncing = false; SelectionChanged?.Invoke(this, EventArgs.Empty);
    }
    internal void SyncParts()
    {
        if (LabelPart is not null) LabelPart.Text = Label;
        if (SelectPart is null) return;
        Syncing = true; SelectPart.ItemsSource = ItemsSource; SelectPart.SelectedItem = SelectedItem; SelectPart.SelectedIndex = SelectedIndex; SelectPart.IsEnabled = IsEnabled && !IsReadOnly; Syncing = false;
    }
}
