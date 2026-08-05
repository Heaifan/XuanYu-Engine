using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
namespace XuanYu.World.Tests.UiTokens;
// ARCH-UI-SPEC-R1-D2-F1：Token 资源引用图检查——目标存在、无直接/间接循环、聚合只含批准文件、应用只合并一次。
public sealed class UiTokenManifestGraphTests
{
    private static readonly string RepoRoot = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..");
    private static readonly string DesignDir = Path.Combine(RepoRoot, "XuanYu.Editor.UI", "Design");

    [Fact]
    public void Resource_includes_target_existing_files_without_cycles()
    {
        var agg = File.ReadAllText(Path.Combine(DesignDir, "UiTokens.axaml"));
        var includes = System.Text.RegularExpressions.Regex.Matches(agg, @"Source=""avares://XuanYu.Editor.UI/Design/([^""]+)""")
            .Cast<System.Text.RegularExpressions.Match>().Select(m => m.Groups[1].Value).ToList();
        Assert.Equal(7, includes.Count);
        Assert.DoesNotContain("UiTokens.axaml", includes); // 无自引用/间接循环（子文件不得引用聚合）
        foreach (var inc in includes)
        {
            Assert.True(File.Exists(Path.Combine(DesignDir, inc)), $"{inc} 不存在");
            var child = File.ReadAllText(Path.Combine(DesignDir, inc));
            Assert.DoesNotContain("ResourceInclude", child); // 子文件不得含 ResourceInclude
        }
    }

    [Fact]
    public void Application_merges_token_aggregate_exactly_once()
    {
        var ui = File.ReadAllText(Path.Combine(RepoRoot, "XuanYu.Editor.UI", "Ui.axaml"));
        Assert.Equal(1, System.Text.RegularExpressions.Regex.Matches(
            ui, @"ResourceInclude Source=""avares://XuanYu.Editor.UI/Design/UiTokens.axaml""").Count);
    }

    [Fact]
    public void Token_files_respect_100_line_limit()
    {
        foreach (var f in Directory.GetFiles(DesignDir, "UiTokens*.axaml"))
            Assert.True(File.ReadAllLines(f).Length <= 100, $"{Path.GetFileName(f)} 超 100 行");
    }
}
