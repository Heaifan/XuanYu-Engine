using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2：旧 UI 债务基线门禁。
// 真实扫描 XuanYu.Editor.UI 全部非 Design/ .axaml 与 5 处 code-behind 视觉源；
// 原则：已有债务（基线内）允许、新增债务（基线外或超量）失败、债务减少允许、基线不自动增长。
public sealed class UiDebtBaselineTests
{
    private static readonly string RepoRoot = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..");
    private static readonly string[] CsVisualSources =
    [
        "XuanYu.Editor.UI/TreeGuide.cs",
        "XuanYu.Editor.UI/Vm/Logging/LogEntry.cs",
        "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs",
        "XuanYu.Editor.UI/Win/UiWin.Dialogs.cs",
        "XuanYu.Editor.UI/Win/UiWin.UnsavedDialog.cs",
    ];
    private static string[] ScanAxamlFiles() =>
        Directory.GetFiles(Path.Combine(RepoRoot, "XuanYu.Editor.UI"), "*.axaml", SearchOption.AllDirectories)
            .Where(f => !f.Contains("\\Design\\") && !f.Contains("\\bin\\") && !f.Contains("\\obj\\"))
            .OrderBy(f => f).ToArray();

    [Fact]
    public void Every_detected_violation_is_within_known_debt_baseline()
    {
        var over = new List<string>();
        foreach (var f in ScanAxamlFiles())
        {
            var rel = Path.GetRelativePath(RepoRoot, f).Replace('\\', '/');
            var violations = UiSourceContractAnalyzer.AnalyzeAxaml(File.ReadAllText(f), rel);
            foreach (var g in violations.GroupBy(v => (v.Path, v.Kind, v.Value)))
            {
                var allowed = UiDebtBaseline.AllowedCountFor(g.Key.Path, g.Key.Kind, g.Key.Value);
                if (g.Count() > allowed)
                    over.Add($"{g.Key.Path} {g.Key.Kind} {g.Key.Value}: {g.Count()} 处 > 基线允许 {allowed}");
            }
        }
        foreach (var rel in CsVisualSources)
        {
            var path = Path.Combine(RepoRoot, rel);
            if (!File.Exists(path))
                continue;
            var violations = UiSourceContractAnalyzer.AnalyzeCs(File.ReadAllText(path), rel);
            foreach (var g in violations.GroupBy(v => (v.Path, v.Kind, v.Value)))
            {
                var allowed = UiDebtBaseline.AllowedCountFor(g.Key.Path, g.Key.Kind, g.Key.Value);
                if (g.Count() > allowed)
                    over.Add($"{g.Key.Path} {g.Key.Kind} {g.Key.Value}: {g.Count()} 处 > 基线允许 {allowed}");
            }
        }
        Assert.Empty(over);
    }

    [Fact]
    public void Scan_scope_excludes_render_and_design_dirs()
    {
        var files = ScanAxamlFiles().Select(f => Path.GetRelativePath(RepoRoot, f).Replace('\\', '/'));
        Assert.DoesNotContain(files, f => f.Contains("XuanYu.Render"));
        Assert.DoesNotContain(files, f => f.Contains("/Design/"));
    }

    [Fact]
    public void Same_file_second_instance_of_baselined_value_is_rejected()
    {
        // 基线允许 Ui.axaml #172033 出现 1 次；同文件第二处同值必须被判定为超量新增违规。
        const string path = "XuanYu.Editor.UI/Ui.axaml";
        const string value = "#172033";
        var allowed = UiDebtBaseline.AllowedCountFor(path, UiRuleKind.HexColor, value);
        Assert.Equal(1, allowed);
        var text = "<Border Background=\"#172033\"/><Border Background=\"#172033\"/>";
        var count = UiSourceContractAnalyzer.AnalyzeAxaml(text, path).Count(v => v.Value == value);
        Assert.True(count > allowed, "同文件第二处同值必须超过基线允许量");
    }

    [Fact]
    public void Removed_debt_does_not_fail()
    {
        // 债务被整改删除后：扫描不再发现该值 → 不产生违规 → 不失败。
        var text = "<Border Background=\"{StaticResource Color.Text.Primary}\"/>";
        var violations = UiSourceContractAnalyzer.AnalyzeAxaml(text, "Xaml");
        Assert.DoesNotContain(violations, v => v.Value == "#243744");
    }

    [Fact]
    public void Baseline_entries_have_wid_path_kind_and_value()
    {
        Assert.NotEmpty(UiDebtBaseline.Entries);
        Assert.All(UiDebtBaseline.Entries, e =>
        {
            Assert.False(string.IsNullOrWhiteSpace(e.WId));
            Assert.False(string.IsNullOrWhiteSpace(e.Path));
            Assert.False(string.IsNullOrWhiteSpace(e.Value));
        });
    }
}
