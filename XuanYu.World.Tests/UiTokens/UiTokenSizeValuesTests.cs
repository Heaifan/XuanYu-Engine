using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D2：尺寸/控件/图标/动效 Token 数值合同（UI Spec 1.0 §5.1~§6.4/§8.1/§9/§15.3）。

public sealed class UiTokenSizeValuesTests
{
    private static readonly string Dir = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Design");

    public static IEnumerable<object[]> Data()
    {
        yield return new object[] { "UiTokens.Spacing.axaml", "Space.2", "2" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Space.4", "4" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Space.6", "6" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Space.8", "8" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Space.12", "12" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Space.16", "16" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Space.24", "24" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Space.32", "32" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Padding.Compact", "6,2" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Padding.Standard", "8,4" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Padding.Relaxed", "12,6" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Radius.Small", "3" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Radius.Standard", "6" };
        yield return new object[] { "UiTokens.Spacing.axaml", "Radius.Large", "10" };
        yield return new object[] { "UiTokens.Controls.axaml", "Control.Height.Compact", "24" };
        yield return new object[] { "UiTokens.Controls.axaml", "Control.Height.Standard", "28" };
        yield return new object[] { "UiTokens.Controls.axaml", "Control.Height.Emphasized", "32" };
        yield return new object[] { "UiTokens.Controls.axaml", "Size.Width.64", "64" };
        yield return new object[] { "UiTokens.Controls.axaml", "Size.Width.96", "96" };
        yield return new object[] { "UiTokens.Controls.axaml", "Size.Width.128", "128" };
        yield return new object[] { "UiTokens.Controls.axaml", "Size.Width.160", "160" };
        yield return new object[] { "UiTokens.Controls.axaml", "Size.Width.240", "240" };
        yield return new object[] { "UiTokens.Controls.axaml", "Control.LabelColumn.Width", "96" };
        yield return new object[] { "UiTokens.Controls.axaml", "Control.Field.MinWidth", "128" };
        yield return new object[] { "UiTokens.Controls.axaml", "Size.Hit.Compact", "24" };
        yield return new object[] { "UiTokens.Controls.axaml", "Size.Hit.Standard", "28" };
        yield return new object[] { "UiTokens.Controls.axaml", "Size.Hit.Touch", "36" };
        yield return new object[] { "UiTokens.Controls.axaml", "Border.Width.Default", "1" };
        yield return new object[] { "UiTokens.Controls.axaml", "Border.Width.Insert", "2" };
        yield return new object[] { "UiTokens.Controls.axaml", "Border.Width.Focus", "2" };
        yield return new object[] { "UiTokens.Controls.axaml", "Focus.Offset", "1" };
        yield return new object[] { "UiTokens.Controls.axaml", "Shadow.OffsetY", "4" };
        yield return new object[] { "UiTokens.Controls.axaml", "Shadow.Blur", "12" };
        yield return new object[] { "UiTokens.Controls.axaml", "Shadow.Opacity", "0.14" };
        yield return new object[] { "UiTokens.Controls.axaml", "LogTable.Columns", "4,72,56,72,92,*,82" };
        yield return new object[] { "UiTokens.Icons.axaml", "Icon.Size.Standard", "16" };
        yield return new object[] { "UiTokens.Icons.axaml", "Icon.Size.Tool", "20" };
        yield return new object[] { "UiTokens.Icons.axaml", "Icon.Stroke.Width", "1.5" };
        yield return new object[] { "UiTokens.Motion.axaml", "Motion.HoverMs", "100" };
        yield return new object[] { "UiTokens.Motion.axaml", "Motion.ExpandMs", "140" };
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Size_token_matches_spec(string file, string key, string expected)
    {
        var line = File.ReadAllLines(Path.Combine(Dir, file)).Single(l => l.Contains($"x:Key=\"{key}\""));
        Assert.Contains($">{expected}<", line);
    }
}
