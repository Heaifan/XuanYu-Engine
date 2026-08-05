using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2-F2：AXAML 规则入口。Setter → "Style:<Selector>"；内联元素定位见 Structure.cs；颜色记录真实属性名。
public static partial class UiSourceContractAnalyzer
{
    private static readonly Regex StyleBlockRx = new(@"<Style\s+Selector=""([^""]+)""\s*>([\s\S]*?)</Style>", RegexOptions.Compiled);
    private static readonly Regex SetterValRx = new(
        @"<Setter\s+Property=""([^""]+)""\s+Value=""([^""]+)""", RegexOptions.Compiled);
    private static readonly Regex AttrValRx = new(
        @"(?<prop>FontSize|CornerRadius|BoxShadow|StrokeThickness)=""(?<val>[^""]+)""", RegexOptions.Compiled);
    private static readonly Regex CtrlHeightRx = new(
        @"<(Button|ToggleButton|TextBox|TabItem|MenuItem|CheckBox|ComboBox|RadioButton|ListBoxItem)\b[^>]*?\b(?:Height|MinHeight)=""([\d.]+)""",
        RegexOptions.Compiled);
    private static readonly Regex SkipSelectorRx = new(@"Path|Icon|Image|Grid|Border|StackPanel|DockPanel|UniformGrid|ScrollViewer|ListBox$|TabControl|Window|RowDefinition|ColumnDefinition|Canvas|WrapPanel|ItemsControl|ContentControl|Panel", RegexOptions.Compiled);
    private static readonly Regex HexRx = new(@"#[0-9A-Fa-f]{6,8}\b", RegexOptions.Compiled);

    public static List<UiViolation> AnalyzeAxaml(string text, string relPath)
    {
        var result = new List<UiViolation>();
        text = StripAxamlComments(text);
        var index = BuildIndex(text);
        var spans = new List<(int Start, int End)>();
        foreach (Match s in StyleBlockRx.Matches(text))
            spans.Add((s.Index, s.Index + s.Length));
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
                foreach (Match h in HexRx.Matches(val))
                    result.Add(new(relPath, $"Style:{selector}", UiRuleKind.HexColor, prop, h.Value));
            }
        }
        foreach (Match m in AttrValRx.Matches(text))
        {
            if (spans.Any(sp => m.Index > sp.Start && m.Index < sp.End))
                continue;
            var el = FindEl(index, m.Index);
            AddPropViolations(result, relPath, el != null ? LocatorOf(el) : "Elm:Unknown", m.Groups["prop"].Value, m.Groups["val"].Value);
        }
        foreach (Match m in CtrlHeightRx.Matches(text))
        {
            if (spans.Any(sp => m.Index > sp.Start && m.Index < sp.End))
                continue;
            var v = m.Groups[2].Value;
            if (!AllowedHeights.Contains(v))
            {
                var el = FindEl(index, m.Index);
                result.Add(new(relPath, el != null ? LocatorOf(el) : "Elm:Unknown", UiRuleKind.ControlHeight, "Height", v));
            }
        }
        foreach (Match m in HexRx.Matches(text))
        {
            if (spans.Any(sp => m.Index > sp.Start && m.Index < sp.End))
                continue;
            var el = FindEl(index, m.Index);
            var loc = el != null ? LocatorOf(el) : "Elm:Unknown";
            var prop = el != null ? AttributeName(text, m.Index) : "Color";
            result.Add(new(relPath, loc, UiRuleKind.HexColor, prop, m.Value));
        }
        CheckIconRules(result, text, relPath, index);
        CheckTokenDeclRule(result, text, relPath, index);
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
