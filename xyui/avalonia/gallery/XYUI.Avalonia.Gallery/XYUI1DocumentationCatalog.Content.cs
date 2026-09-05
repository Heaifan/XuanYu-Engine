namespace XYUI.Avalonia.Gallery;

public static partial class XYUI1DocumentationCatalog
{
    static IReadOnlyList<string> Usages(string id, string type) => id switch
    {
        "XYUI-1-04" => [$"<xy:{type} Text=\"区域数据集\" />", $"<xy:{type} Text=\"地图编辑\" Variant=\"PageTitle\" />"],
        "XYUI-1-06" => [$"<xy:{type} Content=\"打开对象文档\" />"],
        "XYUI-1-08" => [$"var data = new {type}();", "data.Rows.Add(new XYMonoDataRow(\"X 坐标\", \"142.583\", \"m\"));", "data.Rows.Add(new XYMonoDataRow(\"对象数\", \"1,284\"));"],
        "XYUI-1-09" => [$"<xy:{type} Text=\"草稿\" />", $"<xy:{type} Text=\"已选中\" Variant=\"Accent\" />"],
        "XYUI-1-10" => [$"<xy:{type} Text=\"已保存\" State=\"Success\" />", $"<xy:{type} Text=\"未同步\" State=\"Warning\" />"],
        "XYUI-1-11" => [$"<xy:{type} State=\"Info\" />", $"<xy:{type} State=\"Error\" />"],
        "XYUI-1-12" => [$"<xy:{type} Icon=\"Code\" Size=\"Default\" />", $"<xy:{type} Icon=\"Search\" Size=\"Large\" />"],
        "XYUI-1-13" => [$"<xy:{type} Icon=\"Info\" Label=\"区域\" />"],
        "XYUI-1-14" => [$"<xy:{type} Variant=\"Section\" />", $"<xy:{type} Variant=\"VerticalSplit\" />"],
        "XYUI-1-18" => [$"<xy:{type} Shortcut=\"Ctrl + S\" />"],
        "XYUI-1-19" => [$"<xy:{type} Content=\"提示内容\" />"],
        "XYUI-1-20" => [$"<xy:{type} Text=\"普通内容\" StrongText=\"重点信息\" MonoText=\"region-7ad21c\" />"],
        "XYUI-1-21" => [$"<xy:{type} Text=\"可复制 ID\" Variant=\"Default\" />", $"<xy:{type} Text=\"region-7ad21c\" Variant=\"Technical\" />"],
        "XYUI-1-24" => [$"<xy:{type} Text=\"长对象名称\" Mode=\"End\" />", $"<xy:{type} Text=\"region-7ad21c\" Mode=\"Middle\" />"],
        _ => [$"<xy:{type} Text=\"示例内容\" />"]
    };

    static IReadOnlyList<XYUIDocVariant> Variants(string id)
    {
        var anatomy = Phase1AAnatomy(id);
        if (anatomy.Count > 0) return anatomy;
        return id switch
        {
            "XYUI-1-09" => [new("Default", "普通标签", "草稿"), new("Accent", "强调标签", "已选中")],
            "XYUI-1-14" => [new("Section", "区块分割", "属性分组"), new("VerticalSplit", "垂直分割", "主区 / 侧栏")],
            "XYUI-1-12" => [new("Tiny", "12 DIP / 1.0 DIP", "紧凑工具栏"), new("Small", "14 DIP / 1.25 DIP", "列表与内联"), new("Default", "16 DIP / 1.5 DIP", "默认图标"), new("Large", "20 DIP / 1.75 DIP", "强调入口")],
            "XYUI-1-21" => [new("Default", "普通可选择文本", "可复制 ID"), new("Technical", "技术值等宽风格", "region-7ad21c")],
            "XYUI-1-24" => [new("End", "末尾省略", "长对象名称..."), new("Middle", "中部省略", "region-...7ad21c")],
            _ => []
        };
    }

    static IReadOnlyList<XYUIDocState> States(string id) => id switch
    {
        "XYUI-1-10" or "XYUI-1-11" => [new("Success", "成功或已完成"), new("Warning", "风险或待处理"), new("Error", "失败或阻断"), new("Info", "补充状态"), new("Neutral", "中性状态")],
        _ => []
    };
}
