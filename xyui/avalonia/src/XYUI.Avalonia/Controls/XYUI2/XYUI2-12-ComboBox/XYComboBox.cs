using Avalonia;
using Avalonia.Controls;
using System.Collections;

namespace XYUI.Avalonia.Controls;

public class XYComboBox : ComboBox
{
    public static readonly StyledProperty<string?> PlaceholderProperty = AvaloniaProperty.Register<XYComboBox, string?>(nameof(Placeholder));
    public static readonly StyledProperty<bool> IsCustomValueAllowedProperty = AvaloniaProperty.Register<XYComboBox, bool>(nameof(IsCustomValueAllowed));
    public string? Placeholder { get => GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public bool IsCustomValueAllowed { get => GetValue(IsCustomValueAllowedProperty); set => SetValue(IsCustomValueAllowedProperty, value); }
    IEnumerable? _allItems; bool _filtering;
    public XYComboBox() { Classes.Add("xyui-combo-box"); IsEditable = true; PropertyChanged += OnPropertyChanged; AddHandler(TextBox.TextChangedEvent, OnTextChanged); }
    void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) { if (e.Property == ItemsSourceProperty && !_filtering) _allItems = ItemsSource as IEnumerable; }
    void OnTextChanged(object? sender, TextChangedEventArgs e) { if (_allItems is null || _filtering) return; var query = Text ?? ""; var values = _allItems.Cast<object>().Where(x => (x?.ToString() ?? "").Contains(query, StringComparison.OrdinalIgnoreCase)).ToArray(); _filtering = true; ItemsSource = values; _filtering = false; IsDropDownOpen = true; }
}
