using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI1TextRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI1TextRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Text_family_has_stable_identity_and_inherits_foundation_scopes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var host = new StackPanel();
        var controls = new Control[] { new XYText(), new XYLabel(), new XYCaption(), new XYHeading(), new XYSectionTitle(), new XYLink() };
        foreach (var control in controls) host.Children.Add(control);
        XY.SetSize(host, XYSize.Comfortable); XY.SetDensity(host, XYDensity.Compact);
        Assert.Equal(new[] { "XYUI-1-01", "XYUI-1-02", "XYUI-1-03", "XYUI-1-04", "XYUI-1-05", "XYUI-1-06" },
            controls.Select(x => x switch { XYLink l => l.CanonicalId, XYSectionTitle s => s.CanonicalId, XyuiTextComponent t => t.CanonicalId, _ => "" }));
        Assert.Equal(XYSize.Comfortable, XY.GetSize(controls[0]));
        Assert.Equal(XYDensity.Compact, XY.GetDensity(controls[0]));
    });

    [Fact]
    public void Heading_and_section_title_use_foundation_typography() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var heading = new XYHeading { Variant = XyuiHeadingVariant.PageTitle };
        var section = new XYSectionTitle { Text = "属性分组" };
        var window = XyuiBatchTestHost.Show(new StackPanel { Children = { heading, section } });
        Assert.Equal(XyuiTypographyTokens.FontSizePageTitle, heading.FontSize);
        Assert.Equal(14, section.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Classes.Contains("xyui-section-title-text")).FontSize);
        window.Close();
    });

    [Fact]
    public void Link_uses_shared_focus_and_disabled_state_contract() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var link = new XYLink { Content = "打开文档" };
        var window = XyuiBatchTestHost.Show(link);
        XyuiBatchTestHost.Hover(window, link);
        Assert.Contains("xyui-focusable", link.Classes);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Accent.Strong"), ColorOf(link.Foreground));
        link.IsEnabled = false; link.ApplyStyling();
        Assert.Equal(XyuiBatchTestHost.Token("XY.Text.Disabled"), ColorOf(link.Foreground));
        window.Close();
    });

    static Color ColorOf(IBrush? brush) => Assert.IsAssignableFrom<ISolidColorBrush>(brush).Color;
}
