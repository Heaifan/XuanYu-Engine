using Avalonia.Controls;
using XYUI.Avalonia.Catalog;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

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
        "XYUI-1-07" => new XYCodeText { Text = "terrain/main-heightfield" },
        "XYUI-1-08" => XYMonoPreviewFactory.Create(),
        "XYUI-1-09" => XYBadgePreviewFactory.Create(),
        "XYUI-1-10" => new XYStatusBadge { Text = "Compiled", State = XyuiStatusState.Success },
        "XYUI-1-11" => new XYStatusDot { State = XyuiStatusState.Info },
        "XYUI-1-12" => new XYIcon { Icon = XyuiVectorIcon.Search, Size = XyuiIconSize.Comfortable },
        "XYUI-1-13" => new XYIconLabel { Icon = XyuiVectorIcon.Info, Label = "区域" },
        "XYUI-1-14" => new XYSeparator { Variant = XyuiSeparatorVariant.Section, Width = 240 },
        "XYUI-1-15" => new XYHelpText { Text = "修改将在下次启动时生效" },
        "XYUI-1-16" => new XYErrorText { Text = "路径不存在" },
        "XYUI-1-17" => new XYWarningText { Text = "资源尚未保存" },
        "XYUI-1-18" => new XYShortcutHint { Shortcut = "Ctrl+Shift+S" },
        "XYUI-1-19" => new XYTooltip { Content = new XYCaption { Text = "在当前工程中检索 (Ctrl+F)" } },
        "XYUI-1-20" => new XYRichText { Text = "着色器编译完成：", StrongText = "18 个着色器", MonoText = "pipeline_04 · 2.4s" },
        "XYUI-1-21" => XYSelectableTextPreviewFactory.Create(),
        "XYUI-1-22" => new XYEmptyText { Text = "未找到符合条件的着色器资源" },
        "XYUI-1-23" => new XYSearchHighlight { Text = "World_terrain_chunk_loader" },
        "XYUI-1-24" => new XYTruncatedText { Text = "Textures/Environment/Atmosphere/skybox_hdr_v3.dds", Mode = XyuiTruncatedTextMode.End },
        _ => new TextBlock { Text = "未注册组件" }
    };

    static string Usage(string id, string type) => id switch
    {
        "XYUI-1-04" => $"<c:{type} Text=\"标题\" Variant=\"PanelTitle\" />",
        "XYUI-1-06" => $"<c:{type} Content=\"文档\" />",
        "XYUI-1-09" => $"<c:{type} Text=\"标签\" Variant=\"Accent\" />",
        "XYUI-1-10" => $"<c:{type} Text=\"已保存\" State=\"Success\" />",
        "XYUI-1-11" => $"<c:{type} State=\"Info\" />",
        "XYUI-1-12" => $"<c:{type} Icon=\"Code\" Size=\"Medium\" />",
        "XYUI-1-13" => $"<c:{type} Icon=\"Info\" Label=\"区域\" />",
        "XYUI-1-14" => $"<c:{type} Variant=\"Section\" />",
        "XYUI-1-18" => $"<c:{type} Shortcut=\"Ctrl + S\" />",
        "XYUI-1-19" => $"<c:{type} Content=\"提示\" />",
        "XYUI-1-21" => $"<c:{type} Text=\"region-7ad21c\" Variant=\"Technical\" SelectionStart=\"0\" SelectionEnd=\"4\" />",
        "XYUI-1-24" => $"<c:{type} Text=\"长文本\" Mode=\"End\" />",
        _ => $"<c:{type} Text=\"示例内容\" />"
    };
}
