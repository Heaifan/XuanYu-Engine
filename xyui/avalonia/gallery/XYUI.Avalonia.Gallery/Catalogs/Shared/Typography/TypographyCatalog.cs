namespace XYUI.Avalonia.Gallery;

// Typography 规范页数据（token 表展示；Sample 字号为数据绑定，非 Consumer 字面量）
public sealed record TypographyItem(
    string TokenId, string Category, string Value, string Sample, double SampleSize);

public sealed record TypographySection(string Title, IReadOnlyList<TypographyItem> Items);

public static class TypographyCatalog
{
    public static IReadOnlyList<TypographySection> BuildSections() =>
    [
        new("Font Family", [
            new("XY.Font.UI", "FontFamily", "Source Han Sans SC", "思源黑体 ABC abc", 14),
            new("XY.Font.Mono", "FontFamily", "Source Code Pro", "mono 0123 abc", 14),
            new("XY.Font.Fallback.CJK", "FontFamily", "Noto Sans CJK SC", "回退链（缺字时）", 14),
            new("XY.Font.Fallback.Mono", "FontFamily", "Noto Sans Mono", "回退链（缺字时）", 14),
        ]),
        new("Font Size / Line Height", [
            new("XY.FontSize.Caption", "12 / 16", "Caption", "辅助信息", 12),
            new("XY.FontSize.Auxiliary", "13 / 18", "Auxiliary", "次级技术信息", 13),
            new("XY.FontSize.Body", "14 / 20", "Body", "正文内容", 14),
            new("XY.FontSize.Label", "15 / 20", "Label", "字段标签", 15),
            new("XY.FontSize.Section", "17 / 22", "Section", "区块标题", 17),
            new("XY.FontSize.PanelTitle", "20 / 26", "PanelTitle", "面板标题", 20),
            new("XY.FontSize.PageTitle", "24 / 30", "PageTitle", "页面标题", 24),
            new("XY.FontSize.Mono", "13 / 20", "Mono", "等宽数据 0123", 13),
        ]),
        new("Font Weight", [
            new("XY.FontWeight.Regular", "400", "Regular", "正文与字段值", 14),
            new("XY.FontWeight.Medium", "500", "Medium", "字段标签", 14),
            new("XY.FontWeight.Semibold", "600", "Semibold", "Section/Panel/Button", 14),
            new("XY.FontWeight.Bold", "700", "Bold", "页面级标题", 14),
        ]),
    ];
}
