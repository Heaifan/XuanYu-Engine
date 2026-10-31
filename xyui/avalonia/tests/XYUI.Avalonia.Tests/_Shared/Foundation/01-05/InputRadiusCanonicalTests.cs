using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class InputRadiusCanonicalTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public InputRadiusCanonicalTests(XyuiHeadlessFixture fx) => _fx = fx;

    public static IEnumerable<object[]> InputFamily() =>
    [
        [typeof(XYTextField)], [typeof(XYNumberField)], [typeof(XYTextArea)],
        [typeof(XYSearchField)], [typeof(XYPasswordField)], [typeof(XYDatePicker)],
        [typeof(XYTimePicker)], [typeof(XYColorPicker)], [typeof(XYComboBox)], [typeof(XYSelect)]
    ];

    [Theory]
    [MemberData(nameof(InputFamily))]
    public void Input_family_consumes_canonical_radius(Type controlType) => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var control = (Control)Activator.CreateInstance(controlType)!;
        var window = XyuiBatchTestHost.Show(control);
        Assert.Equal(XyuiSpatialTokens.RadiusInput, ((TemplatedControl)control).CornerRadius.TopLeft);
        window.Close();
    });
}
