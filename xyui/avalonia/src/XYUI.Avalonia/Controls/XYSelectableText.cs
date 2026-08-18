using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed class XYSelectableText : SelectableTextBlock
{
    string _baseText = "";
    bool _hovering;
    public XYSelectableText()
    {
        Classes.Add("xyui-selectable-text");
        PointerEntered += (_, _) => SetHover(true);
        PointerExited += (_, _) => SetHover(false);
    }
    public string CanonicalId => "XYUI-1-21";
    public string CopyGlyph => "⧉";
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty && !_hovering) _baseText = change.GetNewValue<string>();
    }
    void SetHover(bool value)
    {
        _hovering = value; Text = value ? $"{_baseText}  {CopyGlyph}" : _baseText;
    }
}
