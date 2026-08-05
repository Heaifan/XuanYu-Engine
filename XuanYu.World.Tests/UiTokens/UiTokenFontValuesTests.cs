using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D2：字体 Token 数值合同（UI Spec 1.0 §3.1/§3.2/§3.4）。
// 断言 Token 文件中的键存在且数值与正式规范一致；依据见 UiTokenContractCatalog。

public sealed class UiTokenFontValuesTests
{
    private static readonly string File_ = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Design", "UiTokens.Fonts.axaml");

    public static IEnumerable<object[]> Data()
    {
        yield return new object[] { "Font.Family", "Microsoft YaHei UI, Segoe UI, Noto Sans CJK SC" };
        yield return new object[] { "Font.Meta.Size", "10" };
        yield return new object[] { "Font.Meta.LineHeight", "14" };
        yield return new object[] { "Font.Small.Size", "11" };
        yield return new object[] { "Font.Small.LineHeight", "16" };
        yield return new object[] { "Font.Label.Size", "12" };
        yield return new object[] { "Font.Label.LineHeight", "18" };
        yield return new object[] { "Font.Body.Size", "13" };
        yield return new object[] { "Font.Body.LineHeight", "20" };
        yield return new object[] { "Font.Section.Size", "14" };
        yield return new object[] { "Font.Section.LineHeight", "22" };
        yield return new object[] { "Font.Title.Size", "16" };
        yield return new object[] { "Font.Title.LineHeight", "24" };
        yield return new object[] { "Font.Page.Size", "20" };
        yield return new object[] { "Font.Page.LineHeight", "28" };
        yield return new object[] { "Font.Display.Size", "24" };
        yield return new object[] { "Font.Display.LineHeight", "32" };
        yield return new object[] { "Font.Weight.Regular", "Regular" };
        yield return new object[] { "Font.Weight.Medium", "Medium" };
        yield return new object[] { "Font.Weight.SemiBold", "SemiBold" };
        yield return new object[] { "Font.Weight.Bold", "Bold" };
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Font_token_matches_spec(string key, string expected)
    {
        var lines = File.ReadAllLines(File_);
        var line = lines.Single(l => l.Contains($"x:Key=\"{key}\""));
        Assert.Contains($">{expected}<", line);
    }

    [Fact]
    public void Every_font_size_has_a_paired_line_height_key()
    {
        var text = File.ReadAllText(File_);
        foreach (var name in new[] { "Meta", "Small", "Label", "Body", "Section", "Title", "Page", "Display" })
        {
            Assert.Contains($"Font.{name}.Size", text);
            Assert.Contains($"Font.{name}.LineHeight", text);
        }
    }
}
