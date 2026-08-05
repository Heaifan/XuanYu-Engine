using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2-F2：code-behind 颜色构造规则（八类写法全覆盖）。
// 检测：hex 字符串、Colors.*、Color.FromRgb/FromArgb/Parse、new SolidColorBrush、0xAARRGGBB/0xRRGGBB 常量。
public static partial class UiSourceContractAnalyzer
{
    private static readonly Regex HexRx2 = new(@"#[0-9A-Fa-f]{6,8}\b", RegexOptions.Compiled);
    private static readonly Regex ColorsRx = new(@"\bColors\.\w+", RegexOptions.Compiled);
    private static readonly Regex FromRx = new(@"\bColor\.(?:FromRgb|FromArgb|Parse)\s*\(", RegexOptions.Compiled);
    private static readonly Regex BrushRx = new(@"\bnew\s+SolidColorBrush\s*\(", RegexOptions.Compiled);
    private static readonly Regex UintRx = new(@"\b0x[0-9A-Fa-f]{6,8}\b", RegexOptions.Compiled);

    public static List<UiViolation> AnalyzeCs(string text, string relPath)
    {
        var result = new List<UiViolation>();
        text = StripCsComments(text);
        string type = "", member = ""; // 成员上下文：类型名.成员名
        foreach (var line in text.Split('\n'))
        {
            var cm = ClassRx.Match(line);
            if (cm.Success)
            {
                type = cm.Groups[1].Value;
                member = "";
            }
            else if (MemberRx.IsMatch(line))
            {
                var em = ExplicitMemberRx.Match(line);
                if (em.Success)
                    member = em.Groups[1].Value;
                else
                    member = PlainMemberRx.Match(line).Value.TrimEnd('=', '(', '>', '{', ' ').Trim();
            }
            var loc = $"{type}.{(string.IsNullOrEmpty(member) ? "Unknown" : member)}";
            foreach (Match m in HexRx2.Matches(line))
                result.Add(new(relPath, loc, UiRuleKind.CsHexColor, "Hex", m.Value));
            foreach (Match m in ColorsRx.Matches(line))
                result.Add(new(relPath, loc, UiRuleKind.CsHexColor, "Colors", m.Value));
            foreach (Match m in FromRx.Matches(line))
                result.Add(new(relPath, loc, UiRuleKind.CsHexColor, "ColorAPI", m.Value));
            foreach (Match m in BrushRx.Matches(line))
                result.Add(new(relPath, loc, UiRuleKind.CsHexColor, "Brush", m.Value));
            foreach (Match m in UintRx.Matches(line))
                result.Add(new(relPath, loc, UiRuleKind.CsHexColor, "Uint", m.Value));
        }
        return result;
    }
}
