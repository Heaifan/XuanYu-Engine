namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static string[] Usages(string id, string type) => id switch
    {
        "XYUI-2-01" => [$"<c:{type} Content=\"新建\" />", "<c:XYButton Content=\"删除\" Variant=\"Danger\" />"],
        "XYUI-2-02" => [$"<c:{type} Icon=\"Search\" AutomationProperties.Name=\"搜索\" IsSelected=\"true\" />"],
        "XYUI-2-03" => [$"<c:{type} Content=\"网格吸附\" IsChecked=\"true\" />"],
        "XYUI-2-04" => [$"<c:{type} Content=\"新建\" />", "<c:XYSplitButton Content=\"导入\" />"],
        "XYUI-2-05" => [$"<c:{type} Content=\"导出\" />", $"<c:{type} Content=\"排序\" />"],
        "XYUI-2-10" => ["<c:XYNumberField Value=\"125\" />", "<c:XYNumberField Value=\"72\" Suffix=\"%\" />"],
        "XYUI-2-12" => [$"<c:{type} Placeholder=\"选择地区\" ItemsSource=\"候选集合\" />", $"<c:{type} Text=\"North\" IsCustomValueAllowed=\"false\" />"],
        "XYUI-2-13" => [$"<c:{type} Placeholder=\"选择状态\" ItemsSource=\"活动|暂停|归档\" />", $"<c:{type} SelectedIndex=\"1\" ItemsSource=\"性能|均衡|质量\" />"],
        "XYUI-2-14" => ["<c:XYTextArea Placeholder=\"请描述问题……\" />", "<c:XYTextArea Mode=\"Editor\" EditorType=\"JSON\" MaxHeight=\"150\" />"],
        "XYUI-2-15" => ["<c:XYSearchField Placeholder=\"搜索地图、区域或资源\" />", "<c:XYSearchField Text=\"北部区域\" FilterActive=\"true\" />"],
        "XYUI-2-16" => ["<c:XYPasswordField Placeholder=\"请输入访问密码\" />", "<c:XYPasswordField Password=\"部署密钥\" />"],
        "XYUI-2-17" => [$"<c:{type} SelectedDate=\"2026-08-12\" />", $"<c:{type} SelectedDate=\"2028-02-29\" />"],
        "XYUI-2-18" => [$"<c:{type} Time=\"14:30:25\" ShowSeconds=\"true\" />", $"<c:{type} Time=\"09:05:00\" ShowSeconds=\"false\" />"],
        "XYUI-2-19" => [$"<c:{type} Color=\"#326F8A\" Mode=\"RGB\" />", $"<c:{type} Color=\"#326F8A8C\" Mode=\"RGBA\" />"],
        "XYUI-2-20" => [$"<c:{type} Label=\"显示网格\" Value=\"true\" />", $"<c:{type} Label=\"只读状态\" Value=\"false\" IsReadOnly=\"true\" />"],
        "XYUI-2-21" => [$"<c:{type} Label=\"最大速度\" Value=\"8.42\" Suffix=\"米/秒\" />", $"<c:{type} Label=\"质量\" Value=\"12.50\" Suffix=\"千克\" />"],
        "XYUI-2-22" => [$"<c:{type} Label=\"位置\" Dimension=\"Vector3\" />", $"<c:{type} Label=\"缩放\" Dimension=\"Vector2\" />"],
        "XYUI-2-23" => [$"<c:{type} Label=\"渲染模式\" ItemsSource=\"实体|线框|点\" />", $"<c:{type} Label=\"质量等级\" SelectedIndex=\"1\" ItemsSource=\"低|中|高\" />"],
        "XYUI-2-24" => [$"<c:{type} Label=\"目标实体\" Reference=\"实体:Infantry_023\" />", $"<c:{type} Label=\"数据集\" Reference=\"数据集:Road-01\" />"],
        _ => [$"<c:{type} />"]
    };
}
