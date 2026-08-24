using System.Text.RegularExpressions;
using Avalonia.Controls;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Tests;

// 防回潮：src + gallery 全部源码中禁止出现未经 Canonical 登记的颜色常量
public class SecondTruthTests
{
    static readonly string AvaloniaRoot = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..",
        "avalonia");

    static IEnumerable<string> SourceFiles() =>
        Directory.EnumerateFiles(AvaloniaRoot, "*.*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".cs") || f.EndsWith(".axaml"))
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                        && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"));

    [Fact]
    public void No_Undeclared_Hex_Anywhere_In_Avalonia_Sources()
    {
        var canonicalHex = XyuiColorTokens.All
            .SelectMany(t => new[] { t.LightHex, t.DarkHex })
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var hexRx = new Regex("#[0-9A-Fa-f]{6}\\b");
        foreach (var file in SourceFiles())
        {
            foreach (Match m in hexRx.Matches(File.ReadAllText(file)))
            {
                Assert.True(canonicalHex.Contains(m.Value),
                    $"未登记颜色 {m.Value} 出现在 {Path.GetRelativePath(AvaloniaRoot, file)}");
            }
        }
    }

    [Fact]
    public void Every_Axaml_Brush_Reference_Resolves_In_Theme()
    {
        var themeKeys = XyuiTheme.CreateLight().Keys.Cast<string>().ToHashSet();
        var refRx = new Regex(@"XY\.Brush\.[A-Za-z0-9.]+");
        foreach (var file in SourceFiles().Where(f => f.EndsWith(".axaml")))
        {
            foreach (Match m in refRx.Matches(File.ReadAllText(file)))
            {
                Assert.True(themeKeys.Contains(m.Value),
                    $"AXAML 引用了主题不存在的资源 {m.Value} ({Path.GetRelativePath(AvaloniaRoot, file)})");
            }
        }
    }

    [Fact]
    public void No_Inline_Typography_Literals_In_Gallery()
    {
        // 消费侧禁止手写字号、字体族和字重字面量（定义文件除外）
        var litRx = new Regex(@"FontSize\s*=\s*""\d|FontFamily\s*=\s*""[^{]|FontWeight\s*=\s*""[^{]");
        foreach (var file in SourceFiles().Where(f => f.EndsWith(".axaml") && f.Contains("gallery")))
        {
            var m = litRx.Match(File.ReadAllText(file));
            Assert.False(m.Success,
                $"内联 Typography 字面量 {m.Value} 出现在 {Path.GetRelativePath(AvaloniaRoot, file)}");
        }
    }

    [Fact]
    public void No_Hardcoded_FontFamily_Literals_Outside_Foundation()
    {
        var literalRx = new Regex(@"new\s+FontFamily\s*\(\s*\"[^\"]+\"|FontFamily\s*=\s*\"[^\"]+\"");
        foreach (var file in SourceFiles())
        {
            var match = literalRx.Match(File.ReadAllText(file));
            Assert.False(match.Success, $"未登记字体字面量 {match.Value} 出现在 {Path.GetRelativePath(AvaloniaRoot, file)}");
        }
    }
}
