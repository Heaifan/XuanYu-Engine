namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocProperty> Properties(string id) => id switch
    {
        "XYUI-1-04" => [P("Text", "string", "\"\""), P("Variant", "XyuiHeadingVariant", "PanelTitle")],
        "XYUI-1-06" => [P("Content", "object", "null", "按钮内容")],
        "XYUI-1-08" => [P("Rows", "ObservableCollection<XYMonoDataRow>", "[]", "每行提供 Label / Value / Unit")],
        "XYUI-1-09" => [P("Text", "string", "\"\""), P("Variant", "XyuiBadgeVariant", "Default")],
        "XYUI-1-10" => [P("Text", "string", "\"\""), P("State", "XyuiStatusState", "Neutral")],
        "XYUI-1-11" => [P("State", "XyuiStatusState", "Neutral")],
        "XYUI-1-12" => [P("Icon", "XyuiVectorIcon", "Info", "来自 XYUI Vector Icon Registry"), P("Size", "XyuiIconSize", "Medium"), P("StrokeWidth", "double", "1.5", "随尺寸变体联动")],
        "XYUI-1-20" => [P("Text", "string", "\"\""), P("StrongText", "string", "\"\""), P("MonoText", "string", "\"\"")],
        "XYUI-1-13" => [P("Icon", "XyuiVectorIcon", "Info", "来自 XYUI Vector Icon Registry"), P("Label", "string", "\"\"")],
        "XYUI-1-14" => [P("Variant", "XyuiSeparatorVariant", "Default")],
        "XYUI-1-18" => [P("Shortcut", "string", "\"\""), P("CombinationMode", "XyuiShortcutCombinationMode", "SeparateKeycaps")],
        "XYUI-1-19" => [P("Content", "object", "null", "提示内容"), P("MaxWidth", "double", "280"), P("ShowDelay", "int", "400"), P("ViewportAvoidance", "bool", "true"), P("AutoFlip", "bool", "true"), P("PointerCapture", "bool", "false"), P("InteractiveContent", "bool", "false")],
        "XYUI-1-21" => [P("Text", "string", "\"\""), P("Variant", "XyuiSelectableTextVariant", "Default"), P("CopyIcon", "XyuiVectorIcon", "Copy", "Hover 时显示矢量复制提示")],
        "XYUI-1-24" => [P("Text", "string", "\"\""), P("Mode", "XyuiTruncatedTextMode", "End")],
        _ => [P("Text", "string", "\"\"")]
    };

    static IReadOnlyList<XYUIDocToken> Tokens(string id) => id switch
    {
        "XYUI-1-05" => [T("方案", "S-05 · Soft Header + Left Mark"), T("字号 / 行高", "14 / 18 DIP"), T("背景", "#EEF3F6"), T("Left Mark", "3 × 16 DIP / #526873"), T("前景", "#243744"), T("圆角", "3 DIP")],
        "XYUI-1-04" => [T("字体", "XY.Font.UI"), T("层级", "XY.FontSize.PanelTitle / PageTitle")],
        "XYUI-1-07" => [T("正文", "XY.Text.Tertiary"), T("Code Mark", "XY.Icon.Mark"), T("尺寸 / Stroke", "8 / 1.25 DIP"), T("字号", "XY.FontSize.Mono")],
        "XYUI-1-08" => [T("方案", "M-05A · Structured Mono Data"), T("列宽", "Label Auto / Value Auto / Unit Auto"), T("Label", "完整显示 / UI Semibold / Left"), T("Value", "XY.Font.Mono / Regular / Right"), T("Unit", "UI Semibold / Left"), T("列间距", "XY.Space.4 + XY.Space.1 / XY.Space.2（20 / 8 DIP）")],
        "XYUI-1-09" => [T("方案", "Left Pointer Tag"), T("尺寸", "Auto Width / 22 DIP Height / 11 DIP Pointer"), T("Default", "XY.Surface.PanelAlt / XY.Text.Secondary"), T("Accent", "XY.Tag.Accent / XY.Accent.Default")],
        "XYUI-1-10" => [T("表面", "XY.Surface.PanelAlt"), T("语义色", "XY.Semantic.*.Text")],
        "XYUI-1-12" or "XYUI-1-13" => [T("字体", "XY.Font.UI"), T("尺寸", "XY.Icon.Size.*")],
        "XYUI-1-14" => [T("分割线", "XY.Divider.*"), T("间距", "XY.Panel.SectionGap")],
        "XYUI-1-16" => [T("语义色", "XY.Semantic.Error.Text")],
        "XYUI-1-17" => [T("语义色", "XY.Semantic.Warning.Text")],
        "XYUI-1-18" => [T("字体", "XY.Font.Mono"), T("边框", "XY.Border.Color.Subtle")],
        "XYUI-1-19" => [T("表面", "XY.Surface.Overlay"), T("圆角", "XY.Radius.Popup")],
        "XYUI-1-21" => [T("正文", "XY.Text.Primary / Technical: XY.Text.Secondary"), T("Copy Mark", "XY.Text.Disabled / 8 DIP / Uniform Scale"), T("正文→Mark", "XY.Space.2（8 DIP）"), T("选择", "XY.Surface.Selected / XY.Text.Primary")],
        "XYUI-1-22" => [T("字体", "XY.Caption.Default"), T("前景", "XY.Text.Tertiary")],
        "XYUI-1-23" => [T("高亮", "XY.Accent.Soft"), T("前景", "XY.Text.Primary"), T("Search Mark", "XY.Text.Disabled / 8 DIP / Uniform Scale / 8 DIP Gap")],
        _ => [T("字体", "XY.Font.UI"), T("前景", "XY.Text.Primary")]
    };

    static XYUIDocProperty P(string name, string type, string value, string description = "组件属性") =>
        new(name, type, value, description);

    static XYUIDocToken T(string name, string value) =>
        new(name, value, "来自 XYUI Foundation 的语义 Token");
}
