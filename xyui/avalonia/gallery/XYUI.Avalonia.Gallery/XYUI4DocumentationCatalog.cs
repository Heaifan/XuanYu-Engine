using XYUI.Avalonia.Catalog;

namespace XYUI.Avalonia.Gallery;

public static class XYUI4DocumentationCatalog
{
    public const string LatestComponentId = "XYUI-4-4.11";
    static readonly IReadOnlySet<string> ComponentIds = new HashSet<string> { "XYUI-4-4.01", "XYUI-4-4.02", "XYUI-4-4.03", "XYUI-4-4.04", "XYUI-4-4.05", "XYUI-4-4.06", "XYUI-4-4.07", "XYUI-4-4.08", "XYUI-4-4.09", "XYUI-4-4.10", "XYUI-4-4.11", "XYUI-4-4.14", "XYUI-4-4.15", "XYUI-4-4.16" };

    public static IReadOnlyList<XYUI1ComponentDocument> Build() => XyuiCatalogSource.Load()
        .Where(x => ComponentIds.Contains(x.SourceItemId)).Select(Create).ToArray();

    static XYUI1ComponentDocument Create(XyuiCatalogEntry entry)
    {
        var id = entry.SourceItemId;
        var type = ComponentName(id);
        return new(id, ChineseName(id), type, Overview(id), WhenToUse(id),
            () => XYUI4GalleryCatalog.CreatePreview(id), Usages(id), Variants(id), States(id),
            Properties(id), Tokens(id), type)
        {
            CanonicalIdentity = entry.CanonicalIdentity,
            Category = id == "XYUI-4-4.01" ? "Canonical Stable · Selection / Feedback" : "Canonical Stable · Feedback / Activity",
            Acceptance = "RUNTIME IMPLEMENTED · READY FOR USER VISUAL ACCEPTANCE",
            QuickStartXaml = QuickStart(id), CoreRules = Rules(id),
            FoundationMappings = Foundations(id), HowToUse = Guides(id),
            LiveExamplesFactory = () => XYUI4GalleryCatalog.CreateLiveExamples(id)
        };
    }

    static string ComponentName(string id) => id switch { "XYUI-4-4.01" => "XYHoverState", "XYUI-4-4.02" => "SelectedState", "XYUI-4-4.03" => "ActiveState", "XYUI-4-4.04" => "SelectionContextFocus", "XYUI-4-4.05" => "MultiSelection", "XYUI-4-4.06" => "SelectionGroup", "XYUI-4-4.07" => "XYMarqueeSelection", "XYUI-4-4.08" => "XYLassoSelection", "XYUI-4-4.09" => "XYSelectionOutline", "XYUI-4-4.10" => "XYBoundingBox", "XYUI-4-4.11" => "XYDragFeedback", "XYUI-4-4.14" => "XYLoadingIndicator", "XYUI-4-4.15" => "XYSpinner", _ => "XYProgressBar" };
    static string ChineseName(string id) => id switch { "XYUI-4-4.01" => "悬停状态", "XYUI-4-4.02" => "选中状态", "XYUI-4-4.03" => "激活状态", "XYUI-4-4.04" => "选择上下文焦点", "XYUI-4-4.05" => "多选状态", "XYUI-4-4.06" => "选择组", "XYUI-4-4.07" => "框选", "XYUI-4-4.08" => "套索选择", "XYUI-4-4.09" => "选择轮廓", "XYUI-4-4.10" => "包围框", "XYUI-4-4.11" => "拖拽反馈", "XYUI-4-4.14" => "加载指示", "XYUI-4-4.15" => "旋转加载", _ => "进度条" };
    static string Overview(string id) => XYUI4StateDocumentation.Overview(id) ?? (id == "XYUI-4-4.01" ? "HoverState 表达指针当前命中的可交互对象，不改变选择、焦点或布局。" : id == "XYUI-4-4.11" ? "DragFeedback 表达拖拽已经开始，并提供不参与布局的 Source 与 Ghost Preview。" : id == "XYUI-4-4.14"
        ? "LoadingIndicator 表达任务正在进行，并提供任务上下文；本轮用于 Vulkan 视口初始化。"
        : id == "XYUI-4-4.15" ? "Spinner 只表达无法可靠量化进度的持续活动图形，使用 Open Arc 造型。" : "ProgressBar 表达可信的确定性任务进度，支持百分比、标签、阶段和紧凑行内组合。");
    static string WhenToUse(string id) => XYUI4StateDocumentation.WhenToUse(id) ?? (id == "XYUI-4-4.01" ? "用于按钮、菜单、列表行、输入区域和画布对象的上下文悬停反馈。" : id == "XYUI-4-4.11" ? "用于 Canvas Direct Manipulation、Tree/List Reorder 和资源拖拽传输的临时视觉反馈。" : id == "XYUI-4-4.14"
        ? "用于视口、资源读取和其他不确定完成时间的初始化任务。"
        : id == "XYUI-4-4.15" ? "用于 LoadingIndicator 内部或短任务入口中的不确定活动反馈。" : "用于导入、导出、构建、保存和其他可量化的长任务。");
    static string[] Usages(string id) => XYUI4StateDocumentation.Usages(id) is { Length: > 0 } usage ? usage : id == "XYUI-4-4.11" ? ["<c:XYDragFeedback IsDragging=\"True\" />"] : id == "XYUI-4-4.14"
        ? ["<c:XYLoadingIndicator Text=\"正在初始化渲染视口…\" Size=\"Compact\" />"]
        : id == "XYUI-4-4.15" ? ["<c:XYSpinner Size=\"Standard\" IsActive=\"True\" />"] : ["<c:XYProgressBar Minimum=\"0\" Maximum=\"100\" Value=\"68\" ShowPercentage=\"True\" />"];
    static XYUIDocVariant[] Variants(string id) => XYUI4StateDocumentation.Variants(id) is { Length: > 0 } variants ? variants : id == "XYUI-4-4.11" ? [new("Source", "弱化但保留占位", "Reorder / Direct Manipulation"), new("Ghost Preview", "跟随 Pointer 的简化预览", "Transfer / Canvas"), new("Origin", "必要的来源反馈", "Direct Manipulation")] : id == "XYUI-4-4.01" ? [new("Surface", "浅层 Surface Reveal", "按钮、菜单、列表行"), new("Border", "轻度 Border Reveal", "输入框、Inspector"), new("Outline", "几何轮廓强化", "Canvas 对象"), new("Handle", "手柄轻度强化", "Vertex / Resize Handle")] : id == "XYUI-4-4.14"
        ? [new("Inline", "Spinner + 主标签", "Area C Compact"), new("Detail", "Spinner + 主标签 + 次级上下文", "资源或后端初始化")]
        : id == "XYUI-4-4.15" ? [new("Compact", "14 × 14", "Area C"), new("Standard / Large", "18 / 24 DIP", "通用任务反馈")] : [new("Clean Linear", "基础连续进度", "文件 / 计算"), new("Labeled", "标签 + 百分比", "长任务"), new("Segmented Stage", "阶段路径", "导入流程"), new("Inline Compact", "紧凑行内", "列表 / Inspector")];
    static XYUIDocState[] States(string id) => XYUI4StateDocumentation.States(id) is { Length: > 0 } states ? states : id == "XYUI-4-4.01" ? [new("Hovered", "只表达当前指针命中"), new("Selected + Hover", "Selected 保持主体视觉，Hover 仅提供轻度二级变化"), new("Disabled", "不产生 Hover 反馈")] : id == "XYUI-4-4.14"
        ? [new("Active", "任务持续时显示活动图形"), new("Failure", "任务失败后退出 Loading，转入错误反馈")]
        : id == "XYUI-4-4.15" ? [new("Active", "800–1200ms 旋转 Open Arc"), new("Reduced Motion", "静态弧，不启动计时器")] : [new("0 / 50 / 100%", "边界值正确显示"), new("Indeterminate", "仅在真实进度未知时使用")];

    static IReadOnlyList<XYUIDocProperty> Properties(string id) => XYUI4StateDocumentation.Properties(id) is { Length: > 0 } properties ? properties : id == "XYUI-4-4.11" ? [P("SourceRect / PreviewRect", "Rect", "显式几何"), P("IsDragging", "bool", "false"), P("SourceOpacity / PreviewOpacity", "double", "0.55 / 0.90"), P("PreviewText", "string?", "空")] : id == "XYUI-4-4.01" ? [P("Variant", "XyuiHoverStateVariant", "Surface"), P("IsHovered", "bool", "false"), P("IsSelected", "bool", "false"), P("Child", "Control", "可交互内容")] : id == "XYUI-4-4.14"
        ? [P("Text", "string", "正在加载…"), P("SecondaryText", "string", "空"), P("Size", "XyuiSpinnerSize", "Standard"), P("Variant", "XyuiLoadingIndicatorVariant", "Inline"), P("IsActive", "bool", "true")]
        : id == "XYUI-4-4.15" ? [P("Size", "XyuiSpinnerSize", "Standard"), P("IsActive", "bool", "true"), P("IsReducedMotion", "bool", "false"), P("Track / Arc", "IBrush?", "Theme Token")] : [P("Minimum / Maximum / Value", "double", "0 / 100 / 0"), P("IsIndeterminate", "bool", "false"), P("ShowPercentage", "bool", "true"), P("StatusText", "string?", "空"), P("Variant / Size", "enum", "CleanLinear / Standard")];
    static IReadOnlyList<XYUIDocToken> Tokens(string id) => XYUI4StateDocumentation.Tokens(id) is { Length: > 0 } tokens ? tokens : id == "XYUI-4-4.11" ? [T("Source", "XY.State.Color.Dragging / XY.Opacity.DragGhost"), T("Preview", "XY.State.Color.Selected / XY.Editor.Selection"), T("Shadow", "XY.Shadow.DragPreview"), T("LayoutShift", "XY.State.ResizeOnChange（禁止）")] : id == "XYUI-4-4.01" ? [T("Surface / Border", "XY.State.Color.Hover"), T("Outline", "XY.Color.Accent"), T("Border Width", "XY.Border.Width.Default"), T("Transition / Shadow", "XY.Motion.Fast / XY.Shadow.None"), T("LayoutShift", "XY.State.ResizeOnChange（禁止）")] : id == "XYUI-4-4.14"
        ? [T("Indicator", "XY.Accent.Soft / XY.Color.Accent"), T("Text", "XY.Text.Secondary"), T("Background", "Transparent"), T("LayoutShift", "Forbidden")]
        : id == "XYUI-4-4.15" ? [T("Track", "XY.Accent.Soft"), T("Arc", "XY.Color.Accent"), T("Stroke", "2–3 DIP"), T("Center / Shadow", "Transparent / None")] : [T("Track / Fill", "XY.Accent.Soft / XY.Color.Accent"), T("Height", "Compact 3–5 / Standard 6–8 / Emphasis 8–10 DIP"), T("Radius / Shadow", "XY.Radius.Full / XY.Shadow.None")];

    static string QuickStart(string id) => id switch { "XYUI-4-4.02" => "<ListBoxItem Classes=\"xyui-interactive xyui-selectable\" IsSelected=\"True\" />", "XYUI-4-4.03" => "<ToggleButton Classes=\"xyui-active\" IsChecked=\"True\" />", "XYUI-4-4.04" => "<Button Classes=\"xyui-interactive xyui-focusable\" Content=\"对象\" />", _ => id == "XYUI-4-4.01" ? "<c:XYHoverState Variant=\"Surface\"><c:XYText Text=\"对象\" /></c:XYHoverState>" : id == "XYUI-4-4.11" ? "<c:XYDragFeedback IsDragging=\"True\" />" : id == "XYUI-4-4.14" ? "<c:XYLoadingIndicator Text=\"正在初始化渲染视口…\" Size=\"Compact\" />" : id == "XYUI-4-4.15" ? "<c:XYSpinner Size=\"Standard\" />" : "<c:XYProgressBar Value=\"68\" StatusText=\"正在导入 DEM\" />" };
    static IReadOnlyList<XYUIDocRule> Rules(string id) => XYUI4StateDocumentation.Rules(id) ?? id switch
    {
        "XYUI-4-4.01" => [new("语义边界", "Hover 不表示 Selected、Active 或 Focus。"), new("布局稳定", "Hover 不改变尺寸、不推动布局，不使用大面积高饱和 Fill。"), new("状态优先级", "Disabled 优先；Selected 保持主体视觉。"), new("Variant 分工", "Surface / Border / Outline / Handle 按对象上下文选择。")],
        "XYUI-4-4.11" => [new("职责边界", "DragFeedback 只管理临时视觉，不决定最终 Drop 位置。"), new("布局稳定", "Source 保留占位，Preview 使用 overlay，不改变宿主 DesiredSize。"), new("生命周期", "Pointer Up、Esc、Cancel 或 Capture Lost 后必须清理反馈。"), new("来源可见", "Source 不得完全消失或看起来像 Disabled。")],
        "XYUI-4-4.14" => [new("任务上下文", "LoadingIndicator 不显示孤立 Spinner，必须保留任务标签或上下文。"), new("真实状态", "禁止伪造进度；失败后退出 Loading。")],
        "XYUI-4-4.15" => [new("Open Arc", "完整低对比轨道加一段 Accent 弧，圆帽、透明中心。"), new("动效生命周期", "不可见或 Reduced Motion 时停止计时器。")],
        _ => [new("真实进度", "Value 必须来自可信任务进度，禁止用动画伪造确定进度。"), new("轻量更新", "Value 更新只重绘进度条，不触发无关页面重排。")]
    };
    static IReadOnlyList<XYUIDocFoundationItem> Foundations(string id) => [new("颜色", id switch { "XYUI-4-4.02" => "XY.State.Color.Selected / XY.Border.Color.Selected", "XYUI-4-4.03" => "XY.State.Color.Active / XY.State.Color.Pressed", "XYUI-4-4.04" => "XY.Focus.Control.OutlineColor / XY.Border.Color.Focus", "XYUI-4-4.01" => "XY.State.Color.Hover / XY.Color.Accent", "XYUI-4-4.15" => "XY.Accent.Soft / XY.Color.Accent", _ => "XY.Accent.Soft / XY.Color.Accent" }, "由主题动态资源提供")];
    static IReadOnlyList<XYUIDocGuideItem> Guides(string id) => [new("Gallery", id switch { "XYUI-4-4.02" => "观察 Selected 在 Hover 离开后保持，并与 Disabled 区分。", "XYUI-4-4.03" => "切换 Persistent Active 与 Pressed，确认生命周期不残留。", "XYUI-4-4.04" => "使用键盘导航观察独立 Focus Ring。", "XYUI-4-4.01" => "在 Gallery 中分别观察四种 Hover Variant 与 Disabled / Selected 组合。", "XYUI-4-4.14" => "放在 Vulkan Viewport 左下角、Scale Indicator 上方。", "XYUI-4-4.15" => "作为 4.14 LoadingIndicator 的内部基础活动图形。", _ => "长任务使用 Labeled，阶段任务使用 Segmented，列表周边使用 Inline Compact。" })];
    static XYUIDocProperty P(string n, string t, string d) => new(n, t, d, "组件属性");
    static XYUIDocToken T(string n, string v) => new(n, v, "来自 XYUI Foundation 的语义 Token");
}
