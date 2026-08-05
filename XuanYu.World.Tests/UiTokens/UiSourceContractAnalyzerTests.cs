using System.Linq;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2-F1：分析器正反例（内存字符串）。
public sealed class UiSourceContractAnalyzerTests
{
    [Fact]
    public void Legal_token_reference_passes()
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
    public void Unregistered_values_are_reported()
    {
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(
            "<Border Background=\"#112233\"/><TextBlock FontSize=\"15\"/><Border CornerRadius=\"5\"/><Button MinHeight=\"34\"/>", "X");
        Assert.Contains(v, x => x.Kind == UiRuleKind.HexColor);
        Assert.Contains(v, x => x.Kind == UiRuleKind.FontSize);
        Assert.Contains(v, x => x.Kind == UiRuleKind.CornerRadius);
        Assert.Contains(v, x => x.Kind == UiRuleKind.ControlHeight);
    }

    [Fact]
    public void Chinese_button_and_tooltip_text_pass()
    {
        const string text = "<Button Content=\"保存地图\"/><TextBlock Text=\"鼠标悬停查看说明\"/>";
        Assert.DoesNotContain(UiSourceContractAnalyzer.AnalyzeAxaml(text, "X"), x => x.Kind == UiRuleKind.EmojiIcon);
    }

    [Fact]
    public void Valid_stream_geometry_passes()
    {
        const string text = "<Path Data=\"M1 2C3 4,5 6,7 8L9 10Z\"/>";
        Assert.DoesNotContain(UiSourceContractAnalyzer.AnalyzeAxaml(text, "X"), x => x.Kind == UiRuleKind.EmojiIcon);
    }

    [Theory]
    [InlineData("<Button Content=\"\u2699\"/>")]
    [InlineData("<ToggleButton Content=\"\U0001F512\"/>")]
    [InlineData("<TextBlock Classes=\"icon\" Text=\"\U0001F441\"/>")]
    [InlineData("<TextBlock x:Name=\"IconText\">\u25B6</TextBlock>")]
    [InlineData("<Path Data=\"\U0001F600\"/>")]
    [InlineData("<PathIcon Data=\"\u2699\"/>")]
    public void Emoji_or_unicode_icon_at_icon_position_fails(string text)
    {
        Assert.Contains(UiSourceContractAnalyzer.AnalyzeAxaml(text, "X"), x => x.Kind == UiRuleKind.EmojiIcon);
    }

    [Fact]
    public void Token_declaration_outside_design_fails()
    {
        const string text = """
            <UserControl.Resources>
              <SolidColorBrush x:Key="MyPage.SpecialBlue" Color="Blue"/>
              <x:Double x:Key="MyPage.FontSize">15</x:Double>
            </UserControl.Resources>
            """;
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(text, "XuanYu.Editor.UI/MyPage.axaml");
        Assert.Equal(2, v.Count(x => x.Kind == UiRuleKind.TokenDeclaration));
    }

    [Fact]
    public void Icon_resource_dictionary_in_design_passes()
    {
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(
            "<StreamGeometry x:Key=\"SearchIcon\">M1 2Z</StreamGeometry>", "XuanYu.Editor.UI/Design/UiTokens.axaml");
        Assert.DoesNotContain(v, x => x.Kind == UiRuleKind.TokenDeclaration);
    }

    [Fact]
    public void Cs_color_construction_is_reported_with_type_member_locator()
    {
        const string text = "namespace N;\nclass Helper\n{\n    private void Paint()\n    {\n        var b = Brush.Parse(\"#aabbcc\");\n    }\n}";
        var v = UiSourceContractAnalyzer.AnalyzeCs(text, "Helper.cs");
        Assert.Contains(v, x => x.Kind == UiRuleKind.CsHexColor && x.Locator == "Helper.Paint");
    }

    [Fact]
    public void New_ui_cs_file_with_raw_color_is_detected()
    {
        // 模拟"此前不存在的新 UI .cs"加入原始颜色——递归扫描必然报告（不依赖固定清单）。
        const string newFile = "namespace XuanYu.Editor.UI; sealed class SixthVisualHelper { static string C = \"#123456\"; }";
        Assert.Contains(UiSourceContractAnalyzer.AnalyzeCs(newFile, "SixthVisualHelper.cs"), x => x.Value == "#123456");
    }

    [Fact]
    public void Comments_do_not_trigger_or_shift_violations()
    {
        const string text = "<!-- #abcdef 注释色值 -->\n<Border Background=\"White\"/>";
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(text, "X");
        Assert.DoesNotContain(v, x => x.Kind == UiRuleKind.HexColor);
    }
}
