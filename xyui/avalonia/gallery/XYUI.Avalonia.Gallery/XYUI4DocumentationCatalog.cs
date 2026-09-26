using XYUI.Avalonia.Catalog;

namespace XYUI.Avalonia.Gallery;

public static class XYUI4DocumentationCatalog
{
    static readonly IReadOnlySet<string> ComponentIds = new HashSet<string> { "XYUI-4-4.14", "XYUI-4-4.15", "XYUI-4-4.16" };

    public static IReadOnlyList<XYUI1ComponentDocument> Build() => XyuiCatalogSource.Load()
        .Where(x => ComponentIds.Contains(x.SourceItemId)).Select(Create).ToArray();

    static XYUI1ComponentDocument Create(XyuiCatalogEntry entry)
    {
        var id = entry.SourceItemId;
        var type = entry.AvaloniaType.Split('.').Last();
        return new(id, ChineseName(id), type, Overview(id), WhenToUse(id),
            () => XYUI4GalleryCatalog.CreatePreview(id), Usages(id), Variants(id), States(id),
            Properties(id), Tokens(id), type)
        {
            CanonicalIdentity = entry.CanonicalIdentity,
            Category = "Canonical Stable · Feedback / Activity",
            Acceptance = "RUNTIME IMPLEMENTED · READY FOR USER VISUAL ACCEPTANCE",
            QuickStartXaml = QuickStart(id), CoreRules = Rules(id),
            FoundationMappings = Foundations(id), HowToUse = Guides(id),
            LiveExamplesFactory = () => XYUI4GalleryCatalog.CreateLiveExamples(id)
        };
    }

    static string ChineseName(string id) => id switch { "XYUI-4-4.14" => "加载指示", "XYUI-4-4.15" => "旋转加载", _ => "进度条" };
    static string Overview(string id) => id == "XYUI-4-4.14"
        ? "LoadingIndicator 表达任务正在进行，并提供任务上下文；本轮用于 Vulkan 视口初始化。"
        : id == "XYUI-4-4.15" ? "Spinner 只表达无法可靠量化进度的持续活动图形，使用 Open Arc 造型。" : "ProgressBar 表达可信的确定性任务进度，支持百分比、标签、阶段和紧凑行内组合。";
    static string WhenToUse(string id) => id == "XYUI-4-4.14"
        ? "用于视口、资源读取和其他不确定完成时间的初始化任务。"
        : id == "XYUI-4-4.15" ? "用于 LoadingIndicator 内部或短任务入口中的不确定活动反馈。" : "用于导入、导出、构建、保存和其他可量化的长任务。";
    static string[] Usages(string id) => id == "XYUI-4-4.14"
        ? ["<c:XYLoadingIndicator Text=\"正在初始化渲染视口…\" Size=\"Compact\" />"]
        : id == "XYUI-4-4.15" ? ["<c:XYSpinner Size=\"Standard\" IsActive=\"True\" />"] : ["<c:XYProgressBar Minimum=\"0\" Maximum=\"100\" Value=\"68\" ShowPercentage=\"True\" />"];
    static XYUIDocVariant[] Variants(string id) => id == "XYUI-4-4.14"
        ? [new("Inline", "Spinner + 主标签", "Area C Compact"), new("Detail", "Spinner + 主标签 + 次级上下文", "资源或后端初始化")]
        : id == "XYUI-4-4.15" ? [new("Compact", "14 × 14", "Area C"), new("Standard / Large", "18 / 24 DIP", "通用任务反馈")] : [new("Clean Linear", "基础连续进度", "文件 / 计算"), new("Labeled", "标签 + 百分比", "长任务"), new("Segmented Stage", "阶段路径", "导入流程"), new("Inline Compact", "紧凑行内", "列表 / Inspector")];
    static XYUIDocState[] States(string id) => id == "XYUI-4-4.14"
        ? [new("Active", "任务持续时显示活动图形"), new("Failure", "任务失败后退出 Loading，转入错误反馈")]
        : id == "XYUI-4-4.15" ? [new("Active", "800–1200ms 旋转 Open Arc"), new("Reduced Motion", "静态弧，不启动计时器")] : [new("0 / 50 / 100%", "边界值正确显示"), new("Indeterminate", "仅在真实进度未知时使用")];

    static IReadOnlyList<XYUIDocProperty> Properties(string id) => id == "XYUI-4-4.14"
        ? [P("Text", "string", "正在加载…"), P("SecondaryText", "string", "空"), P("Size", "XyuiSpinnerSize", "Standard"), P("Variant", "XyuiLoadingIndicatorVariant", "Inline"), P("IsActive", "bool", "true")]
        : id == "XYUI-4-4.15" ? [P("Size", "XyuiSpinnerSize", "Standard"), P("IsActive", "bool", "true"), P("IsReducedMotion", "bool", "false"), P("Track / Arc", "IBrush?", "Theme Token")] : [P("Minimum / Maximum / Value", "double", "0 / 100 / 0"), P("IsIndeterminate", "bool", "false"), P("ShowPercentage", "bool", "true"), P("StatusText", "string?", "空"), P("Variant / Size", "enum", "CleanLinear / Standard")];
    static IReadOnlyList<XYUIDocToken> Tokens(string id) => id == "XYUI-4-4.14"
        ? [T("Indicator", "XY.Accent.Soft / XY.Color.Accent"), T("Text", "XY.Text.Secondary"), T("Background", "Transparent"), T("LayoutShift", "Forbidden")]
        : id == "XYUI-4-4.15" ? [T("Track", "XY.Accent.Soft"), T("Arc", "XY.Color.Accent"), T("Stroke", "2–3 DIP"), T("Center / Shadow", "Transparent / None")] : [T("Track / Fill", "XY.Accent.Soft / XY.Color.Accent"), T("Height", "Compact 3–5 / Standard 6–8 / Emphasis 8–10 DIP"), T("Radius / Shadow", "XY.Radius.Full / XY.Shadow.None")];

    static string QuickStart(string id) => id == "XYUI-4-4.14" ? "<c:XYLoadingIndicator Text=\"正在初始化渲染视口…\" Size=\"Compact\" />" : id == "XYUI-4-4.15" ? "<c:XYSpinner Size=\"Standard\" />" : "<c:XYProgressBar Value=\"68\" StatusText=\"正在导入 DEM\" />";
    static IReadOnlyList<XYUIDocRule> Rules(string id) => id == "XYUI-4-4.14"
        ? [new("任务上下文", "LoadingIndicator 不显示孤立 Spinner，必须保留任务标签或上下文。"), new("真实状态", "禁止伪造进度；失败后退出 Loading。")]
        : id == "XYUI-4-4.15" ? [new("Open Arc", "完整低对比轨道加一段 Accent 弧，圆帽、透明中心。"), new("动效生命周期", "不可见或 Reduced Motion 时停止计时器。")]
        : [new("真实进度", "Value 必须来自可信任务进度，禁止用动画伪造确定进度。"), new("轻量更新", "Value 更新只重绘进度条，不触发无关页面重排。")];
    static IReadOnlyList<XYUIDocFoundationItem> Foundations(string id) => [new("颜色", id == "XYUI-4-4.15" ? "XY.Accent.Soft / XY.Color.Accent" : "XY.Accent.Soft / XY.Color.Accent", "由主题动态资源提供")];
    static IReadOnlyList<XYUIDocGuideItem> Guides(string id) => [new("Gallery", id == "XYUI-4-4.14" ? "放在 Vulkan Viewport 左下角、Scale Indicator 上方。" : id == "XYUI-4-4.15" ? "作为 4.14 LoadingIndicator 的内部基础活动图形。" : "长任务使用 Labeled，阶段任务使用 Segmented，列表周边使用 Inline Compact。")];
    static XYUIDocProperty P(string n, string t, string d) => new(n, t, d, "组件属性");
    static XYUIDocToken T(string n, string v) => new(n, v, "来自 XYUI Foundation 的语义 Token");
}
