using System.Linq;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2-F2：AXAML 属性/位置换位反例（父链定位 v3）。
// 证明：同 Style 属性换位（Foreground→Background/BorderBrush）、匿名同类型控件换位、
// 不同父级同类型换位均失败；空白/注释/无关属性变化不造成基线漂移。
public sealed class UiDebtBaselineBypassF2Tests
{
    private static readonly string Ui = "XuanYu.Editor.UI/Ui.axaml";
    private static int Allowed(string loc, UiRuleKind kind, string prop, string val) =>
        UiDebtBaseline.AllowedCountFor(Ui, loc, kind, prop, val);

    [Fact]
    public void Same_style_foreground_moved_to_background_fails()
    {
        // 基线：Style:Window Foreground #172033（1 处）；同值移到 Background → Property 不匹配 → 0
        Assert.Equal(1, Allowed("Style:Window", UiRuleKind.HexColor, "Foreground", "#172033"));
        Assert.Equal(0, Allowed("Style:Window", UiRuleKind.HexColor, "Background", "#172033"));
    }

    [Fact]
    public void Same_style_background_moved_to_borderbrush_fails()
    {
        // 基线：Style:TabItem.sideTab:selected Background #edf4ff；同值移到 BorderBrush → 0
        Assert.Equal(1, Allowed("Style:TabItem.sideTab:selected", UiRuleKind.HexColor, "Background", "#edf4ff"));
        Assert.Equal(0, Allowed("Style:TabItem.sideTab:selected", UiRuleKind.HexColor, "BorderBrush", "#edf4ff"));
    }

    [Fact]
    public void Anonymous_border_relocation_fails()
    {
        // 基线无 Path:ROOT/Grid/Border:1（原债务在 Style:Window）；匿名 Border 新增同值 → 0
        Assert.Equal(0, Allowed("Path:ROOT/Grid/Border:1", UiRuleKind.HexColor, "Background", "#172033"));
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(
            "<Grid><Border Background=\"#172033\"/></Grid>", Ui);
        Assert.Contains(v, x => x.Locator == "Path:ROOT/Grid/Border:1");
    }

    [Fact]
    public void Anonymous_textblock_relocation_fails()
    {
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(
            "<Grid><TextBlock Foreground=\"#172033\"/></Grid>", Ui);
        Assert.Contains(v, x => x.Locator == "Path:ROOT/Grid/TextBlock:1");
        Assert.Equal(0, Allowed("Path:ROOT/Grid/TextBlock:1", UiRuleKind.HexColor, "Foreground", "#172033"));
    }

    [Fact]
    public void Different_parents_same_element_type_fail()
    {
        const string text = "<Grid><StackPanel><Border Background=\"#172033\"/></StackPanel>"
            + "<DockPanel><Border Background=\"#172033\"/></DockPanel></Grid>";
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(text, Ui);
        Assert.Contains(v, x => x.Locator == "Path:ROOT/Grid/StackPanel/Border:1");
        Assert.Contains(v, x => x.Locator == "Path:ROOT/Grid/DockPanel/Border:1");
        Assert.Equal(0, Allowed("Path:ROOT/Grid/StackPanel/Border:1", UiRuleKind.HexColor, "Background", "#172033"));
        Assert.Equal(0, Allowed("Path:ROOT/Grid/DockPanel/Border:1", UiRuleKind.HexColor, "Background", "#172033"));
    }

    [Fact]
    public void Whitespace_and_comment_changes_pass()
    {
        const string base_ = "<Border Background=\"#d5dfec\"/>";
        const string changed = "<!-- 新增注释 #ff0000 -->\n<Border  Background = \"#d5dfec\" />";
        Assert.Equal(UiSourceContractAnalyzer.AnalyzeAxaml(base_, "X").Count,
            UiSourceContractAnalyzer.AnalyzeAxaml(changed, "X").Count);
    }

    [Fact]
    public void Unrelated_attribute_addition_passes()
    {
        const string base_ = "<Border Background=\"#d5dfec\"/>";
        const string added = "<Border Background=\"#d5dfec\" Margin=\"4\" CornerRadius=\"6\"/>";
        Assert.Equal(UiSourceContractAnalyzer.AnalyzeAxaml(base_, "X").Count,
            UiSourceContractAnalyzer.AnalyzeAxaml(added, "X").Count);
    }

    [Fact]
    public void Anonymous_ordinal_sequence_is_stable()
    {
        const string text = "<Grid><Border/><Border/><Border Background=\"#112233\"/></Grid>";
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(text, "X");
        Assert.Contains(v, x => x.Locator == "Path:ROOT/Grid/Border:3");
    }
}
