using Avalonia;

namespace XYUI.Avalonia.Controls;

public abstract class XyuiMarkedTextComponent : XyuiTextComponent
{
    bool _formatting;

    protected XyuiMarkedTextComponent(string className) : base(className) { }

    protected virtual string FormatText(string value) => value;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != TextProperty || _formatting) return;
        _formatting = true;
        Text = FormatText(change.GetNewValue<string>());
        _formatting = false;
    }
}
