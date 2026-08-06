using System.IO;
using System.Linq;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4-F1：字体统一——Section14/Label12/Body13/Button12 全走公共 Token；
// D4-F1 范围页面无裸 FontSize、无局部 FontFamily、无语义字重散落。
public sealed class UiD4F1TypographyContractTests
{
    static readonly string RepoRoot = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");

    static readonly string[] Scope =
    [
        "Ui.axaml",
        "Right/InspectorPanel.axaml",
        "Right/Right.axaml",
        "Right/MapPagePanel.axaml",
        "Right/MapEditorPanel.axaml",
        "Right/LayerPanel.axaml",
        "Right/LayerInspectorPanel.axaml",
    ];

    static string Read(string rel) => File.ReadAllText(Path.Combine(RepoRoot, "XuanYu.Editor.UI", rel));

    [Fact]
    public void Public_semantic_styles_use_formal_tokens()
    {
        var ui = Read("Ui.axaml");
        Assert.Contains("Font.Label.Size", ui);   // uiLabel 12
        Assert.Contains("Font.Body.Size", ui);    // uiValue 13
        Assert.Contains("Font.Section.Size", ui); // uiSection 14
    }

    [Fact]
    public void D4F1_pages_have_no_bare_fontsize_and_no_local_fontfamily()
    {
        foreach (var rel in Scope)
        {
            var text = Read(rel);
            // 裸 FontSize = 字面量数值（排除 {StaticResource 引用）
            Assert.DoesNotMatch(new System.Text.RegularExpressions.Regex(
                "FontSize\" Value=\"(?!\\{)"), text);
            Assert.DoesNotContain("FontWeight\" Value=\"Bold\"", text); // 无语义字重
            Assert.DoesNotContain("FontWeight\" Value=\"Regular\"", text);
        }
        // 全局字体族只允许在 Ui.axaml 的 Window 样式声明一处（不属"局部 FontFamily"）
        Assert.Equal(1, CountOccurrences(Read("Ui.axaml"), "FontFamily\" Value=\"Microsoft YaHei UI"));
        Assert.Equal(0, CountOccurrences(Read("Right/InspectorPanel.axaml"), "FontFamily"));
        Assert.Equal(0, CountOccurrences(Read("Right/MapPagePanel.axaml"), "FontFamily"));
    }

    static int CountOccurrences(string text, string needle) =>
        System.Text.RegularExpressions.Regex.Matches(text, System.Text.RegularExpressions.Regex.Escape(needle)).Count;

    [Fact]
    public void Page_level_fonts_reference_only_public_styles_or_tokens()
    {
        var mapEditor = Read("Right/MapEditorPanel.axaml");
        Assert.Contains("Font.Section.Size", mapEditor);  // 二级页签 14
        var layerPanel = Read("Right/LayerPanel.axaml");
        Assert.Contains("Font.Body.Size", layerPanel);    // 图层名 13
        Assert.Contains("Font.Meta.Size", layerPanel);    // 类型标签 10~11
    }

    [Fact]
    public void Manifest_stays_frozen()
    {
        var manifest = System.Text.Json.JsonSerializer.Deserialize<UiTokenManifestSnapshot>(
            File.ReadAllText(Path.Combine(RepoRoot, "XuanYu.Editor.UI", "Design", "UiTokenManifest.json")));
        Assert.NotNull(manifest);
        Assert.Equal(112, manifest.Tokens!.Length);
        Assert.All(manifest.Tokens, t => Assert.Equal("Frozen", t.SpecStatus));
    }

    sealed class UiTokenManifestSnapshot
    {
        public UiTokenSnapshot[]? Tokens { get; set; }
    }

    sealed class UiTokenSnapshot
    {
        public string? SpecStatus { get; set; }
    }
}
