using Avalonia.Controls;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2QuickStartNormalizationTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2QuickStartNormalizationTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Theory]
    [InlineData("XYUI-2-13", 2)]
    [InlineData("XYUI-2-14", 2)]
    [InlineData("XYUI-2-15", 2)]
    [InlineData("XYUI-2-16", 2)]
    [InlineData("XYUI-2-17", 2)]
    [InlineData("XYUI-2-18", 2)]
    public void Quick_start_has_at_most_two_core_previews(string id, int expectedCount) => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var document = XYUI2DocumentationCatalog.Build().Single(x => x.Id == id);
        var preview = document.PreviewFactory();
        var window = XyuiBatchTestHost.Show(preview);
        var controls = preview.GetVisualDescendants().Count(x => x is XYSelect or XYTextArea or XYSearchField or XYPasswordField or XYDatePicker or XYTimePicker);
        Assert.Equal(expectedCount, controls);
        foreach (var text in new[] { "Disabled", "ReadOnly", "Error", "Boundary", "Keyboard Matrix", "Lifecycle" })
            Assert.DoesNotContain(text, document.QuickStartXaml, StringComparison.OrdinalIgnoreCase);
        window.Close();
    });
}
