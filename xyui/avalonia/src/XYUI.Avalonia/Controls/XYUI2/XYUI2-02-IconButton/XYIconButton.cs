using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Controls;

// XYUI-2-02 Ghost Reveal：Category = Command，Selected ≠ Checked。
// IsSelected 由外部状态（如当前工具）驱动；点击本身不改变 Selected。
public class XYIconButton : Button
{
    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<XYIconButton, bool>(nameof(IsSelected));

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public XYIconButton() { Classes.Add("xyui-icon-button"); XyuiSizingScope.Attach(this, iconOnly: true); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsSelectedProperty) PseudoClasses.Set(":selected", e.GetNewValue<bool>());
    }
}
