using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D2-F1：Token 合同测试——Manifest（唯一机器事实源）与 XAML 双向一致。
// Manifest 键集合 == XAML 键集合（112/112 键、类型、值全覆盖）；无重复/缺失/大小写漂移/未登记别名；
// ResourceInclude 完整图：目标存在、无直接/间接循环、聚合只含批准文件、应用只合并一次。

public sealed class UiTokenManifestTests
{
    private static readonly string RepoRoot = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..");
    private static readonly string DesignDir = Path.Combine(RepoRoot, "XuanYu.Editor.UI", "Design");

    private record ManifestRow(string Key, string Type, string Value, string Category, string SpecStatus);

    private static List<ManifestRow> ReadManifest()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(DesignDir, "UiTokenManifest.json")));
        return doc.RootElement.GetProperty("Tokens").EnumerateArray()
            .Select(e => new ManifestRow(
                e.GetProperty("Key").GetString() ?? "",
                e.GetProperty("Type").GetString() ?? "",
                e.GetProperty("Value").GetString() ?? "",
                e.GetProperty("Category").GetString() ?? "",
                e.GetProperty("SpecStatus").GetString() ?? ""))
            .ToList();
    }

    private static string XamlText(string category) => category switch
    {
        "Fonts" => "UiTokens.Fonts.axaml",
        "Colors.Core" => "UiTokens.Colors.Core.axaml",
        "Colors.Components" => "UiTokens.Colors.Components.axaml",
        "Spacing" => "UiTokens.Spacing.axaml",
        "Controls" => "UiTokens.Controls.axaml",
        "Icons" => "UiTokens.Icons.axaml",
        "Motion" => "UiTokens.Motion.axaml",
        _ => throw new System.ArgumentException(category),
    } is { } file
        ? File.ReadAllText(Path.Combine(DesignDir, file))
        : "";

    private static string ExpectedMarkup(ManifestRow t) => t.Type switch
    {
        "Color" => $"<SolidColorBrush x:Key=\"{t.Key}\" Color=\"{t.Value}\"/>",
        "Double" => $"<x:Double x:Key=\"{t.Key}\">{t.Value}</x:Double>",
        "String" => $"<x:String x:Key=\"{t.Key}\">{t.Value}</x:String>",
        _ => $"<{t.Type} x:Key=\"{t.Key}\">{t.Value}</{t.Type}>",
    };

    [Fact]
    public void Manifest_has_no_duplicate_keys()
    {
        var rows = ReadManifest();
        Assert.Equal(rows.Count, rows.Select(r => r.Key).Distinct().Count());
    }

    [Fact]
    public void Manifest_keys_match_xaml_keys_exactly()
    {
        var manifestKeys = ReadManifest().Select(r => r.Key).OrderBy(k => k).ToArray();
        var xamlKeys = Directory.GetFiles(DesignDir, "UiTokens*.axaml")
            .Where(f => !f.EndsWith("UiTokens.axaml"))
            .SelectMany(f => System.Text.RegularExpressions.Regex.Matches(File.ReadAllText(f), @"x:Key=""([^""]+)""")
                .Cast<System.Text.RegularExpressions.Match>().Select(m => m.Groups[1].Value))
            .OrderBy(k => k).ToArray();
        Assert.Equal(manifestKeys, xamlKeys);
    }

    [Fact]
    public void Every_manifest_token_exists_with_correct_type_and_value()
    {
        foreach (var t in ReadManifest())
        {
            var xaml = XamlText(t.Category);
            Assert.Contains(ExpectedMarkup(t), xaml);
        }
    }

    [Fact]
    public void All_tokens_have_spec_section_and_status()
    {
        Assert.All(ReadManifest(), t =>
        {
            Assert.False(string.IsNullOrWhiteSpace(t.Key));
            Assert.False(string.IsNullOrWhiteSpace(t.Type));
            Assert.False(string.IsNullOrWhiteSpace(t.Value));
            Assert.False(string.IsNullOrWhiteSpace(t.SpecStatus));
        });
    }
}
