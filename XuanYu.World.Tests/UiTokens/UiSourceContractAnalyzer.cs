using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D2-F1：UI 源码违规分析器（测试侧）。允许值从 UiTokenManifest.json 读取。

public readonly record struct UiViolation(string Path, string Locator, UiRuleKind Kind, string Property, string Value);

public static partial class UiSourceContractAnalyzer
{
    private static readonly string ManifestPath = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Design", "UiTokenManifest.json");

    public static IReadOnlyList<string> AllowedFontSizes { get; } = LoadAllowed("Font");
    public static IReadOnlyList<string> AllowedRadii { get; } = ["0", "3", "6", "10"];
    public static IReadOnlyList<string> AllowedHeights { get; } = LoadAllowed("Control.Height");
    public const string AllowedStroke = "1.5";

    private static readonly Regex SymbolRx = new( // Emoji/Dingbats/Misc Symbols/Arrows/Geometric；CJK 正文不在其中
        @"[\u2190-\u21FF\u2300-\u23FF\u25A0-\u25FF\u2600-\u26FF\u2700-\u27BF\u2B00-\u2BFF\uFE0F\uD83C-\uDBFF\uDC00-\uDFFF]",
        RegexOptions.Compiled);
    private static readonly Regex PathDataRx = new(@"<(?:Path|PathIcon)\b[^>]*?Data=""([^""]*)""", RegexOptions.Compiled);
    private static readonly Regex ContentRx = new(@"<(Button|ToggleButton)\b[^>]*?\bContent=""([^""]*)""", RegexOptions.Compiled);
    private static readonly Regex IconTextRx = new(
        @"<TextBlock\b[^>]*?(?:Classes=""[^""]*icon[^""]*""|x:Name=""[^""]*(?:Icon|icon)[^""]*"")[^>]*?Text=""([^""]*)""",
        RegexOptions.Compiled);
    private static readonly Regex IconContentRx = new(
        @"<TextBlock\b[^>]*?(?:Classes=""[^""]*icon[^""]*""|x:Name=""[^""]*(?:Icon|icon)[^""]*"")[^>]*>([^<>]*)</TextBlock>",
        RegexOptions.Compiled);
    private static readonly Regex TokenDeclRx = new(
        @"<(SolidColorBrush|x:Double|x:String|Thickness|CornerRadius|FontWeight|FontFamily)\s+x:Key=""[^""]+""",
        RegexOptions.Compiled);
    private static readonly Regex ClassRx = new(@"(?:class|record)\s+([A-Za-z_]\w*)", RegexOptions.Compiled); // 类型上下文
    private static readonly Regex MemberRx = new(
        @"^\s*(?:(?:async\s+)?(?:public|private|internal|protected)\s+[\w<>\[\],\s\?\.]+\s+[A-Za-z_]\w*\s*(?:=|\(|=>|\{)|(?:[\w<>\[\],\s]+)\s+[\w.]+\.[A-Za-z_]\w*\s*\(|(?:async\s+)?(?!if|for|while|foreach|switch|using|return|catch|lock|await|var|yield|const)[\w<>\[\],\s\?\.]+\s+[A-Za-z_]\w*\s*\(|^\s*const\s+[\w<>\[\],\s\?\.]+\s+[A-Za-z_]\w*\s*=)",
        RegexOptions.Compiled);
    private static readonly Regex ExplicitMemberRx = new(@"\.([A-Za-z_]\w*)\s*\(", RegexOptions.Compiled);
    private static readonly Regex PlainMemberRx = new(@"\b[A-Za-z_]\w*\s*(?:=|\(|=>|\{)", RegexOptions.Compiled);

    private static List<string> LoadAllowed(string prefix) =>
        ParseManifest().Where(t => t.Key.StartsWith(prefix)).Select(t => t.Value).Distinct().ToList();

    public static IReadOnlyList<ManifestEntry> ParseManifest()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(ManifestPath));
        var list = new List<ManifestEntry>();
        foreach (var el in doc.RootElement.GetProperty("Tokens").EnumerateArray())
            list.Add(new ManifestEntry(el.GetProperty("Key").GetString() ?? "",
                el.GetProperty("Type").GetString() ?? "", el.GetProperty("Value").GetString() ?? ""));
        return list;
    }

    public static bool IsSymbolText(string s) => SymbolRx.IsMatch(s);
    public static MatchCollection PathDataMatches(string text) => PathDataRx.Matches(text);
    public static MatchCollection ContentMatches(string text) => ContentRx.Matches(text);
    public static MatchCollection IconTextMatches(string text) => IconTextRx.Matches(text);
    public static MatchCollection TokenDeclMatches(string text) => TokenDeclRx.Matches(text);

    // 剥离注释，避免注释内容漂移造成基线变化（反例 9）。
    public static string StripAxamlComments(string text) =>
        Regex.Replace(text, "<!--[\\s\\S]*?-->", "");
    public static string StripCsComments(string text) =>
        Regex.Replace(Regex.Replace(text, "/\\*[\\s\\S]*?\\*/", ""), "//[^\\r\\n]*", "");
}

public sealed record ManifestEntry(string Key, string Type, string Value);
