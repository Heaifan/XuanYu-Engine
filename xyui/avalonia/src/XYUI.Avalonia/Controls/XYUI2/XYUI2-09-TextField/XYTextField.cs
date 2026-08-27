using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYTextField : TextBox
{
    bool _selectAllOnPointerRelease;
    public static readonly StyledProperty<string?> PlaceholderProperty =
        TextBox.PlaceholderTextProperty.AddOwner<XYTextField>();
    public static readonly StyledProperty<bool> IsErrorProperty =
        AvaloniaProperty.Register<XYTextField, bool>(nameof(IsError));

    public string? Placeholder { get => GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public bool IsError { get => GetValue(IsErrorProperty); set => SetValue(IsErrorProperty, value); }

    public XYTextField() => Classes.Add("xyui-text-field");
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsErrorProperty) PseudoClasses.Set(":error", change.GetNewValue<bool>());
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        _selectAllOnPointerRelease = !IsKeyboardFocusWithin && !IsReadOnly && !string.IsNullOrEmpty(Text);
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_selectAllOnPointerRelease) { _selectAllOnPointerRelease = false; SelectAll(); }
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        if (!IsReadOnly && !_selectAllOnPointerRelease && !string.IsNullOrEmpty(Text)) SelectAll();
    }
}
