using System.Text.Json;
using Avalonia.Controls;
using XYUI.Avalonia.Catalog;
namespace XYUI.Avalonia.Gallery;
public static class XYUI2DocumentationCatalog
{
    public static readonly IReadOnlySet<string> BatchIds = new HashSet<string>
    {
        "XYUI-2-01", "XYUI-2-02", "XYUI-2-03", "XYUI-2-04", "XYUI-2-05", "XYUI-2-06", "XYUI-2-07", "XYUI-2-08", "XYUI-2-09", "XYUI-2-10", "XYUI-2-11", "XYUI-2-12", "XYUI-2-13", "XYUI-2-14", "XYUI-2-15", "XYUI-2-16", "XYUI-2-17", "XYUI-2-18", "XYUI-2-19", "XYUI-2-20"
    };
    public static IReadOnlyList<XYUI1ComponentDocument> Build() =>
        XyuiCatalogSource.Load().Where(x => x.Module == "XYUI-2" && BatchIds.Contains(x.SourceItemId))
            .Select(Create).ToArray();
    static XYUI1ComponentDocument Create(XyuiCatalogEntry entry)
    {
        var type = entry.AvaloniaType.Split('.').Last();
        var name = ChineseName(entry);
        return new(entry.SourceItemId, name, type, entry.Description,
            entry.Usage == "See canonical spec" ? entry.States : entry.Usage,
            () => XYUI2GalleryCatalog.CreatePreview(entry.SourceItemId),
            Usages(entry.SourceItemId, type), Names(entry.Variants).Select(x => new XYUIDocVariant(x, "", "")).ToArray(),
            Names(entry.States).Select(x => new XYUIDocState(x, "")).ToArray(),
            Properties(entry.SourceItemId), Tokens(entry.SourceItemId), type)
        {
            CanonicalIdentity = entry.CanonicalIdentity, KnownGap = entry.KnownGap,
            Acceptance = PendingAcceptance
        };
    }
    public const string PendingAcceptance = "READY FOR USER VISUAL ACCEPTANCE";
    static string[] Names(string block) => block == "None defined" || block == "See canonical spec"
        ? []
        : block.Split('；', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    static string ChineseName(XyuiCatalogEntry entry) => entry.Title.Split('｜').Last().Split('/').Last().Trim();
    static string[] Usages(string id, string type) => id switch
    {
        "XYUI-2-01" => [$"<c:{type} Content=\"新建\" />", "<c:XYButton Content=\"删除\" Variant=\"Danger\" />"],
        "XYUI-2-02" => [$"<c:{type} Icon=\"Search\" AutomationProperties.Name=\"搜索\" IsSelected=\"true\" />"],
        "XYUI-2-03" => [$"<c:{type} Content=\"网格吸附\" IsChecked=\"true\" />"],
        "XYUI-2-04" => [$"<c:{type} Content=\"新建\" />", "<c:XYSplitButton Content=\"导入\" />"],
        "XYUI-2-05" => [$"<c:{type} Content=\"导出\" />", $"<c:{type} Content=\"排序\" />"],
        "XYUI-2-10" => ["<c:XYNumberField Value=\"125\" />", "<c:XYNumberField Value=\"72\" Suffix=\"%\" />"],
        "XYUI-2-12" => [$"<c:{type} Placeholder=\"选择地区\" ItemsSource=\"候选集合\" />", $"<c:{type} Text=\"North\" IsCustomValueAllowed=\"false\" />"],
        "XYUI-2-13" => [$"<c:{type} Placeholder=\"Select status\" ItemsSource=\"Active|Paused|Archived\" />", $"<c:{type} SelectedIndex=\"1\" ItemsSource=\"Performance|Balanced|Quality\" />"],
        "XYUI-2-14" => ["<c:XYTextArea Placeholder=\"请描述问题……\" />", "<c:XYTextArea Mode=\"Editor\" EditorType=\"JSON\" MaxHeight=\"150\" />"],
        "XYUI-2-15" => ["<c:XYSearchField Placeholder=\"搜索地图、区域或资源\" />", "<c:XYSearchField Text=\"北部区域\" FilterActive=\"true\" />"],
        "XYUI-2-16" => ["<c:XYPasswordField Placeholder=\"请输入访问密码\" />", "<c:XYPasswordField Password=\"部署密钥\" />"],
        "XYUI-2-17" => [$"<c:{type} SelectedDate=\"2026-08-12\" />", $"<c:{type} SelectedDate=\"2028-02-29\" />"],
        "XYUI-2-18" => [$"<c:{type} Time=\"14:30:25\" ShowSeconds=\"true\" />", $"<c:{type} Time=\"09:05:00\" ShowSeconds=\"false\" />"],
        "XYUI-2-19" => [$"<c:{type} Color=\"#326F8A\" Mode=\"RGB\" />", $"<c:{type} Color=\"#326F8A8C\" Mode=\"RGBA\" />"],
        "XYUI-2-20" => [$"<c:{type} Label=\"显示网格\" Value=\"true\" />", $"<c:{type} Label=\"只读状态\" Value=\"false\" IsReadOnly=\"true\" />"],
        _ => [$"<c:{type} />"]
    };
    static IReadOnlyList<XYUIDocProperty> Properties(string id) => id switch
    {
        "XYUI-2-06" => [new("IsChecked", "bool?", "false", "支持 Unchecked / Checked / Mixed"), new("IsThreeState", "bool", "false", "启用 Mixed 状态")],
        "XYUI-2-07" => [new("GroupName", "string", "", "同组互斥"), new("IsChecked", "bool", "false", "当前选项")],
        "XYUI-2-08" => [new("IsChecked", "bool", "false", "真实切换 Track / Thumb")],
        "XYUI-2-09" => [new("Text", "string", "", "单行文本"), new("Placeholder", "string?", "null", "占位提示"), new("IsReadOnly", "bool", "false", "只读")],
        "XYUI-2-10" => [new("Value", "double", "0", "统一数值真值"), new("Minimum", "double", "0", "下限"), new("Maximum", "double", "100", "上限"), new("Step", "double", "1", "普通步长"), new("LargeStep", "double", "10", "Shift 步长"), new("SmallStep", "double", "0.1", "Ctrl 步长"), new("Suffix", "string?", "null", "仅显示后缀"), new("DecimalPlaces", "int", "2", "显示小数位")],
        "XYUI-2-11" => [new("Value", "double", "0", "Slider 与 NumberField 的唯一真值"), new("Minimum", "double", "0", "下限"), new("Maximum", "double", "100", "上限"), new("Step", "double", "1", "普通步长"), new("LargeStep", "double", "10", "Shift 步长"), new("SmallStep", "double", "0.1", "Ctrl 步长"), new("DecimalPlaces", "int", "2", "显示小数位"), new("Suffix", "string?", "null", "仅显示后缀"), new("IsNumberFieldVisible", "bool", "true", "显示精确输入")],
        "XYUI-2-12" => [new("ItemsSource", "IEnumerable", "[]", "可编辑候选"), new("SelectedItem", "object?", "null", "当前候选"), new("IsCustomValueAllowed", "bool", "false", "允许自定义值")],
        "XYUI-2-13" => [new("ItemsSource", "IEnumerable", "[]", "固定候选"), new("SelectedIndex", "int", "-1", "当前候选索引"), new("SelectedItem", "object?", "null", "当前候选"), new("Placeholder", "string?", "null", "未选择时的提示")],
        "XYUI-2-14" => [new("Text", "string", "", "多行文本"), new("Placeholder", "string?", "null", "占位提示"), new("Mode", "XYTextAreaMode", "Standard", "标准 / 编辑模式"), new("AutoGrow", "bool", "true", "内容驱动增长"), new("MinHeight", "double", "54", "最小高度"), new("MaxHeight", "double", "Auto", "达到后内部滚动"), new("EditorType", "string", "文本", "编辑标题栏类型"), new("IsError", "bool", "false", "错误边框状态")],
        "XYUI-2-15" => [new("Text", "string", "", "搜索文本"), new("Placeholder", "string?", "null", "占位提示"), new("FilterContent", "Control?", "null", "真实筛选面板内容"), new("IsFilterOpen", "bool", "false", "筛选面板是否打开"), new("FilterActive", "bool", "false", "独立的筛选激活态"), new("IsSearching", "bool", "false", "搜索进行中"), new("IsNoResult", "bool", "false", "无结果态")],
        "XYUI-2-16" => [new("Password", "string", "", "密码文本别名"), new("Placeholder", "string?", "null", "占位提示"), new("IsRevealed", "bool", "false", "按住时临时显示")],
        "XYUI-2-17" => [new("SelectedDate", "DateOnly", "2026-08-12", "当前日期"), new("MinDate", "DateOnly?", "null", "可选最小日期"), new("MaxDate", "DateOnly?", "null", "可选最大日期"), new("DateChanged", "event", "—", "日期变化事件")],
        "XYUI-2-18" => [new("Time", "TimeOnly", "14:30:25", "当前时间"), new("ShowSeconds", "bool", "true", "显示秒分段"), new("TimeChanged", "event", "—", "时间变化事件")],
        "XYUI-2-19" => [new("Color", "Color", "#326F8A", "颜色真值"), new("Mode", "XYColorPickerMode", "RGBA", "RGB / RGBA 显示模式"), new("IsOpen", "bool", "false", "颜色面板是否打开"), new("ColorChanged", "event", "—", "颜色变化事件")],
        "XYUI-2-20" => [new("Label", "string", "属性", "属性名称"), new("Value", "bool", "false", "布尔真值"), new("IsReadOnly", "bool", "false", "只读状态"), new("ValueChanged", "event", "—", "值变化事件")],
        _ => []
    };
    static IReadOnlyList<XYUIDocToken> Tokens(string id)
    {
        var root = FindRepositoryRoot();
        if (root is null) return [];
        var path = Path.Combine(root, "xyui", "specs", "XYUI2", "XYUI-2.mapping.json");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var component = document.RootElement.GetProperty("components").EnumerateArray()
            .Single(x => x.GetProperty("component_id").GetString() == id);
        return component.GetProperty("refs").EnumerateArray().Select(item => new XYUIDocToken(
            item.GetProperty("property").GetString() ?? "",
            item.GetProperty("value").GetString() ?? "",
            item.GetProperty("kind").GetString() ?? "")).ToArray();
    }

    static string? FindRepositoryRoot()
    {
        foreach (var start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(start);
            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "xyui", "registry", "foundation", "identity-map.json")))
                    return directory.FullName;
                directory = directory.Parent;
            }
        }
        return null;
    }
}
