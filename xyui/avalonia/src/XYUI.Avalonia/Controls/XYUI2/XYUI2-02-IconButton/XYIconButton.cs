using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

// XYUI-2-02 Ghost Reveal：Category = Command，Selected ≠ Checked。
// IsSelected 由外部状态（如当前工具）驱动；点击本身不改变 Selected。
public class XYIconButton : Button
{
    bool _syncing;
    object? _consumerContent;
    public static readonly StyledProperty<XyuiVectorIcon?> IconProperty =
        AvaloniaProperty.Register<XYIconButton, XyuiVectorIcon?>(nameof(Icon));
    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<XYIconButton, bool>(nameof(IsSelected));

    public XyuiVectorIcon? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public XYIconButton() => Classes.Add("xyui-icon-button");

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsSelectedProperty) PseudoClasses.Set(":selected", e.GetNewValue<bool>());
        if (e.Property == IconProperty || e.Property == ContentControl.ContentProperty) SyncIconContent(e);
    }

    void SyncIconContent(AvaloniaPropertyChangedEventArgs e)
    {
        if (_syncing) return;
        if (e.Property == ContentControl.ContentProperty) _consumerContent = e.GetNewValue<object?>();
        _syncing = true;
        try { base.SetValue(ContentControl.ContentProperty, Icon is { } icon ? new XYIcon { Icon = icon } : _consumerContent); }
        finally { _syncing = false; }
    }
}
