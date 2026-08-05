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
    TokenDeclaration,
}

public sealed record BaselineEntry(
    string WId,
    string Path,
    string Locator,
    UiRuleKind Kind,
    string Property,
    string Value,
    int AllowedCount = 1,
    string Phase = "D3");

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

    // 规范化比较：hex 大小写不敏感。
    private static string Norm(string s) => s.Trim().ToUpperInvariant();

    // 细粒度指纹匹配：Path + Locator + Kind + Property + Value 全部参与。
    public static int AllowedCountFor(string path, string locator, UiRuleKind kind, string property, string value) =>
        Entries
            .Where(e => e.Path == path && e.Locator == locator && e.Kind == kind
                && e.Property == property && Norm(e.Value ?? "") == Norm(value ?? ""))
            .Select(e => e.AllowedCount)
            .DefaultIfEmpty(0)
            .First();
}
