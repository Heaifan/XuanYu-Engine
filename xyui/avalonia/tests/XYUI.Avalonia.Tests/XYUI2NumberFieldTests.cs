using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2NumberFieldTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2NumberFieldTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void NumberField_syncs_value_suffix_and_clamps() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYNumberField { Value = 125, Suffix = "%", Minimum = 0, Maximum = 100 };
        var window = XyuiBatchTestHost.Show(field); Assert.Equal(100, field.Value); Assert.Equal("100.00", field.Text); Assert.Equal("%", field.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Name == "PART_Suffix").Text); field.Text = "72"; field.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter });
        Assert.Equal(72, field.Value); Assert.False(field.IsError); window.Close();
    });

    [Fact]
    public void NumberField_keyboard_uses_modifier_steps_and_escape_reverts() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYNumberField { Value = 10, Step = 2, LargeStep = 10, SmallStep = .5 };
        var window = XyuiBatchTestHost.Show(field); field.Focus(); Raise(field, Key.Up); Assert.Equal(10.01, field.Value, 10); Raise(field, Key.Up, KeyModifiers.Shift); Assert.Equal(20.01, field.Value, 10); Raise(field, Key.Down, KeyModifiers.Control); Assert.Equal(19.51, field.Value, 10); field.Text = "bad"; Raise(field, Key.Enter); Assert.Equal("19.51", field.Text); field.Text = "99"; Raise(field, Key.Escape); Assert.Equal("10.00", field.Text); window.Close();
    });

    [Fact]
    public void NumberField_has_hidden_then_visible_stepper_and_real_buttons() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYNumberField { Width = 200, Value = 10, Step = 2 }; var window = XyuiBatchTestHost.Show(field);
        var stepper = field.GetVisualDescendants().Single(x => x.Name == "PART_StepperCell"); Assert.Equal(0, stepper.Opacity); field.Focus(); Assert.Equal(1, stepper.Opacity);
        var up = field.GetVisualDescendants().OfType<Button>().Single(x => x.Name == "PART_UpButton"); up.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(10.01, field.Value, 10); window.Close();
    });

    [Fact]
    public void NumberField_scrub_requires_threshold_and_captures_pointer() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYNumberField { Width = 200, Value = 10, Step = 2 }; var window = XyuiBatchTestHost.Show(field);
        Assert.True(field.ValueHost?.Bounds.Width > 0); var start = field.ValueHost!.TranslatePoint(new Point(10, 16), window)!.Value;
        var far = new Point(start.X + 20, start.Y);
        window.MouseDown(start, MouseButton.Left); Assert.True(field.IsScrubArmed); window.MouseMove(start); window.MouseMove(far); Dispatcher.UIThread.RunJobs(); Assert.True(field.IsScrubbing); Assert.True(field.Value > 10);
        window.MouseUp(far, MouseButton.Left); Assert.False(field.IsScrubbing); window.Close();
    });

    [Fact]
    public void NumberField_gallery_is_real_and_defaults_to_component() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-10"); var fields = preview.GetVisualDescendants().OfType<XYNumberField>().ToArray();
        Assert.True(fields.Length >= 7); Assert.Contains(fields, x => x.Suffix == "%"); Assert.Contains(fields, x => !x.IsEnabled); Assert.Contains(fields, x => x.Minimum == 0 && x.Maximum == 100); preview.ApplyStyling();
    });

    static void Raise(XYNumberField field, Key key, KeyModifiers modifiers = KeyModifiers.None) => field.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key, KeyModifiers = modifiers });
}
