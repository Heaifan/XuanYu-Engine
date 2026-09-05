using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYColorPicker
{
    internal XYTextField? HexField { get; private set; }
    internal XYNumberField? RedField { get; private set; }
    internal XYNumberField? GreenField { get; private set; }
    internal XYNumberField? BlueField { get; private set; }
    internal XYNumberField? AlphaField { get; private set; }
    internal XYSlider? HueSlider { get; private set; }
    internal XYSlider? AlphaSlider { get; private set; }
    internal Control? AlphaPanel { get; private set; }
    internal Control? AlphaFieldPanel { get; private set; }
    internal Grid? ColorArea { get; private set; }
    internal Border? ColorAreaBase { get; private set; }
    internal Control? ColorAreaMarker { get; private set; }
    internal TextBlock? ErrorPart { get; private set; }
    internal bool PanelRefreshing { get; set; }

    internal Control BuildColorPanel()
    {
        ColorArea = BuildColorArea();
        HueSlider = Slider(360); HueSlider.PropertyChanged += OnHueChanged;
        AlphaSlider = Slider(255); AlphaSlider.PropertyChanged += OnAlphaChanged;
        HexField = Field("HEX", OnHexCommitted); RedField = NumberField("R", 'R'); GreenField = NumberField("G", 'G'); BlueField = NumberField("B", 'B'); AlphaField = NumberField("A", 'A');
        ErrorPart = new TextBlock { Text = "颜色格式无效", IsVisible = false };
        var alphaPanel = Labeled("透明度", AlphaSlider); var alphaFieldPanel = Labeled("A", AlphaField); AlphaPanel = alphaPanel; AlphaFieldPanel = alphaFieldPanel;
        var fields = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), RowDefinitions = new RowDefinitions("Auto,Auto,Auto"), ColumnSpacing = 8, RowSpacing = 6, Children = { Labeled("HEX", HexField), Labeled("R", RedField), Labeled("G", GreenField), Labeled("B", BlueField), alphaFieldPanel } };
        Grid.SetColumn(fields.Children[1], 1); Grid.SetRow(fields.Children[1], 0); Grid.SetRow(fields.Children[2], 1); Grid.SetColumn(fields.Children[2], 0); Grid.SetRow(fields.Children[3], 1); Grid.SetColumn(fields.Children[3], 1); Grid.SetRow(fields.Children[4], 2); Grid.SetColumn(fields.Children[4], 0);
        var panel = new StackPanel { Spacing = 6, MinWidth = 250, Children = { new TextBlock { Text = "颜色" }, ColorArea, new TextBlock { Text = "色相" }, HueSlider, alphaPanel, fields, ErrorPart } };
        SyncPanelValues(); SyncModeVisibility(); return new Border { Padding = new Thickness(10), Child = panel };
    }
    XYSlider Slider(double maximum) => new() { Minimum = 0, Maximum = maximum, Step = 1, LargeStep = 10, SmallStep = 1, DecimalPlaces = 0, IsNumberFieldVisible = false };
    XYTextField Field(string label, Action commit) { var field = new XYTextField { Placeholder = label, MinWidth = 82, Height = 28 }; field.KeyDown += (_, e) => { if (e.Key == Key.Enter) { commit(); e.Handled = true; } }; field.LostFocus += (_, _) => commit(); return field; }
    XYNumberField NumberField(string label, char channel) { var field = new XYNumberField { Placeholder = label, Minimum = 0, Maximum = 255, Step = 1, LargeStep = 10, SmallStep = 1, DecimalPlaces = 0, IsScrubEnabled = true, MinWidth = 82, Height = 28 }; field.PropertyChanged += (_, e) => OnNumberChanged(field, channel, e); return field; }
    static StackPanel Labeled(string label, Control field) => new() { Spacing = 2, Children = { new TextBlock { Text = label }, field } };
}
