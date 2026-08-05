using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D2-F1：AXAML 规则实现（入口 + 属性违规归类）。正则与 ElementLocator 见 Inline.cs。
// Locator：Style 块内 Setter → "Style:<Selector>"；元素内联 → "Name:<x:Name>" 或 "Elm:<类型>"。

public static partial class UiSourceContractAnalyzer
{
    public static List<UiViolation> AnalyzeAxaml(string text, string relPath)
    {
        var result = new List<UiViolation>();
        text = StripAxamlComments(text);
        var styleSpans = new List<(int Start, int End, string Selector)>();
        foreach (Match s in StyleBlockRx.Matches(text))
            styleSpans.Add((s.Index, s.Index + s.Length, s.Groups[1].Value));
        CheckStyleRules(result, text, relPath, styleSpans);
        CheckInlineRules(result, text, relPath, styleSpans);
        CheckIconRules(result, text, relPath);
        CheckTokenDeclRule(result, text, relPath);
        return result;
    }
    private static void AddPropViolations(List<UiViolation> result, string relPath, string loc, string prop, string val)
    {
        switch (prop)
        {
            case "FontSize":
                if (!AllowedFontSizes.Contains(val))
                    result.Add(new(relPath, loc, UiRuleKind.FontSize, prop, val));
                break;
            case "CornerRadius":
                if (!AllowedRadii.Contains(val))
                    result.Add(new(relPath, loc, UiRuleKind.CornerRadius, prop, val));
                break;
            case "BoxShadow":
                result.Add(new(relPath, loc, UiRuleKind.BoxShadow, prop, val));
                break;
            case "StrokeThickness":
                if (val != AllowedStroke)
                    result.Add(new(relPath, loc, UiRuleKind.StrokeThickness, prop, val));
                break;
            case "Height":
            case "MinHeight":
                if (!AllowedHeights.Contains(val))
                    result.Add(new(relPath, loc, UiRuleKind.ControlHeight, prop, val));
                break;
        }
    }
}
