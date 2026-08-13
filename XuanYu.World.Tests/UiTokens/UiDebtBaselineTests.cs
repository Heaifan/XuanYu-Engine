using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2-F1：细粒度基线门禁。
// 匹配 = Path + Locator + Kind + Property + Value 全部参与；稳定定位：Style Selector → x:Name → 元素类型；
// 递归扫描全部 AXAML 与全部 UI code-behind（不依赖固定文件清单）。
// 10 项绕过反例：换位置/换选择器/换 x:Name/换属性/注释漂移/基线增长。
public sealed class UiDebtBaselineTests
{
    private static readonly string RepoRoot = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..");
    private static readonly string UiDir = Path.Combine(RepoRoot, "XuanYu.Editor.UI");

    private static int ScanCount(string relPath, string text) =>
        UiSourceContractAnalyzer.AnalyzeAxaml(text, relPath).Count;

    [Fact]
    public void Every_detected_violation_is_within_known_debt_baseline()
    {
        var over = new List<string>();
        foreach (var f in Directory.GetFiles(UiDir, "*.axaml", SearchOption.AllDirectories)
            .Select(Path.GetFullPath)
            .Where(f => !f.Contains("\\Design\\") && !f.Contains("\\bin\\") && !f.Contains("\\obj\\")))
        {
            var rel = Path.GetRelativePath(RepoRoot, f).Replace('\\', '/');
            var violations = UiSourceContractAnalyzer.AnalyzeAxaml(File.ReadAllText(f), rel);
            AssertBaseline(violations, over);
        }
        foreach (var f in Directory.GetFiles(UiDir, "*.cs", SearchOption.AllDirectories)
            .Select(Path.GetFullPath)
            .Where(f => !f.Contains("\\bin\\") && !f.Contains("\\obj\\")))
        {
            var rel = Path.GetRelativePath(RepoRoot, f).Replace('\\', '/');
            AssertBaseline(UiSourceContractAnalyzer.AnalyzeCs(File.ReadAllText(f), rel), over);
        }
        Assert.True(over.Count == 0, "超基线项:\n" + string.Join("\n", over.Take(60)));
    }

    private static void AssertBaseline(IEnumerable<UiViolation> violations, List<string> over)
    {
        foreach (var g in violations.GroupBy(v => (v.Path, v.Locator, v.Kind, v.Property, v.Value)))
        {
            var allowed = UiDebtBaseline.AllowedCountFor(
                g.Key.Path, g.Key.Locator, g.Key.Kind, g.Key.Property, g.Key.Value);
            if (g.Count() > allowed)
                over.Add($"{g.Key.Path} {g.Key.Locator} {g.Key.Kind} {g.Key.Property} {g.Key.Value}: {g.Count()} > 允许 {allowed}");
        }
    }

    [Fact]
    public void Scan_scope_excludes_render_and_design()
    {
        var files = Directory.GetFiles(UiDir, "*.axaml", SearchOption.AllDirectories)
            .Select(Path.GetFullPath)
            .Where(f => !f.Contains("\\Design\\") && !f.Contains("\\bin\\") && !f.Contains("\\obj\\"))
            .Select(f => Path.GetRelativePath(RepoRoot, f).Replace('\\', '/'));
        Assert.DoesNotContain(files, f => f.Contains("XuanYu.Render") || f.Contains("/Design/"));
        Assert.Contains(files, f => f.EndsWith("Ui.axaml"));
        Assert.Equal(34, files.Count()); // 另含 MAP-DATA-A 的 Owned 删除确认窗口与 Marker 面板。
    }
}
