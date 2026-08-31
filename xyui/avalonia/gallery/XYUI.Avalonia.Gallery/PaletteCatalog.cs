using Avalonia.Media;
using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Gallery;

// 色板数据：按家族分组展示全部 canonical 颜色（值来自运行时 token 表）
public sealed record PaletteItem(string TokenId, string Hex, IBrush Brush);

public sealed record PaletteSection(string Title, IReadOnlyList<PaletteItem> Items);

public static class PaletteCatalog
{
    public static IReadOnlyList<PaletteSection> BuildSections(bool dark) =>
        XyuiColorTokens.All
            .GroupBy(t => t.TokenId.Split('.')[1])
            .Select(g => new PaletteSection(g.Key, g.Select(t =>
                new PaletteItem(t.TokenId, t.Hex(dark), new SolidColorBrush(t.ToColor(dark))))
                .ToArray()))
            .ToArray();
}
