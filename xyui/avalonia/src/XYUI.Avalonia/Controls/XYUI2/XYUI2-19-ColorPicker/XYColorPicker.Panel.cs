using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public partial class XYColorPicker
{
    static LinearGradientBrush HueBrush() => new() { StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative), GradientStops = [new GradientStop(Colors.Red, 0), new GradientStop(Colors.Yellow, .17), new GradientStop(Colors.Lime, .33), new GradientStop(Colors.Cyan, .5), new GradientStop(Colors.Blue, .67), new GradientStop(Colors.Magenta, .83), new GradientStop(Colors.Red, 1)] };
    LinearGradientBrush AlphaBrush() => new() { StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative), GradientStops = [new GradientStop(Color.FromArgb(0, Color.R, Color.G, Color.B), 0), new GradientStop(Color.FromArgb(255, Color.R, Color.G, Color.B), 1)] };
    Grid BuildColorArea()
    {
        var baseLayer = new Border(); var white = new Border { Background = new LinearGradientBrush { StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative), GradientStops = [new GradientStop(Colors.White, 0), new GradientStop(Colors.Transparent, 1)] }, IsHitTestVisible = false };
        var black = new Border { Background = new LinearGradientBrush { StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative), GradientStops = [new GradientStop(Colors.Transparent, 0), new GradientStop(Colors.Black, 1)] }, IsHitTestVisible = false };
        var marker = new Ellipse { Width = 10, Height = 10, Stroke = Brushes.White, StrokeThickness = 2, IsHitTestVisible = false }; ColorAreaBase = baseLayer; ColorAreaMarker = marker;
        var area = new Grid { Width = 250, Height = 130, Children = { baseLayer, white, black, marker } }; area.PointerPressed += OnAreaPressed; area.PointerMoved += OnAreaMoved; area.PointerReleased += OnAreaReleased; return area;
    }
    void OnAreaPressed(object? sender, PointerPressedEventArgs e) { if (!IsEnabled) return; e.Pointer.Capture(ColorArea); SetFromArea(e.GetPosition(ColorArea!)); e.Handled = true; }
    void OnAreaMoved(object? sender, PointerEventArgs e) { if (e.Pointer.Captured == ColorArea && e.GetCurrentPoint(ColorArea).Properties.IsLeftButtonPressed) SetFromArea(e.GetPosition(ColorArea!)); }
    void OnAreaReleased(object? sender, PointerReleasedEventArgs e) { if (e.Pointer.Captured == ColorArea) e.Pointer.Capture(null); }
    void SetFromArea(Point point) { var s = Math.Clamp(point.X / Math.Max(1, ColorArea!.Bounds.Width), 0, 1); var v = 1 - Math.Clamp(point.Y / Math.Max(1, ColorArea.Bounds.Height), 0, 1); SetFromHsv(Hue, s, v); }
    void OnHueChanged(object? sender, RangeBaseValueChangedEventArgs e) { if (!PanelRefreshing) SetFromHsv(e.NewValue, Saturation, Value); }
    void OnAlphaChanged(object? sender, RangeBaseValueChangedEventArgs e) { if (!PanelRefreshing) SetColor(Color.FromArgb((byte)Math.Clamp(Math.Round(e.NewValue), 0, 255), Color.R, Color.G, Color.B)); }
    internal void SyncPanelValues()
    {
        if (HueSlider is null) return; PanelRefreshing = true; HueSlider.Value = Hue; AlphaSlider!.Value = Color.A; HexField!.Text = HexValue(); RedField!.Text = Color.R.ToString(); GreenField!.Text = Color.G.ToString(); BlueField!.Text = Color.B.ToString(); AlphaField!.Text = Color.A.ToString(); if (ColorAreaBase is not null) ColorAreaBase.Background = new SolidColorBrush(HsvToColor(Hue, 1, 1, 255)); if (ColorAreaMarker is not null && ColorArea is not null) { Canvas.SetLeft(ColorAreaMarker, Saturation * ColorArea.Bounds.Width - 5); Canvas.SetTop(ColorAreaMarker, (1 - Value) * ColorArea.Bounds.Height - 5); } PanelRefreshing = false;
    }
}
