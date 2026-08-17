using Avalonia.Media;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Tests;

// Brush Runtime：主题字典的 key 完整性 / 类型 / 值 / 重复
[Collection("XyuiHeadless")]
public class BrushRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;

    public BrushRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Light_Theme_Has_All_Brushes_No_Duplicates() => _fx.Run(() =>
    {
        var dict = XyuiTheme.CreateLight();
        var keys = dict.Keys.Cast<string>().ToArray();
        Assert.Equal(XyuiColorTokens.All.Count, keys.Length);
        Assert.Equal(keys.Length, keys.Distinct().Count());
    });

    [Fact]
    public void Every_Brush_Resolves_To_Canonical_Color() => _fx.Run(() =>
    {
        var dict = XyuiTheme.CreateLight();
        foreach (var t in XyuiColorTokens.All)
        {
            var key = XyuiColorTokens.BrushKey(t.TokenId);
            Assert.True(dict.ContainsKey(key), $"缺少资源: {key}");
            var brush = Assert.IsType<SolidColorBrush>(dict[key]);
            Assert.Equal(t.ToColor(false), brush.Color);
        }
    });

    [Fact]
    public void Dark_Theme_Matches_Canonical_Dark_Values() => _fx.Run(() =>
    {
        var dict = XyuiTheme.CreateDark();
        foreach (var t in XyuiColorTokens.All)
        {
            var brush = Assert.IsType<SolidColorBrush>(dict[XyuiColorTokens.BrushKey(t.TokenId)]);
            Assert.Equal(t.ToColor(true), brush.Color);
        }
    });

    [Fact]
    public void Unknown_Key_Is_Absent() => _fx.Run(() =>
    {
        var dict = XyuiTheme.CreateLight();
        Assert.False(dict.ContainsKey("XY.Brush.Not.Exist"));
    });
}
