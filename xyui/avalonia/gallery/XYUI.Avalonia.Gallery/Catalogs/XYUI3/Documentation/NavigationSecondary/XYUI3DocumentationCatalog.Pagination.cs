namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildPaginationDoc(string id, string type) => new(
        id, "分页导航", "Pagination",
        "邻近页快速跳转与紧凑数据页脚组合的分页导航，高度 34 DIP，支持前后翻页、输入跳页与每页容量切换。",
        "用于资源检索、日志历史、实体记录等大量同构项的高密浏览与精准定位；边界时自动禁用对应翻页方向。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYPagination CurrentPage=\"1\" TotalPages=\"20\" TotalItems=\"400\" ShowTotalItems=\"True\" />", "<c:XYPaginationFooter />"],
        [new("Compact Neighbor", "34 DIP 高度，居中当前页与前后相邻页，末端直达输入框", "Desktop Standard"), new("Footer Bar", "XYPaginationFooter 聚合每页容量选择与数据条数总计", "Data Table Bottom")],
        [new("Current", "深色 Selected 表面与 Accent 高亮页码"), new("Neighbor", "邻近页可直接点击"), new("Disabled", "首页时禁用 Prev，末页时禁用 Next"), new("Jump Input", "回车直达合法页码")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "分页栏容器底色"), new("XY.Brush.Accent.Default", "Accent", "当前活动页选中高亮"), new("XY.Border.Color.Subtle", "Subtle", "分隔线与输入框边框")],
        type)
    {
        CanonicalIdentity = "3.13 · Pagination / 分页导航",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<StackPanel Spacing="16">
    <!-- 基础紧凑分页 -->
    <c:XYPagination CurrentPage="3" TotalPages="20" TotalItems="400" ShowTotalItems="True" />
    <!-- 表格底部标准聚合页脚 (含每页行数下拉) -->
    <c:XYPaginationFooter />
</StackPanel>
""",
        CoreRules =
        [
            new("34 DIP 等高对齐规范", "翻页按钮、邻近页按键、分隔线与跳页输入框必须严格保持 34 DIP 垂直同轴对齐。"),
            new("动态邻近页开窗机制", "仅展示当前页及其前后相邻各一页（Current-1, Current, Current+1），搭配跳页输入保障高密度无拥挤。"),
            new("首尾翻页严格边界保护", "当前为第 1 页时必须禁用 Previous 箭头，当前为 TotalPages 时必须禁用 Next 箭头，禁止越界。"),
            new("数据页脚标准封装", "表格/视口底栏优先消费 XYPaginationFooter，统一包含总数统计、每页条数选择 (25/50/100) 与翻页。")
        ],
        DoDonts =
        [
            new("边界禁用", "DO: 第一页禁用上一页，最后一页禁用下一页。", "DON'T: 允许小于 1 或超过 TotalPages 的无效翻页请求。", "必须在视觉和事件两层杜绝越界翻页。"),
            new("对齐控制", "DO: 所有子控件严格统一 34 DIP 高度。", "DON'T: 跳页输入框独立放大或缩小导致上下锯齿。", "编辑器密集排版中高度不一会带来严重杂乱感。"),
            new("职责划分", "DO: 表格底栏统一消费 XYPaginationFooter。", "DON'T: 业务层重复写 TextBlock+Select 拼凑底栏。", "统一页脚有助于在全局多模块保持一致体验。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
