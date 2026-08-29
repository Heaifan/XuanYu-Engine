using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public enum XYTextAreaMode { Standard, Editor }

public class XYTextArea : XyuiEditableTextBox
{
    public static readonly StyledProperty<XYTextAreaMode> ModeProperty = AvaloniaProperty.Register<XYTextArea, XYTextAreaMode>(nameof(Mode));
    public static readonly StyledProperty<string?> PlaceholderProperty = TextBox.PlaceholderTextProperty.AddOwner<XYTextArea>();
    public XYTextAreaMode Mode { get => GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    public string? Placeholder { get => GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public int LineCount => string.IsNullOrEmpty(Text) ? 1 : Text!.Split('\n').Length;
    public int CharacterCount => Text?.Length ?? 0;
    public XYTextArea() { Classes.Add("xyui-text-area"); AcceptsReturn = true; TextWrapping = TextWrapping.Wrap; MinHeight = 54; }
}
