using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;
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
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public void Optional_slots_add_only_their_runtime_track(bool icon, bool modified, bool closable)
    {
        var plain = Measure(false, false, false);
        var actual = Measure(icon, modified, closable) - plain;
        var expected = icon ? XyuiComponentTokens.TabIconTrackWidth : modified ? XyuiComponentTokens.TabModifiedTrackWidth : closable ? XyuiComponentTokens.TabCloseHitTargetSize : 0;
        Assert.Equal(expected, actual, 1);
    }
    [Fact]
    public void Combined_optional_slots_equal_the_sum_of_their_tracks()
    {
        var plain = Measure(false, false, false);
        var combined = Measure(true, true, true);
        Assert.Equal(XyuiComponentTokens.TabIconTrackWidth + XyuiComponentTokens.TabModifiedTrackWidth + XyuiComponentTokens.TabCloseHitTargetSize, combined - plain, 1);
    }
    [Fact]
    public void Same_label_closable_tabs_keep_width_after_selection_switch()
    {
        var widths = _fx.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var tabs = new XYTabs(new XYTab { Id = "one", Label = "同名", IsClosable = true },
                new XYTab { Id = "two", Label = "同名", IsClosable = true }) { Width = 420 };
            SetSizingMode(tabs, "Content");
            var window = XyuiBatchTestHost.Show(tabs);
            var before = (tabs.Items[0].Bounds.Width, tabs.Items[1].Bounds.Width);
            tabs.Select("two"); tabs.UpdateLayout();
            var after = (tabs.Items[0].Bounds.Width, tabs.Items[1].Bounds.Width);
            window.Close();
            return (before, after);
        });
        Assert.Equal(widths.before.Item1, widths.after.Item1, 1);
        Assert.Equal(widths.before.Item2, widths.after.Item2, 1);
    }
    double Measure(bool icon, bool modified, bool closable) => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var tab = new XYTab { Id = "tab", Label = "基准", Icon = icon ? XyuiVectorIcon.Info : null,
            IsModified = modified, IsClosable = closable };
        var tabs = new XYTabs(tab) { Width = 420 };
        SetSizingMode(tabs, "Content");
        var window = XyuiBatchTestHost.Show(tabs);
        var width = tab.Bounds.Width;
        window.Close();
        return width;
    });
    static void SetSizingMode(XYTabs tabs, string value)
    {
        var property = typeof(XYTabs).GetProperty("SizingMode");
        Assert.NotNull(property);
        Assert.Equal("XyuiTabSizingMode", property!.PropertyType.Name);
        property.SetValue(tabs, Enum.Parse(property.PropertyType, value));
    }
}
