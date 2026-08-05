using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D2：颜色 Token 数值合同（UI Spec 1.0 §4.1~§4.4 + 组件色）。
// 键、类型与色值来自正式规范；色值格式统一 #RRGGBB（6 位 hex）。

public sealed class UiTokenColorValuesTests
{
    private static readonly string CoreFile = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Design", "UiTokens.Colors.Core.axaml");
    private static readonly string CompFile = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Design", "UiTokens.Colors.Components.axaml");

    public static IEnumerable<object[]> CoreData()
    {
        yield return new object[] { "Color.Bg.Application", "#F3F6F8" };
        yield return new object[] { "Color.Bg.Panel", "#F8FAFB" };
        yield return new object[] { "Color.Bg.Control", "#FFFFFF" };
        yield return new object[] { "Color.Bg.Overlay", "#FFFFFF" };
        yield return new object[] { "Color.Border.Default", "#D5DEE4" };
        yield return new object[] { "Color.Border.Strong", "#B9C7D0" };
        yield return new object[] { "Color.Text.Primary", "#243744" };
        yield return new object[] { "Color.Text.Secondary", "#5D6F7C" };
        yield return new object[] { "Color.Text.Disabled", "#929FA8" };
        yield return new object[] { "Color.Accent", "#326F8A" };
        yield return new object[] { "Color.Accent.Hover", "#285F77" };
        yield return new object[] { "Color.Selection.Bg", "#E5F0F4" };
        yield return new object[] { "Color.Hover.Bg", "#EEF4F6" };
        yield return new object[] { "Color.Focus", "#2B6F9B" };
        yield return new object[] { "Color.Success", "#2F7658" };
        yield return new object[] { "Color.Warning", "#93651F" };
        yield return new object[] { "Color.Error", "#B14A4A" };
        yield return new object[] { "Color.Danger", "#A53F43" };
        yield return new object[] { "Color.Object.System", "#7B8794" };
        yield return new object[] { "Color.Object.User", "#326F8A" };
    }

    public static IEnumerable<object[]> ComponentData()
    {
        yield return new object[] { "Log.Accent.Error", "#c75b5b" };
        yield return new object[] { "Log.Accent.Warning", "#d89b32" };
        yield return new object[] { "Log.Accent.Info", "#4f7fb8" };
        yield return new object[] { "Log.Accent.Debug", "#6b7a90" };
        yield return new object[] { "Log.Accent.Trace", "#8b96a8" };
        yield return new object[] { "Log.RepeatText", "#7a5a19" };
        yield return new object[] { "DocStatus.SuccessBg", "#eef7f1" };
        yield return new object[] { "DocStatus.WarningBg", "#fff7df" };
        yield return new object[] { "DocStatus.ErrorBg", "#fdeeee" };
        yield return new object[] { "DocStatus.ErrorText", "#a43f3f" };
        yield return new object[] { "DocStatus.SaveHighlightBg", "#fff6dd" };
        yield return new object[] { "Layer.Kind.Region.Bg", "#E8F3F6" };
        yield return new object[] { "Layer.Kind.Region.Text", "#326B7B" };
        yield return new object[] { "Layer.Kind.System.Text", "#687582" };
        yield return new object[] { "Layer.State.Visible", "#326F8A" };
        yield return new object[] { "Layer.State.Locked", "#7A6238" };
        yield return new object[] { "Layer.DropLine", "#7FA8C6" };
        yield return new object[] { "Tree.Guide", "#C7D7EA" };
    }

    [Theory]
    [MemberData(nameof(CoreData))]
    public void Core_color_token_matches_spec(string key, string hex) =>
        AssertValue(CoreFile, key, hex);

    [Theory]
    [MemberData(nameof(ComponentData))]
    public void Component_color_token_matches_spec(string key, string hex) =>
        AssertValue(CompFile, key, hex);

    [Fact]
    public void All_color_values_use_6_digit_hex_format()
    {
        foreach (var f in new[] { CoreFile, CompFile })
        {
            var lines = File.ReadAllLines(f).Where(l => l.Contains("Color="));
            Assert.All(lines, l => Assert.Matches("#[0-9A-Fa-f]{6}\"", l));
        }
    }

    private static void AssertValue(string file, string key, string hex)
    {
        var line = File.ReadAllLines(file).Single(l => l.Contains($"x:Key=\"{key}\""));
        Assert.Contains($"Color=\"{hex}\"", line);
    }
}
