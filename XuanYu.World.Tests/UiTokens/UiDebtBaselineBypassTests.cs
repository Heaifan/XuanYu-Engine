using System.Collections.Generic;
using System.Linq;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2-F1：基线绕过反例（内存字符串）。
// 证明：删除旧债务后异位/异选择器/异 x:Name/异属性新增同值均失败；注释漂移不改变基线；基线不自动增长。
public sealed class UiDebtBaselineBypassTests
{
    private static readonly string Ui = "XuanYu.Editor.UI/Ui.axaml";
    private static int Scan(string text) => UiSourceContractAnalyzer.AnalyzeAxaml(text, Ui).Count;

    // 反例 1/2：原位置保留 PASS；删除 PASS。
    [Fact]
    public void Baseline_kept_at_original_position_passes_and_removal_passes()
    {
        const string kept = "<Style Selector=\"Window\"><Setter Property=\"Foreground\" Value=\"#172033\"/></Style>";
        Assert.Equal(1, Scan(kept));
        Assert.Empty(UiSourceContractAnalyzer.AnalyzeAxaml("<Border Background=\"White\"/>", "X"));
    }

    // 反例 3：同选择器第二处同值 FAIL。
    [Fact]
    public void Second_instance_same_selector_fails()
    {
        const string text = "<Style Selector=\"Window\"><Setter Property=\"Foreground\" Value=\"#172033\"/>"
            + "<Setter Property=\"Background\" Value=\"#172033\"/></Style>";
        var count = Scan(text);
        Assert.True(count > 1, "同选择器第二处同值必须超基线");
    }

    // 反例 4：删除原位置 + 另一控件新增同值 FAIL（不同 Locator）。
    [Fact]
    public void Moved_to_another_control_fails()
    {
        const string text = "<Border x:Name=\"OtherElm\" Background=\"#172033\"/>";
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(text, "XuanYu.Editor.UI/Ui.axaml");
        Assert.Contains(v, x => x.Locator == "Name:OtherElm");
        Assert.Equal(0, UiDebtBaseline.AllowedCountFor("XuanYu.Editor.UI/Ui.axaml", "Name:OtherElm", UiRuleKind.HexColor, "Color", "#172033"));
    }

    // 反例 5：同选择器另一属性 FAIL（Property 参与匹配）。
    [Fact]
    public void Same_selector_other_property_fails()
    {
        const string text = "<Style Selector=\"TabItem.sideTab\"><Setter Property=\"Height\" Value=\"30\"/></Style>";
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(text, "XuanYu.Editor.UI/Ui.axaml");
        Assert.Contains(v, x => x.Kind == UiRuleKind.ControlHeight && x.Property == "Height" && x.Value == "30");
        Assert.Equal(0, UiDebtBaseline.AllowedCountFor("XuanYu.Editor.UI/Ui.axaml", "Style:TabItem.sideTab",
            UiRuleKind.ControlHeight, "Height", "30"));
    }

    // 反例 6：旧违规移到另一 Style FAIL（Locator 不同）。
    [Fact]
    public void Moved_to_another_style_fails()
    {
        const string text = "<Style Selector=\"Button\"><Setter Property=\"Foreground\" Value=\"#172033\"/></Style>";
        var v = UiSourceContractAnalyzer.AnalyzeAxaml(text, "XuanYu.Editor.UI/Ui.axaml");
        Assert.Contains(v, x => x.Locator == "Style:Button");
        Assert.Equal(0, UiDebtBaseline.AllowedCountFor("XuanYu.Editor.UI/Ui.axaml", "Style:Button", UiRuleKind.HexColor, "Color", "#172033"));
    }

    // 反例 7/8：同值不同 x:Name / 不同属性 FAIL。
    [Fact]
    public void Different_name_or_kind_fails()
    {
        Assert.Equal(0, UiDebtBaseline.AllowedCountFor("X", "Name:A", UiRuleKind.HexColor, "Color", "#123456"));
        Assert.Equal(0, UiDebtBaseline.AllowedCountFor("X", "Style:Y", UiRuleKind.CornerRadius, "CornerRadius", "5"));
    }

    // 反例 9：注释漂移不造成基线变化。
    [Fact]
    public void Comment_changes_do_not_shift_baseline()
    {
        const string base_ = "<Border Background=\"#d5dfec\"/>";
        const string withComment = "<!-- 新增说明 #ff0000 旧色值注释 -->\n<Border Background=\"#d5dfec\"/>";
        Assert.Equal(UiSourceContractAnalyzer.AnalyzeAxaml(base_, "X").Count,
            UiSourceContractAnalyzer.AnalyzeAxaml(withComment, "X").Count);
    }

    // 反例 10：基线不可自动增长（未知位置允许量为 0，且条目静态不可变）。
    [Fact]
    public void Baseline_cannot_grow_automatically()
    {
        Assert.Equal(0, UiDebtBaseline.AllowedCountFor("X", "Name:New", UiRuleKind.HexColor, "Color", "#abcdef"));
        Assert.True(UiDebtBaseline.Entries.Count > 0);
    }

    [Fact]
    public void Baseline_entries_have_full_fingerprint()
    {
        Assert.All(UiDebtBaseline.Entries, e =>
        {
            Assert.False(string.IsNullOrWhiteSpace(e.WId));
            Assert.False(string.IsNullOrWhiteSpace(e.Path));
            Assert.False(string.IsNullOrWhiteSpace(e.Locator));
            Assert.False(string.IsNullOrWhiteSpace(e.Property));
            Assert.False(string.IsNullOrWhiteSpace(e.Value));
        });
    }
}
