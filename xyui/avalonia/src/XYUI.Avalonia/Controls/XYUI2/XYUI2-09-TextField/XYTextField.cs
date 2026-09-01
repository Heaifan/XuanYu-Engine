using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Controls;

public partial class XYTextField : XyuiEditableTextBox
{
    public static readonly StyledProperty<string?> PlaceholderProperty =
        TextBox.PlaceholderTextProperty.AddOwner<XYTextField>();
    public static readonly StyledProperty<bool> IsErrorProperty =
        AvaloniaProperty.Register<XYTextField, bool>(nameof(IsError));

    public string? Placeholder { get => GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public bool IsError { get => GetValue(IsErrorProperty); set => SetValue(IsErrorProperty, value); }

    public XYTextField() { Classes.Add("xyui-text-field"); XyuiSizingScope.Attach(this); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsErrorProperty) PseudoClasses.Set(":error", change.GetNewValue<bool>());
    }

}
