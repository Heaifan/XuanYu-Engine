namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<XYUIDocVariant> Phase1DAnatomy(string id) => id switch
    {
        "XYUI-1-19" => [
            new("Content Host", "ContentControl 浮层内容承载体", "Content property"),
            new("Border Left", "2 DIP 重点色左边框线", "BorderThickness 2,0,0,0"),
            new("Parameters", "ShowDelay(400) / MaxWidth(280) / ViewportAvoidance(true)", "Behavior contract")
        ],
        "XYUI-1-20" => [
            new("Normal Run", "基础正文段落", "Run(Text)"),
            new("Strong Run", "加粗强调段落", "Run(StrongText) SemiBold"),
            new("Mono Run", "等宽代码段落", "Run(MonoText) FontMono")
        ],
        "XYUI-1-21" => [
            new("Default", "默认常规字阶只读选区文本", "Variant=\"Default\""),
            new("Technical", "等宽技术值风格可选择文本", "Variant=\"Technical\""),
            new("Copy Target", "8 DIP 浅灰矢量拷贝角标", "Built-in Copy button")
        ],
        "XYUI-1-22" => [
            new("Role", "轻量纯文本空状态反馈", "Lightweight empty text"),
            new("Typography", "Caption (11 DIP) 次级字阶", "Non-intrusive style")
        ],
        "XYUI-1-23" => [
            new("Result Text", "搜索命中文本本体", "Precomputed match string"),
            new("Corner Mark", "8 DIP 矢量放大镜角标 (TopRight)", "Search vector mark")
        ],
        "XYUI-1-24" => [
            new("End", "末尾截断省略策略 (长对象名称...)", "Mode=\"End\" (Default)"),
            new("Middle", "居中省略策略 (当前 Avalonia 降级为 EndEllipsis)", "Mode=\"Middle\" (GAP-002)")
        ],
        _ => []
    };
}
