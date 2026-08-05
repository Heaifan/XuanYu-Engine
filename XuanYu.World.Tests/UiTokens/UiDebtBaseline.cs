using System.Collections.Generic;
using System.Linq;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D2：旧 UI 债务细粒度基线。
// 基线 = 审计矩阵 W01~W71 的自动化子集快照（自动生成自真实 UI 源码现状值，见 UiDebtBaseline.Colors.* / .Typography.cs）。
// 原则：已知债务允许、新增债务失败、债务减少允许、基线不自动增长（增加基线项必须独立治理批准）。

public enum UiRuleKind
{
    HexColor,
    FontSize,
    CornerRadius,
    ControlHeight,
    BoxShadow,
    StrokeThickness,
    CsHexColor,
    EmojiIcon,
}

public sealed record BaselineEntry(
    string WId,
    string Path,
    UiRuleKind Kind,
    string Property,
    string Value,
    int AllowedCount = 1);

internal static partial class UiDebtBaseline
{
    public static IReadOnlyList<BaselineEntry> Entries { get; } = Build();

    private static IReadOnlyList<BaselineEntry> Build()
    {
        var list = new List<BaselineEntry>();
        AddAxaml1(list);
        AddAxaml2(list);
        AddCs(list);
        AddTypography(list);
        return list;
    }

    // 规范化比较：hex 大小写不敏感，数值去掉前导零。
    private static string Norm(string s) => s.Trim().ToUpperInvariant();

    public static int AllowedCountFor(string path, UiRuleKind kind, string value) =>
        Entries
            .Where(e => e.Path == path && e.Kind == kind
                && Norm(e.Value ?? "") == Norm(value ?? ""))
            .Select(e => e.AllowedCount)
            .DefaultIfEmpty(0)
            .First();
}
