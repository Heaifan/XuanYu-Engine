using System.IO;
using System.Linq;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4-F1（纠偏 v2）：样式文件可读性（Setter 正常分行、≤100 行、无压缩行）；
// 公共样式全部引用正式 Token；Manifest 保持 112 Frozen / 0 Pending。
public sealed class UiD4F1TypographyContractTests
{
    static readonly string RepoRoot = TestSourceRoot.Root;

    static readonly string[] Scope =
    [
        "Infrastructure/Resources/Theme/Ui.axaml",
        "Infrastructure/Resources/Theme/UiStyles.D4F1.axaml",
        "Panels/Inspector/InspectorPanels/InspectorPanel.axaml",
        "Panels/Inspector/RightPanel/EditorRightTabs.axaml",
        "MapUI/Presentation/MapToolPanels/MapPagePanel.axaml",
        "MapUI/Presentation/EditorPanels/MapEditorPanel.axaml",
        "MapUI/Editing/LayerPanels/LayerPanel.axaml",
        "Panels/Inspector/InspectorPanels/LayerInspectorPanel.axaml",
    ];

    static string Read(string rel) => File.ReadAllText(Path.Combine(RepoRoot, "XuanYu.Editor.UI", rel));

    [Fact]
    public void Public_semantic_styles_use_formal_tokens()
    {
        var ui = Read("Infrastructure/Resources/Theme/UiStyles.D4F1.axaml");
        Assert.Contains("Font.Label.Size", ui);   // uiLabel / uiTextButton 12
        Assert.Contains("Font.Body.Size", ui);    // uiValue 13
        Assert.Contains("Font.Section.Size", ui); // uiSection 14
    }

    [Fact]
    public void Style_files_are_readable_and_within_100_lines()
    {
        foreach (var rel in new[] { "Infrastructure/Resources/Theme/Ui.axaml", "Infrastructure/Resources/Theme/UiStyles.D4F1.axaml" })
        {
            var lines = File.ReadAllLines(Path.Combine(RepoRoot, "XuanYu.Editor.UI", rel));
            Assert.True(lines.Length <= 100, $"{rel} 超过 100 行（{lines.Length}）");
            foreach (var line in lines)
            {
                // 禁止压缩 Style 行：同一行同时出现 <Style 与 </Style>（多 Setter 挤在一行）
                Assert.False(line.Contains("<Style") && line.Contains("</Style>"),
                    $"{rel} 存在压缩单行 Style：{line.Trim()[..System.Math.Min(60, line.Trim().Length)]}");
            }
        }
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
        Assert.Equal(1, CountOccurrences(Read("Infrastructure/Resources/Theme/Ui.axaml"), "FontFamily\" Value=\"Microsoft YaHei UI"));
        Assert.Equal(0, CountOccurrences(Read("Panels/Inspector/InspectorPanels/InspectorPanel.axaml"), "FontFamily"));
        Assert.Equal(0, CountOccurrences(Read("MapUI/Presentation/MapToolPanels/MapPagePanel.axaml"), "FontFamily"));
    }

    static int CountOccurrences(string text, string needle) =>
        System.Text.RegularExpressions.Regex.Matches(text, System.Text.RegularExpressions.Regex.Escape(needle)).Count;

    [Fact]
    public void Page_level_fonts_reference_only_public_styles_or_tokens()
    {
        var mapEditor = Read("MapUI/Presentation/EditorPanels/MapEditorPanel.axaml");
        Assert.Contains("<xy:XYToggleButton", mapEditor);
        Assert.Contains("Content=\"环境\"", mapEditor);
        var layerPanel = Read("MapUI/Editing/LayerPanels/LayerPanel.axaml");
        Assert.Contains("<xy:XYTruncatedText", layerPanel); // 图层名 13（XYUI-1-04）
        Assert.Contains("<xy:XYBadge", layerPanel);         // 类型标签（XYUI-1-09）
    }

    [Fact]
    public void Manifest_stays_frozen()
    {
        var manifest = System.Text.Json.JsonSerializer.Deserialize<UiTokenManifestSnapshot>(
            File.ReadAllText(Path.Combine(RepoRoot, "XuanYu.Editor.UI", "Infrastructure", "Resources", "Theme", "UiTokenManifest.json")));
        Assert.NotNull(manifest);
        Assert.Equal(112, manifest.Tokens!.Length);
        Assert.All(manifest.Tokens, t => Assert.Equal("Frozen", t.SpecStatus));
    }

    sealed class UiTokenManifestSnapshot { public UiTokenSnapshot[]? Tokens { get; set; } }

    sealed class UiTokenSnapshot { public string? SpecStatus { get; set; } }
}
