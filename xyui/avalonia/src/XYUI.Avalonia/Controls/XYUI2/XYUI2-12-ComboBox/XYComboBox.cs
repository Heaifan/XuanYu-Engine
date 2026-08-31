using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.Collections;

namespace XYUI.Avalonia.Controls;

public partial class XYComboBox : ComboBox
{
    public static readonly StyledProperty<string?> PlaceholderProperty = AvaloniaProperty.Register<XYComboBox, string?>(nameof(Placeholder));
    public static readonly StyledProperty<bool> IsCustomValueAllowedProperty = AvaloniaProperty.Register<XYComboBox, bool>(nameof(IsCustomValueAllowed));
    public static readonly StyledProperty<bool> IsErrorProperty = AvaloniaProperty.Register<XYComboBox, bool>(nameof(IsError));
    public string? Placeholder { get => GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public bool IsCustomValueAllowed { get => GetValue(IsCustomValueAllowedProperty); set => SetValue(IsCustomValueAllowedProperty, value); }
    public bool IsError { get => GetValue(IsErrorProperty); set => SetValue(IsErrorProperty, value); }
    public new bool IsDropDownOpen { get => _isDropDownOpen; set => SetDropDownOpen(value); }
    internal IReadOnlyList<object> FilteredItems { get; private set; } = [];
    internal XYTextField? TextFieldPart { get; private set; }
    internal Button? ChevronPart { get; private set; }
    internal Popup? PopupPart { get; private set; }
    internal ListBox? ListPart { get; private set; }
    internal bool IsKeyboardSelecting { get; set; }
    internal bool ShowingAllItems { get; set; }
    internal bool SyncingText { get; set; }
    bool _isDropDownOpen;
    public XYComboBox() { Classes.Add("xyui-combo-box"); IsEditable = true; PropertyChanged += OnComboPropertyChanged; }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsErrorProperty) PseudoClasses.Set(":error", change.GetNewValue<bool>());
    }
}
