using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public enum XYColorPickerMode { RGB, RGBA }

public partial class XYColorPicker : TemplatedControl
{
    public static readonly StyledProperty<global::Avalonia.Media.Color> ColorProperty = AvaloniaProperty.Register<XYColorPicker, global::Avalonia.Media.Color>(nameof(Color), global::Avalonia.Media.Color.FromArgb(255, 50, 111, 138));
    public static readonly StyledProperty<XYColorPickerMode> ModeProperty = AvaloniaProperty.Register<XYColorPicker, XYColorPickerMode>(nameof(Mode), XYColorPickerMode.RGBA);
    public static readonly StyledProperty<bool> IsOpenProperty = AvaloniaProperty.Register<XYColorPicker, bool>(nameof(IsOpen));
    public global::Avalonia.Media.Color Color { get => GetValue(ColorProperty); set => SetValue(ColorProperty, value); }
    public XYColorPickerMode Mode { get => GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
    public event EventHandler? ColorChanged;
    internal Popup? PopupPart { get; set; }
    internal Border? PopupSurfacePart { get; set; }
    internal Border? SwatchPart { get; set; }
    internal TextBlock? ValuePart { get; set; }
    internal XYIcon? ChevronPart { get; set; }
    internal double Hue { get; private set; }
    internal double Saturation { get; private set; }
    internal double Value { get; private set; }

    public XYColorPicker() { Classes.Add("xyui-color-picker"); Focusable = true; UpdateHsv(Color); }
    protected override void OnKeyDown(KeyEventArgs e) { if (e.Key == Key.Escape && IsOpen) { IsOpen = false; e.Handled = true; return; } base.OnKeyDown(e); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ColorProperty) { UpdateHsv(Color); SyncVisuals(); ColorChanged?.Invoke(this, EventArgs.Empty); }
        if (change.Property == ModeProperty) SyncVisuals();
        if (change.Property == IsOpenProperty) { Classes.Set("xyui-color-open", IsOpen); if (IsOpen) OpenPanel(); else ClosePanel(); }
        if (change.Property == IsEnabledProperty && !IsEnabled) IsOpen = false;
    }
    internal string DisplayValue() => Mode == XYColorPickerMode.RGBA ? $"{HexValue()} · {AlphaPercent()}%" : HexValue();
    internal string HexValue() => $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";
    internal int AlphaPercent() => (Color.A * 100 + 127) / 255;
    internal void SetColor(global::Avalonia.Media.Color color) { if (Color != color) Color = color; else SyncVisuals(); }
}
