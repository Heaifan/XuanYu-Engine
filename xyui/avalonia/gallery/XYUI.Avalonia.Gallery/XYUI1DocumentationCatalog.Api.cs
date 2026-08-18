namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocProperty> Properties(string id) => id switch
    {
        "XYUI-1-04" => [P("Text", "string", "\"\""), P("Variant", "XyuiHeadingVariant", "PanelTitle")],
        "XYUI-1-06" => [P("Content", "object", "null", "按钮内容")],
        "XYUI-1-09" => [P("Text", "string", "\"\""), P("Variant", "XyuiBadgeVariant", "Default")],
        "XYUI-1-10" => [P("Text", "string", "\"\""), P("State", "XyuiStatusState", "Neutral")],
        "XYUI-1-11" => [P("State", "XyuiStatusState", "Neutral")],
        "XYUI-1-12" => [P("Glyph", "string", "•"), P("Size", "XyuiIconSize", "Medium")],
        "XYUI-1-13" => [P("IconGlyph", "string", "•"), P("Label", "string", "\"\"")],
        "XYUI-1-14" => [P("Variant", "XyuiSeparatorVariant", "Default")],
        "XYUI-1-18" => [P("Shortcut", "string", "\"\"")],
        "XYUI-1-19" => [P("Content", "object", "null", "提示内容")],
        "XYUI-1-24" => [P("Text", "string", "\"\""), P("Mode", "XyuiTruncatedTextMode", "End")],
        _ => [P("Text", "string", "\"\"")]
    };

    static IReadOnlyList<XYUIDocToken> Tokens(string id) => id switch
    {
        "XYUI-1-04" => [T("字体", "XY.Font.UI"), T("层级", "XY.FontSize.PanelTitle / PageTitle")],
        "XYUI-1-07" or "XYUI-1-08" => [T("字体", "XY.Font.Mono"), T("字号", "XY.FontSize.Mono")],
        "XYUI-1-09" or "XYUI-1-10" => [T("表面", "XY.Surface.PanelAlt"), T("语义色", "XY.Semantic.*.Text")],
        "XYUI-1-12" or "XYUI-1-13" => [T("字体", "XY.Font.UI"), T("尺寸", "XY.Icon.Size.*")],
        "XYUI-1-14" => [T("分割线", "XY.Divider.*"), T("间距", "XY.Panel.SectionGap")],
        "XYUI-1-16" => [T("语义色", "XY.Semantic.Error.Text")],
        "XYUI-1-17" => [T("语义色", "XY.Semantic.Warning.Text")],
        "XYUI-1-18" => [T("字体", "XY.Font.Mono"), T("边框", "XY.Border.Color.Subtle")],
        "XYUI-1-19" => [T("表面", "XY.Surface.Overlay"), T("圆角", "XY.Radius.Popup")],
        "XYUI-1-22" => [T("字体", "XY.Caption.Default"), T("前景", "XY.Text.Tertiary")],
        "XYUI-1-23" => [T("高亮", "XY.Accent.Soft"), T("前景", "XY.Text.Primary")],
        _ => [T("字体", "XY.Font.UI"), T("前景", "XY.Text.Primary")]
    };

    static XYUIDocProperty P(string name, string type, string value, string description = "组件属性") =>
        new(name, type, value, description);

    static XYUIDocToken T(string name, string value) =>
        new(name, value, "来自 XYUI Foundation 的语义 Token");
}
