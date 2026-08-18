using Avalonia.Controls;
using XYUI.Avalonia.Catalog;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public sealed record XYUI1GalleryItem(
    string Id, string Name, string Title, Control Preview, string Variants,
    string States, string Usage, string CanonicalDependencies, string AvaloniaType);

public static class XYUI1GalleryCatalog
{
    public static IReadOnlyList<XYUI1GalleryItem> Build()
    {
        return XyuiCatalogSource.Load().Where(x => x.Module == "XYUI-1")
            .Select(Create).ToArray();
    }

    static XYUI1GalleryItem Create(XyuiCatalogEntry entry)
    {
        var type = entry.AvaloniaType.Split('.').Last();
        return new(entry.CanonicalId, entry.Name, entry.Title, CreatePreview(entry.CanonicalId),
            entry.Variants, entry.States, Usage(entry.CanonicalId, type), entry.ApiText, type);
    }

    public static Control CreatePreview(string id) => id switch
    {
        "XYUI-1-01" => new XYText { Text = "普通正文：地图数据说明" },
        "XYUI-1-02" => new XYLabel { Text = "字段名称" },
        "XYUI-1-03" => new XYCaption { Text = "次级说明 · 2026-08-18" },
        "XYUI-1-04" => new XYHeading { Text = "区域数据集", Variant = XyuiHeadingVariant.PanelTitle },
        "XYUI-1-05" => new XYSectionTitle { Text = "属性分组" },
        "XYUI-1-06" => new XYLink { Content = "打开对象文档" },
        "XYUI-1-07" => new XYCodeText { Text = "region-7ad21c" },
        "XYUI-1-08" => new XYMonoText { Text = "X = 421.482   Y = 718.215" },
        "XYUI-1-09" => new XYBadge { Text = "草稿", Variant = XyuiBadgeVariant.Default },
        "XYUI-1-10" => new XYStatusBadge { Text = "已保存", State = XyuiStatusState.Success },
        "XYUI-1-11" => new XYStatusDot { State = XyuiStatusState.Info },
        "XYUI-1-12" => new XYIcon { Glyph = "◇", Size = XyuiIconSize.Medium },
        "XYUI-1-13" => new XYIconLabel { IconGlyph = "◇", Label = "区域" },
        "XYUI-1-14" => new XYSeparator { Variant = XyuiSeparatorVariant.Section, Width = 240 },
        "XYUI-1-15" => new XYHelpText { Text = "填写后可在层级树中定位对象。" },
        "XYUI-1-16" => new XYErrorText { Text = "名称不能为空。" },
        "XYUI-1-17" => new XYWarningText { Text = "数据尚未保存。" },
        "XYUI-1-18" => new XYShortcutHint { Shortcut = "Ctrl + S" },
        "XYUI-1-19" => new XYTooltip { Content = new XYCaption { Text = "悬浮提示内容" } },
        "XYUI-1-20" => new XYRichText { Text = "普通内容", StrongText = "重点信息", MonoText = "region-7ad21c" },
        "XYUI-1-21" => new XYSelectableText { Text = "可复制的对象 ID" },
        "XYUI-1-22" => new XYEmptyText { Text = "暂无区域数据" },
        "XYUI-1-23" => new XYSearchHighlight { Text = "命中：区域数据集" },
        "XYUI-1-24" => new XYTruncatedText { Text = "这是一个在窄布局中使用末尾省略策略的长对象名称" },
        _ => new TextBlock { Text = "未注册组件" }
    };

    static string Usage(string id, string type) => id switch
    {
        "XYUI-1-04" => $"<c:{type} Text=\"标题\" Variant=\"PanelTitle\" />",
        "XYUI-1-06" => $"<c:{type} Content=\"文档\" />",
        "XYUI-1-09" => $"<c:{type} Text=\"标签\" Variant=\"Accent\" />",
        "XYUI-1-10" => $"<c:{type} Text=\"已保存\" State=\"Success\" />",
        "XYUI-1-11" => $"<c:{type} State=\"Info\" />",
        "XYUI-1-12" => $"<c:{type} Glyph=\"◇\" Size=\"Medium\" />",
        "XYUI-1-13" => $"<c:{type} IconGlyph=\"◇\" Label=\"区域\" />",
        "XYUI-1-14" => $"<c:{type} Variant=\"Section\" />",
        "XYUI-1-18" => $"<c:{type} Shortcut=\"Ctrl + S\" />",
        "XYUI-1-19" => $"<c:{type} Content=\"提示\" />",
        "XYUI-1-21" => $"<c:{type} Text=\"可复制 ID\" SelectionStart=\"0\" SelectionEnd=\"4\" />",
        "XYUI-1-24" => $"<c:{type} Text=\"长文本\" Mode=\"End\" />",
        _ => $"<c:{type} Text=\"示例内容\" />"
    };
}
