using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYColorPicker
{
    internal TextBox? HexField { get; private set; }
    internal TextBox? RedField { get; private set; }
    internal TextBox? GreenField { get; private set; }
    internal TextBox? BlueField { get; private set; }
    internal TextBox? AlphaField { get; private set; }
    internal Slider? HueSlider { get; private set; }
    internal Slider? AlphaSlider { get; private set; }
    internal Grid? ColorArea { get; private set; }
    internal Border? ColorAreaBase { get; private set; }
    internal Control? ColorAreaMarker { get; private set; }
    internal TextBlock? ErrorPart { get; private set; }
    internal bool PanelRefreshing { get; set; }

    internal Control BuildColorPanel()
    {
        ColorArea = BuildColorArea();
        HueSlider = new Slider { Minimum = 0, Maximum = 360, Height = 24, Background = HueBrush() }; HueSlider.ValueChanged += OnHueChanged;
        AlphaSlider = new Slider { Minimum = 0, Maximum = 255, Height = 24, Background = AlphaBrush() }; AlphaSlider.ValueChanged += OnAlphaChanged;
        HexField = Field("HEX", OnHexCommitted); RedField = Field("R", OnRedCommitted); GreenField = Field("G", OnGreenCommitted); BlueField = Field("B", OnBlueCommitted); AlphaField = Field("A", OnAlphaCommitted);
        ErrorPart = new TextBlock { Text = "颜色格式无效", IsVisible = false };
        var fields = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), RowDefinitions = new RowDefinitions("Auto,Auto,Auto"), ColumnSpacing = 8, RowSpacing = 6, Children = { Labeled("HEX", HexField), Labeled("R", RedField), Labeled("G", GreenField), Labeled("B", BlueField), Labeled("A", AlphaField) } };
        Grid.SetColumn(fields.Children[1], 1); Grid.SetRow(fields.Children[1], 0); Grid.SetRow(fields.Children[2], 1); Grid.SetColumn(fields.Children[2], 0); Grid.SetRow(fields.Children[3], 1); Grid.SetColumn(fields.Children[3], 1); Grid.SetRow(fields.Children[4], 2); Grid.SetColumn(fields.Children[4], 0);
        var panel = new StackPanel { Spacing = 6, MinWidth = 250, Children = { new TextBlock { Text = "颜色" }, ColorArea, new TextBlock { Text = "色相" }, HueSlider, new TextBlock { Text = "透明度" }, AlphaSlider, fields, ErrorPart } };
        SyncPanelValues(); return new Border { Padding = new Thickness(10), Child = panel };
    }
    TextBox Field(string label, Action commit) { var field = new TextBox { PlaceholderText = label, MinWidth = 82, Height = 28 }; field.KeyDown += (_, e) => { if (e.Key == Key.Enter) { commit(); e.Handled = true; } }; field.LostFocus += (_, _) => commit(); return field; }
    static StackPanel Labeled(string label, TextBox field) => new() { Spacing = 2, Children = { new TextBlock { Text = label }, field } };
}
