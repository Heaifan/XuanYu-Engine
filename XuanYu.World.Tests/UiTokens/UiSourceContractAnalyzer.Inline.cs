using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2-F1：AXAML 规则实现（Style 块 + 内联元素）。
// Locator：Style 块内 Setter → "Style:<Selector>"；元素内联 → "Name:<x:Name>" 或 "Elm:<类型>"。
public static partial class UiSourceContractAnalyzer
{
    private static readonly Regex StyleBlockRx = new(
        @"<Style\s+Selector=""([^""]+)""\s*>([\s\S]*?)</Style>", RegexOptions.Compiled);
    private static readonly Regex SetterValRx = new(
        @"<Setter\s+Property=""([^""]+)""\s+Value=""([^""]+)""", RegexOptions.Compiled);
    private static readonly Regex AttrValRx = new(
        @"(?<prop>FontSize|CornerRadius|BoxShadow|StrokeThickness)=""(?<val>[^""]+)""", RegexOptions.Compiled);
    private static readonly Regex CtrlHeightRx = new(
        @"<(Button|ToggleButton|TextBox|TabItem|MenuItem|CheckBox|ComboBox|RadioButton|ListBoxItem)\b[^>]*?\b(?:Height|MinHeight)=""([\d.]+)""",
        RegexOptions.Compiled);
    private static readonly Regex SkipSelectorRx = new(
        @"Path|Icon|Image|Grid|Border|StackPanel|DockPanel|UniformGrid|ScrollViewer|ListBox$|TabControl|Window|RowDefinition|ColumnDefinition|Canvas|WrapPanel|ItemsControl|ContentControl|Panel",
        RegexOptions.Compiled);
    private static readonly Regex OpenTagRx = new(@"<([A-Za-z]\w*)", RegexOptions.Compiled);
    private static readonly Regex NameAttrRx = new(@"x:Name=""([^""]+)""", RegexOptions.Compiled);
    private static readonly Regex HexRx = new(@"#[0-9A-Fa-f]{6,8}\b", RegexOptions.Compiled);

    private static string ElementLocator(string text, int index)
    {
        var start = text.LastIndexOf('<', index);
        if (start < 0)
            return "Elm:Unknown";
        var open = text[start..Math.Min(text.Length, start + 200)];
        var name = NameAttrRx.Match(open);
        if (name.Success)
            return $"Name:{name.Groups[1].Value}";
        var tag = OpenTagRx.Match(open);
        return tag.Success ? $"Elm:{tag.Groups[1].Value}" : "Elm:Unknown";
    }

    private static void CheckStyleRules(List<UiViolation> result, string text, string relPath,
        List<(int Start, int End, string Selector)> spans)
    {
        foreach (Match s in StyleBlockRx.Matches(text))
        {
            var selector = s.Groups[1].Value;
            var body = s.Groups[2].Value;
            foreach (Match set in SetterValRx.Matches(body))
            {
                var prop = set.Groups[1].Value;
                var val = set.Groups[2].Value;
                if ((prop == "Height" || prop == "MinHeight") && SkipSelectorRx.IsMatch(selector))
                    continue;
                AddPropViolations(result, relPath, $"Style:{selector}", prop, val);
            }
            foreach (Match h in HexRx.Matches(body))
                result.Add(new(relPath, $"Style:{selector}", UiRuleKind.HexColor, "Color", h.Value));
        }
    }

    private static void CheckInlineRules(List<UiViolation> result, string text, string relPath,
        List<(int Start, int End, string Selector)> spans)
    {
        foreach (Match m in AttrValRx.Matches(text))
        {
            if (spans.Any(sp => m.Index > sp.Start && m.Index < sp.End))
                continue;
            AddPropViolations(result, relPath, ElementLocator(text, m.Index), m.Groups["prop"].Value, m.Groups["val"].Value);
        }
        foreach (Match m in CtrlHeightRx.Matches(text))
        {
            if (spans.Any(sp => m.Index > sp.Start && m.Index < sp.End))
                continue;
            var v = m.Groups[2].Value;
            if (!AllowedHeights.Contains(v))
                result.Add(new(relPath, ElementLocator(text, m.Index), UiRuleKind.ControlHeight, "Height", v));
        }
        foreach (Match m in HexRx.Matches(text))
        {
            if (spans.Any(sp => m.Index > sp.Start && m.Index < sp.End))
                continue;
            result.Add(new(relPath, ElementLocator(text, m.Index), UiRuleKind.HexColor, "Color", m.Value));
        }
    }
}
