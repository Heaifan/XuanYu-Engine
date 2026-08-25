using Avalonia;
using Avalonia.Media;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Tests;

// Gallery Smoke：App 资源加载 / 窗口创建 / 标题（Headless 可验证的部分）
[Collection("XyuiHeadless")]
public class GallerySmokeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;

    public GallerySmokeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void App_Resources_Contain_Theme_Brushes() => _fx.Run(() =>
    {
        var app = Application.Current!;
        // Headless 会话不触发 OnFrameworkInitializationCompleted，手动复现 App 资源合并
        var dict = XyuiTheme.CreateLight();
        app.Resources.MergedDictionaries.Add(dict);
        foreach (var key in new[] { "XY.Brush.Surface.App", "XY.Brush.Text.Primary",
                                    "XY.Brush.Accent.Default", "XY.Brush.Semantic.Error.Text" })
        {
            Assert.True(dict.ContainsKey(key), $"主题字典缺少 {key}");
            var ok = app.Resources.TryGetResource(key, null, out var brush);
            Assert.True(ok, $"App 资源缺少 {key}");
            Assert.IsType<SolidColorBrush>(brush);
        }
    });

    [Fact]
    public void MainWindow_Creates_With_Expected_Title() => _fx.Run(() =>
    {
        var win = new MainWindow();
        Assert.Contains("XYUI-1 文档 Gallery", win.Title);
    });

    [Fact]
    public void PaletteCatalog_Covers_All_Token_Families() => _fx.Run(() =>
    {
        var sections = PaletteCatalog.BuildSections(dark: false);
        Assert.True(sections.Count >= 8, $"家族分区不足: {sections.Count}");
        Assert.Equal(84, sections.Sum(s => s.Items.Count));
    });
}
