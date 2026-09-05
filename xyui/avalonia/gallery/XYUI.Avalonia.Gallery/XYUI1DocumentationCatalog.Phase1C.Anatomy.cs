namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocVariant> Phase1CAnatomy(string id) => id switch
    {
        "XYUI-1-14" => [
            new("Default", "默认水平分割线，内联无边距", "Horizontal divider (0 inset)"),
            new("Header", "标题下方分割线，贴合边框", "Header divider (0 inset)"),
            new("Panel", "面板间结构分割线，贴合边框", "Panel divider (0 inset)"),
            new("Section", "区块属性分隔线，带 Space2 (8 DIP) 内嵌", "Section divider (8 DIP inset)"),
            new("ListRow", "列表行项分隔线，带 Space4 (16 DIP) 内嵌", "ListRow divider (16 DIP inset)"),
            new("VerticalSplit", "主工作区与侧栏之间的垂直分隔线", "Vertical divider (1 DIP width)")
        ],
        "XYUI-1-15" => [
            new("Composition", "XyuiVectorTextSurface 承载矢量 Mark 与 TextBlock", "Surface composition"),
            new("Mark Geometry", "8 DIP Info 装饰性信息标记矢量路径", "Info mark vector primitive"),
            new("Gap", "标准间距 Space1 (4 DIP)", "Horizontal 4 DIP spacing"),
            new("Typography", "Caption (11 DIP / Normal 400)", "Caption typography")
        ],
        "XYUI-1-16" => [
            new("Composition", "XyuiVectorTextSurface 承载矢量 Mark 与 TextBlock", "Surface composition"),
            new("Mark Geometry", "8 DIP Error 错误叉号标记矢量路径", "Error mark vector primitive"),
            new("Semantic Family", "Mark 与 Text 100% 共享 Semantic.Error 色族", "Unified error palette"),
            new("Typography", "Caption (11 DIP / Medium 500)", "Caption medium typography")
        ],
        "XYUI-1-17" => [
            new("Composition", "XyuiVectorTextSurface 承载矢量 Mark 与 TextBlock", "Surface composition"),
            new("Mark Geometry", "8 DIP Warning 警告叹号标记矢量路径", "Warning mark vector primitive"),
            new("Semantic Family", "Mark 与 Text 100% 共享 Semantic.Warning 色族", "Unified warning palette"),
            new("Typography", "Caption (11 DIP / Medium 500)", "Caption medium typography")
        ],
        "XYUI-1-18" => [
            new("Structure", "[Keycap] + [Separator] + [Keycap] 键帽结构", "SeparateKeycaps structure"),
            new("CombinationMode", "当前唯一模式：SeparateKeycaps", "Mode enum"),
            new("Keycap Surface", "22 DIP 高度 / PanelAlt 浅底色 / Subtle 边框", "Keycap container"),
            new("Typography", "Caption Mono 等宽字体族群", "Mono typography")
        ],
        _ => []
    };
}
