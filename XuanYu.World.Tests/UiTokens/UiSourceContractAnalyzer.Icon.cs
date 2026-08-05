using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2-F2：图标位置符号（Emoji/Unicode）与 Design 外 Token 声明规则。
public static partial class UiSourceContractAnalyzer
{
    private static void CheckIconRules(List<UiViolation> result, string text, string relPath,
        Dictionary<int, El> index)
    {
        foreach (Match m in PathDataMatches(text))
            if (IsSymbolText(m.Groups[1].Value))
                result.Add(new(relPath, ElLoc(index, m.Index), UiRuleKind.EmojiIcon, "PathData", m.Groups[1].Value));
        foreach (Match m in ContentMatches(text))
            if (IsSymbolText(m.Groups[2].Value))
                result.Add(new(relPath, ElLoc(index, m.Index), UiRuleKind.EmojiIcon, "Content", m.Groups[2].Value));
        foreach (Match m in IconTextMatches(text))
            if (IsSymbolText(m.Groups[1].Value))
                result.Add(new(relPath, ElLoc(index, m.Index), UiRuleKind.EmojiIcon, "Text", m.Groups[1].Value));
        foreach (Match m in IconContentRx.Matches(text))
            if (IsSymbolText(m.Groups[1].Value))
                result.Add(new(relPath, ElLoc(index, m.Index), UiRuleKind.EmojiIcon, "Content", m.Groups[1].Value));
    }

    private static void CheckTokenDeclRule(List<UiViolation> result, string text, string relPath,
        Dictionary<int, El> index)
    {
        if (!relPath.Contains("/Design/"))
            foreach (Match m in TokenDeclMatches(text))
                result.Add(new(relPath, ElLoc(index, m.Index), UiRuleKind.TokenDeclaration, "x:Key", m.Value));
    }

    private static string ElLoc(Dictionary<int, El> index, int pos)
    {
        var el = FindEl(index, pos);
        return el != null ? LocatorOf(el) : "Elm:Unknown";
    }
}
