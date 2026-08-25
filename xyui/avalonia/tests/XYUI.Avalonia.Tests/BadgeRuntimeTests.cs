using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Theme;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class BadgeRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public BadgeRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Badge_is_content_sized_with_one_22_dip_tag_geometry() => _fx.Run(() =>
    {
        Prepare(); var shortBadge = new XYBadge { Text = "草稿" }; var longBadge = new XYBadge { Text = "轻量级标签" };
        var panel = new StackPanel { Width = 500, Children = { shortBadge, longBadge } };
        var window = new Window { Content = panel }; window.Show();
        Assert.Equal(HorizontalAlignment.Left, shortBadge.HorizontalAlignment);
        Assert.Equal(XYBadge.BadgeHeight, shortBadge.Height); Assert.True(double.IsNaN(shortBadge.Width));
        Assert.True(shortBadge.Bounds.Width < panel.Bounds.Width, $"badge={shortBadge.Bounds.Width}, host={panel.Bounds.Width}");
        Assert.True(longBadge.Bounds.Width > shortBadge.Bounds.Width, $"long={longBadge.Bounds.Width}, short={shortBadge.Bounds.Width}");
        var shape = Assert.Single(shortBadge.GetVisualDescendants().OfType<VectorPath>());
        Assert.Contains("xyui-badge-background-shape", shape.Classes);
        var geometry = Assert.IsAssignableFrom<Geometry>(shape.DefiningGeometry);
        Assert.IsType<StreamGeometry>(geometry);
        Assert.Equal(XYBadge.BadgeHeight, geometry.Bounds.Height);
        Assert.Equal(XYBadge.PointerTipInset, geometry.Bounds.X); Assert.Equal(11, XYBadge.PointerWidth);
        Assert.Equal(shortBadge.Bounds.Width - XYBadge.PointerTipInset, geometry.Bounds.Width);
        Assert.Equal(2, XYBadge.PointerTipInset);
        Assert.Equal(XYBadge.PointerWidth + 8, shortBadge.GetVisualDescendants().OfType<TextBlock>().Single().Margin.Left);
        window.Close();
    });

    [Fact]
    public void Gallery_uses_real_default_and_accent_badges() => _fx.Run(() =>
    {
        var preview = Assert.IsType<StackPanel>(XYUI1GalleryCatalog.CreatePreview("XYUI-1-09"));
        var badges = preview.Children.OfType<XYBadge>().ToArray();
        Assert.Equal(3, badges.Length); Assert.All(badges, x => Assert.Equal(HorizontalAlignment.Left, x.HorizontalAlignment));
        Assert.Contains(badges, x => x.Variant == XyuiBadgeVariant.Default);
        Assert.Contains(badges, x => x.Variant == XyuiBadgeVariant.Accent);
    });

    [Fact]
    public void Default_and_accent_tag_resources_follow_light_and_dark() => _fx.Run(() =>
    {
        var app = Prepare(); app.RequestedThemeVariant = ThemeVariant.Light;
        var preview = Assert.IsType<StackPanel>(XYUI1GalleryCatalog.CreatePreview("XYUI-1-09"));
        var window = new Window { Content = preview }; window.Show();
        var badges = preview.Children.OfType<XYBadge>().Take(2).ToArray();
        var defaultPath = PathOf(badges[0]); var accentPath = PathOf(badges[1]);
        Assert.Equal(Token("XY.Surface.PanelAlt", false), ColorOf(defaultPath.Fill));
        Assert.Equal(Token("XY.Tag.Accent", false), ColorOf(accentPath.Fill));
        app.RequestedThemeVariant = ThemeVariant.Dark;
        Assert.Equal(Token("XY.Surface.PanelAlt", true), ColorOf(defaultPath.Fill));
        Assert.Equal(Token("XY.Tag.Accent", true), ColorOf(accentPath.Fill)); window.Close();
    });

    static Application Prepare() { var app = Application.Current!; app.Resources.MergedDictionaries.Add(XyuiTheme.CreateThemeDictionaries()); app.Styles.Add(XyuiComponentStyles.Create()); return app; }
    static VectorPath PathOf(XYBadge badge) => badge.GetVisualDescendants().OfType<VectorPath>().Single();
    static Color Token(string id, bool dark) => XyuiColorTokens.All.Single(x => x.TokenId == id).ToColor(dark);
    static Color ColorOf(IBrush? brush) => Assert.IsType<SolidColorBrush>(brush).Color;
}
