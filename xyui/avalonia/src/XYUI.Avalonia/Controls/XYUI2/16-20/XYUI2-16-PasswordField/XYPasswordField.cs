using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace XYUI.Avalonia.Controls;

public partial class XYPasswordField : XyuiEditableTextBox
{
    public static readonly StyledProperty<string?> PlaceholderProperty = TextBox.PlaceholderTextProperty.AddOwner<XYPasswordField>();
    public string? Placeholder { get => GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public string Password { get => Text ?? string.Empty; set => Text = value; }
    public bool IsRevealed { get; private set; }
    public event EventHandler<RoutedEventArgs>? PasswordChanged;
    internal TextPresenter? PasswordPresenterPart { get; private set; }
    internal Border? TextPaddingPart { get; private set; }
    internal Button? RevealPart { get; private set; }

    public XYPasswordField() { Classes.Add("xyui-password-field"); PasswordChar = '●'; TextChanged += OnPasswordTextChanged; }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsEnabledProperty && !IsEnabled) ForceHidePassword();
    }
    void OnPasswordTextChanged(object? sender, TextChangedEventArgs e) { SyncPresentation(); PasswordChanged?.Invoke(this, new RoutedEventArgs()); }
    internal void SetRevealed(bool value) { if (!IsEnabled && value) return; IsRevealed = value; Classes.Set("xyui-password-holding", value); SyncPresentation(); }
    internal void SyncPresentation()
    {
        if (PasswordPresenterPart is not null) PasswordPresenterPart.Text = IsRevealed ? Text ?? string.Empty : new string('●', Text?.Length ?? 0);
        if (RevealPart is not null) RevealPart.IsEnabled = IsEnabled;
    }
    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e); global::Avalonia.Threading.Dispatcher.UIThread.Post(() => { if (RevealPart?.IsFocused != true) ForceHidePassword(); });
    }
}
