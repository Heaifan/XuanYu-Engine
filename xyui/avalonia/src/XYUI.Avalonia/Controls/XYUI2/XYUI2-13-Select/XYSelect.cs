using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYSelect : ComboBox
{
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<XYSelect, string?>(nameof(Placeholder));
    public string? Placeholder { get => GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public new bool IsDropDownOpen { get => _isDropDownOpen; set => SetDropDownOpen(value); }
    internal TextBlock? ValuePart { get; private set; }
    internal Border? ValueSurfacePart { get; private set; }
    internal Border? ChevronCellPart { get; private set; }
    internal XYIcon? ChevronPart { get; private set; }
    internal Popup? PopupPart { get; private set; }
    internal ListBox? ListPart { get; private set; }
    internal bool IsKeyboardNavigating { get; set; }
    bool _isDropDownOpen;
    public XYSelect() { Classes.Add("xyui-select"); IsEditable = false; Focusable = true; SelectionChanged += OnSelectionChanged; }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        OnSelectKeyDown(this, e);
        if (!e.Handled) base.OnKeyDown(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsEnabledProperty && !change.GetNewValue<bool>()) ClosePopupForLifecycle();
        if (change.Property == ItemsSourceProperty || change.Property == SelectedIndexProperty || change.Property == SelectedItemProperty || change.Property == PlaceholderProperty)
        { SyncParts(); }
    }
}
