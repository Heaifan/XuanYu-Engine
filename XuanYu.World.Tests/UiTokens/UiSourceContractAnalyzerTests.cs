using System.Linq;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2：门禁自验证——分析器正反例（对内存字符串运行，不触碰真实页面）。
// 证明：分析器能发现违规、允许清单不误放普通 UI、基线不掩盖新增违规、测试夹具不污染仓库扫描。
public sealed class UiSourceContractAnalyzerTests
{
    [Fact]
    public void Legal_token_reference_text_passes()
    {
        const string text = """
            <StackPanel Spacing="8">
              <Button FontSize="12" Height="28" CornerRadius="6" Background="{StaticResource Color.Bg.Control}"/>
              <TextBox MinHeight="24"/>
            </StackPanel>
            """;
        Assert.Empty(UiSourceContractAnalyzer.AnalyzeAxaml(text, "Xaml"));
    }

    [Fact]
    public void Unregistered_hex_color_is_reported()
    {
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(
            "<Border Background=\"#112233\"/>", "Xaml");
        Assert.Contains(v, x => x.Kind == UiRuleKind.HexColor && x.Value == "#112233");
    }

    [Fact]
    public void Unregistered_font_size_is_reported()
    {
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(
            "<TextBlock FontSize=\"15\"/>", "Xaml");
        Assert.Contains(v, x => x.Kind == UiRuleKind.FontSize && x.Value == "15");
    }

    [Fact]
    public void Unregistered_corner_radius_is_reported()
    {
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(
            "<Border CornerRadius=\"5\"/>", "Xaml");
        Assert.Contains(v, x => x.Kind == UiRuleKind.CornerRadius && x.Value == "5");
    }

    [Fact]
    public void Unregistered_control_height_is_reported()
    {
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(
            "<Button MinHeight=\"34\"/>", "Xaml");
        Assert.Contains(v, x => x.Kind == UiRuleKind.ControlHeight && x.Value == "34");
    }

    [Fact]
    public void Emoji_or_unicode_icon_is_reported()
    {
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(
            "<Path Data=\"\U0001F600\"/>", "Xaml");
        Assert.Contains(v, x => x.Kind == UiRuleKind.EmojiIcon);
    }

    [Fact]
    public void Box_shadow_is_reported()
    {
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(
            "<Border BoxShadow=\"0 14 30 0 #160f172a\"/>", "Xaml");
        Assert.Contains(v, x => x.Kind == UiRuleKind.BoxShadow);
    }

    [Fact]
    public void Non_standard_stroke_thickness_is_reported_but_1_5_passes()
    {
        Assert.Contains(UiSourceContractAnalyzer.AnalyzeAxaml(
            "<Path StrokeThickness=\"2.2\"/>", "Xaml"), x => x.Kind == UiRuleKind.StrokeThickness);
        Assert.Empty(UiSourceContractAnalyzer.AnalyzeAxaml(
            "<Path StrokeThickness=\"1.5\"/>", "Xaml"));
    }

    [Fact]
    public void Layout_and_icon_values_do_not_false_positive()
    {
        const string text = """
            <Grid MinHeight="320" Height="*">
              <Border MinHeight="0"/>
              <Path Height="16" Width="16" StrokeThickness="1.5"/>
              <Border CornerRadius="0"/>
              <StackPanel MinHeight="220"/>
              <ListBox MinHeight="220"/>
            </Grid>
            """;
        Assert.Empty(UiSourceContractAnalyzer.AnalyzeAxaml(text, "Xaml"));
    }

    [Fact]
    public void Cs_color_construction_is_reported()
    {
        var v = UiSourceContractAnalyzer.AnalyzeCs(
            "var b = Brush.Parse(\"#aabbcc\");", "X.cs");
        Assert.Contains(v, x => x.Kind == UiRuleKind.CsHexColor && x.Value == "#aabbcc");
    }
}
