using System.Linq;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2-F2：code-behind 八类颜色写法正反例。
// 每种写法至少一个 FAIL 样例；无颜色构造的普通代码 PASS。
public sealed class UiCsColorRulesTests
{
    [Theory]
    [InlineData("var c = Colors.Red;")]
    [InlineData("var c = Color.FromRgb(17, 34, 51);")]
    [InlineData("var c = Color.FromArgb(255, 17, 34, 51);")]
    [InlineData("var c = Color.Parse(\"#112233\");")]
    [InlineData("var b = new SolidColorBrush(Colors.Red);")]
    [InlineData("var s = \"#112233\";")]
    [InlineData("var s = \"#FF112233\";")]
    public void Every_cs_color_construction_is_reported(string line)
    {
        const string file = "namespace N;\nclass Helper\n{\n    private void Paint()\n    {\n        $LINE\n    }\n}";
        var v = UiSourceContractAnalyzer.AnalyzeCs(file.Replace("$LINE", line), "Helper.cs");
        Assert.Contains(v, x => x.Kind == UiRuleKind.CsHexColor && x.Locator == "Helper.Paint");
    }

    [Theory]
    [InlineData("const uint Accent = 0xFF112233;")]
    [InlineData("const uint Dark = 0x112233;")]
    public void Uint_constants_are_reported_with_field_locator(string line)
    {
        const string file = "namespace N;\nclass Helper\n{\n    $LINE\n    private void Paint() { }\n}";
        var v = UiSourceContractAnalyzer.AnalyzeCs(file.Replace("$LINE", line), "Helper.cs");
        Assert.Contains(v, x => x.Kind == UiRuleKind.CsHexColor
            && (x.Locator == "Helper.Accent" || x.Locator == "Helper.Dark"));
    }

    [Fact]
    public void Cs_code_without_color_construction_passes()
    {
        const string text = "namespace N;\nclass Helper\n{\n    private void Paint()\n    {\n        var s = \"hello\";\n        var n = 42;\n    }\n}";
        Assert.Empty(UiSourceContractAnalyzer.AnalyzeCs(text, "Helper.cs"));
    }

    [Fact]
    public void Cs_comments_do_not_trigger()
    {
        const string text = "namespace N;\nclass Helper\n{\n    // Colors.Red 与 #112233 注释不应触发\n    private void Paint() { }\n}";
        Assert.Empty(UiSourceContractAnalyzer.AnalyzeCs(text, "Helper.cs"));
    }

    [Fact]
    public void New_ui_cs_file_with_any_color_writing_is_detected()
    {
        // 模拟此前不存在的新 UI .cs——无论哪种写法递归扫描必然报告。
        Assert.NotEmpty(UiSourceContractAnalyzer.AnalyzeCs(
            "namespace XuanYu.Editor.UI; sealed class NewHelper { static uint C = 0xABCDEF; }", "NewHelper.cs"));
        Assert.NotEmpty(UiSourceContractAnalyzer.AnalyzeCs(
            "namespace XuanYu.Editor.UI; sealed class NewHelper { static string C = Colors.AliceBlue.ToString(); }", "NewHelper.cs"));
    }
}
