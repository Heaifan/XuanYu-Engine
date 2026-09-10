using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3TabSizingContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3TabSizingContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Content_mode_sizes_tabs_to_their_content()
    {
        _fx.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var tabs = new XYTabs(new XYTab { Id = "short", Label = "短" },
                new XYTab { Id = "long", Label = "非常长的页面名称" }) { Width = 420 };
            SetSizingMode(tabs, "Content");
            var window = XyuiBatchTestHost.Show(tabs);
            Assert.True(tabs.Items[1].Bounds.Width > tabs.Items[0].Bounds.Width + 12,
                $"short={tabs.Items[0].Bounds.Width}, long={tabs.Items[1].Bounds.Width}");
            window.Close();
        });
    }

    [Fact]
    public void Equal_mode_keeps_tab_widths_equal()
    {
        _fx.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var tabs = new XYTabs(new XYTab { Id = "short", Label = "短" },
                new XYTab { Id = "long", Label = "非常长的页面名称" }) { Width = 420 };
            SetSizingMode(tabs, "Equal");
            var window = XyuiBatchTestHost.Show(tabs);
            Assert.Equal(tabs.Items[0].Bounds.Width, tabs.Items[1].Bounds.Width, 1);
            window.Close();
        });
    }

    static void SetSizingMode(XYTabs tabs, string value)
    {
        var property = typeof(XYTabs).GetProperty("SizingMode");
        Assert.NotNull(property);
        Assert.Equal("XyuiTabSizingMode", property!.PropertyType.Name);
        property.SetValue(tabs, Enum.Parse(property.PropertyType, value));
    }
}
