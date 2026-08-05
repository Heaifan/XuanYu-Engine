using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D2：UI 源码违规分析器（测试侧，不进入运行时产品代码）。
// 输入为内存字符串或测试读取的真实文件文本；输出潜在违规，由 UiDebtBaselineTests 按基线过滤。
// 规则只针对 Avalonia UI 控件语义：Path 图标尺寸、布局容器高度、CornerRadius 0（无圆角）不误报。

public readonly record struct UiViolation(string Path, UiRuleKind Kind, string Property, string Value);

public static class UiSourceContractAnalyzer
{
    private static readonly string[] AllowedFontSizes = ["10", "11", "12", "13", "14", "16", "20", "24"];
    private static readonly string[] AllowedRadii = ["0", "3", "6", "10"];
    private static readonly string[] AllowedHeights = ["24", "28", "32"];
    private const string AllowedStroke = "1.5";

    private static readonly Regex HexRx = new(@"#[0-9A-Fa-f]{6,8}\b", RegexOptions.Compiled);
    private static readonly Regex FontSizeRx = new(
        @"FontSize=""([\d.]+)""|<Setter Property=""FontSize"" Value=""([\d.]+)""", RegexOptions.Compiled);
    private static readonly Regex RadiusRx = new(
        @"CornerRadius=""([\d.]+)""|<Setter Property=""CornerRadius"" Value=""([\d.]+)""", RegexOptions.Compiled);
    private static readonly Regex ShadowRx = new(
        @"BoxShadow=""([^""]+)""|<Setter Property=""BoxShadow"" Value=""([^""]+)""", RegexOptions.Compiled);
    private static readonly Regex StrokeRx = new(
        @"StrokeThickness=""([\d.]+)""|<Setter Property=""StrokeThickness"" Value=""([\d.]+)""", RegexOptions.Compiled);
    private static readonly Regex CtrlHeightRx = new(
        @"<(Button|ToggleButton|TextBox|TabItem|MenuItem|CheckBox|ComboBox|RadioButton|ListBoxItem)\b[^>]*?\b(?:Height|MinHeight)=""([\d.]+)""", RegexOptions.Compiled);
    private static readonly Regex StyleBlockRx = new(
        @"<Style Selector=""([^""]+)""\s*>([\s\S]*?)</Style>", RegexOptions.Compiled);
    private static readonly Regex SetterHeightRx = new(
        @"<Setter Property=""(?:Height|MinHeight)"" Value=""([\d.]+)""", RegexOptions.Compiled);
    private static readonly Regex SkipSelectorRx = new(
        @"Path|Icon|Image|Grid|Border|StackPanel|DockPanel|UniformGrid|ScrollViewer|ListBox$|TabControl|Window|RowDefinition|ColumnDefinition|Canvas|WrapPanel|ItemsControl|ContentControl|Panel",
        RegexOptions.Compiled);
    private static readonly Regex PathDataRx = new(@"<Path\b[^>]*?Data=""([^""]*)""", RegexOptions.Compiled);
    // SVG Path 数据允许字符：数字、逗号、空格、正负号、指令字母；出现其他字符（含 Emoji/Unicode 符号）即疑似非法图标。
    private static readonly Regex PathDataInvalidRx = new(@"[^\d.,\sMmHhVvLlZzCcSsQqTtAaEe+-]", RegexOptions.Compiled);

    public static List<UiViolation> AnalyzeAxaml(string text, string relPath)
    {
        var result = new List<UiViolation>();
        foreach (Match m in HexRx.Matches(text))
            result.Add(new(relPath, UiRuleKind.HexColor, "Color", m.Value));
        foreach (Match m in FontSizeRx.Matches(text))
            AddIfNotAllowed(result, relPath, UiRuleKind.FontSize, "FontSize", m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value, AllowedFontSizes);
        foreach (Match m in RadiusRx.Matches(text))
            AddIfNotAllowed(result, relPath, UiRuleKind.CornerRadius, "CornerRadius", m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value, AllowedRadii);
        foreach (Match m in ShadowRx.Matches(text))
            result.Add(new(relPath, UiRuleKind.BoxShadow, "BoxShadow", m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value));
        foreach (Match m in StrokeRx.Matches(text))
        {
            var v = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
            if (v != AllowedStroke)
                result.Add(new(relPath, UiRuleKind.StrokeThickness, "StrokeThickness", v));
        }
        foreach (Match m in CtrlHeightRx.Matches(text))
            AddIfNotAllowed(result, relPath, UiRuleKind.ControlHeight, "Height", m.Groups[2].Value, AllowedHeights);
        foreach (Match m in StyleBlockRx.Matches(text))
        {
            if (SkipSelectorRx.IsMatch(m.Groups[1].Value))
                continue;
            foreach (Match s in SetterHeightRx.Matches(m.Groups[2].Value))
                AddIfNotAllowed(result, relPath, UiRuleKind.ControlHeight, "Setter", s.Groups[1].Value, AllowedHeights);
        }
        foreach (Match m in PathDataRx.Matches(text))
            if (PathDataInvalidRx.IsMatch(m.Groups[1].Value))
                result.Add(new(relPath, UiRuleKind.EmojiIcon, "PathData", m.Groups[1].Value));
        return result;
    }

    public static List<UiViolation> AnalyzeCs(string text, string relPath)
    {
        var result = new List<UiViolation>();
        foreach (Match m in HexRx.Matches(text))
            result.Add(new(relPath, UiRuleKind.CsHexColor, "Color", m.Value));
        return result;
    }

    private static void AddIfNotAllowed(List<UiViolation> result, string path, UiRuleKind kind,
        string prop, string value, string[] allowed)
    {
        if (!System.Array.Exists(allowed, a => a == value))
            result.Add(new(path, kind, prop, value));
    }
}
