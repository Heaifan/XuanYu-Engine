using System.IO;
using System.Linq;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D2：Token 合同测试——结构、聚合、加载与 100 行限制。
// 读取正式 Token 文件（Design/）验证，不重复维护第二套完整数值表；合同键清单见 UiTokenContractCatalog。

public sealed class UiTokenContractTests
{
    private static readonly string DesignDir = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Design");

    private static string Read(string file) =>
        File.ReadAllText(Path.Combine(DesignDir, file));

    private static string[] AllTokenFiles() =>
        Directory.GetFiles(DesignDir, "UiTokens*.axaml").OrderBy(f => f).ToArray();

    [Fact]
    public void Catalog_has_no_duplicate_keys()
    {
        var dup = UiTokenContractCatalog.Keys.GroupBy(k => k).Where(g => g.Count() > 1).Select(g => g.Key);
        Assert.Empty(dup);
    }

    [Fact]
    public void Token_keys_are_globally_unique_across_all_files()
    {
        var keys = AllTokenFiles().SelectMany(f => System.Text.RegularExpressions.Regex.Matches(
            File.ReadAllText(f), @"x:Key=""([^""]+)""").Cast<System.Text.RegularExpressions.Match>())
            .Select(m => m.Groups[1].Value).ToList();
        Assert.Equal(keys.Count, keys.Distinct().Count());
    }

    [Fact]
    public void Code_tokens_match_catalog_exactly()
    {
        var codeKeys = AllTokenFiles().SelectMany(f => System.Text.RegularExpressions.Regex.Matches(
            File.ReadAllText(f), @"x:Key=""([^""]+)""").Cast<System.Text.RegularExpressions.Match>())
            .Select(m => m.Groups[1].Value).OrderBy(k => k).ToArray();
        var catalogKeys = UiTokenContractCatalog.Keys.OrderBy(k => k).ToArray();
        Assert.Equal(catalogKeys, codeKeys);
    }

    [Fact]
    public void Aggregate_includes_all_seven_token_files()
    {
        var agg = Read("UiTokens.axaml");
        foreach (var name in new[]
        {
            "UiTokens.Fonts.axaml", "UiTokens.Colors.Core.axaml", "UiTokens.Colors.Components.axaml",
            "UiTokens.Spacing.axaml", "UiTokens.Controls.axaml", "UiTokens.Icons.axaml", "UiTokens.Motion.axaml",
        })
            Assert.Contains(name, agg);
    }

    [Fact]
    public void Aggregate_does_not_reference_itself()
    {
        Assert.DoesNotContain("UiTokens.axaml", Read("UiTokens.axaml"));
    }

    [Fact]
    public void All_token_files_respect_100_line_limit()
    {
        foreach (var f in AllTokenFiles())
        {
            var lines = File.ReadAllLines(f).Length;
            Assert.True(lines <= 100, $"{Path.GetFileName(f)} 为 {lines} 行，超过 100 行限制");
        }
    }

    [Fact]
    public void Application_resources_merge_token_aggregate()
    {
        var ui = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
            "..", "..", "..", "..", "XuanYu.Editor.UI", "Ui.axaml"));
        Assert.Contains("avares://XuanYu.Editor.UI/Design/UiTokens.axaml", ui);
    }
}
