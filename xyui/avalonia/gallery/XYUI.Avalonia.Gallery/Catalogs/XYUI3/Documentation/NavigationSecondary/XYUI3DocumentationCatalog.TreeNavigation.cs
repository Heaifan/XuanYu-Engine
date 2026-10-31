namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildTreeNavigationDoc(string id, string type) => new(
        id, "树状导航", "TreeNavigation",
        "弱默认引导线与强化祖先链的高密度紧凑树导航，行高 28 DIP，缩进 16 DIP，支持节点展开折叠与精确定位。",
        "用于工程资产、地图要素、Hierarchy 节点等层级数据的快速浏览与单选导航；支持鼠标与键盘箭头展开/折叠。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYTreeNavigation><c:XYTreeNode Label=\"地图系统\"><c:XYTreeNode.Children><c:XYTreeNode Label=\"要素\" /></c:XYTreeNode.Children></c:XYTreeNode></c:XYTreeNavigation>"],
        [new("Compact Guided Tree", "28 DIP 行高，16 DIP 缩进，包含引导线与折叠箭头", "Desktop Dense Tree")],
        [new("Selected", "左侧 3 DIP Accent Bar + Selected Surface 高亮"), new("Expanded", "旋转 Chevron 下展并展现下级子项"), new("Focused", "键盘操作无障碍外框指示"), new("Disabled", "弱化显示，禁止鼠标选择与键盘导航"), new("Badge", "消费真实 Badge 呈现计数 (如资源 12、警告 3)"), new("Status", "消费真实 StatusState 呈现状态色 (如 Warning、Info)")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "树容器底色"), new("XY.Brush.Accent.Default", "Accent", "选中指示条与高亮引导线"), new("XY.Border.Color.Subtle", "Subtle", "弱化默认引导线")],
        type)
    {
        CanonicalIdentity = "3.12 · TreeNavigation / 树状导航",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<c:XYTreeNavigation>
    <c:XYTreeNode Label="地图系统" Icon="Locate" IsExpanded="True">
        <c:XYTreeNode.Children>
            <c:XYTreeNode Label="基础要素" IsSelected="True" />
            <c:XYTreeNode Label="环境配置" Icon="Eye" Badge="警告" Status="Warning" IsExpanded="True">
                <c:XYTreeNode.Children>
                    <c:XYTreeNode Label="地形高度图" />
                    <c:XYTreeNode Label="气候与光照" IsEnabled="False" />
                </c:XYTreeNode.Children>
            </c:XYTreeNode>
        </c:XYTreeNode.Children>
    </c:XYTreeNode>
    <c:XYTreeNode Label="数据集合" Icon="Code" Badge="12" Status="Info" />
</c:XYTreeNavigation>
""",
        CoreRules =
        [
            new("Children 唯一事实源", "树层级 100% 由 Children 嵌套结构表达，Depth、Parent 与 HasChildren 均由 Runtime 自动计算，严禁手写同步。"),
            new("高密度与低装饰", "行高严格统一为 28 DIP，引导线细密浅淡，不做大面积粗糙卡片或文件树过度装饰。"),
            new("祖先导线高亮机制", "当前选中项向上高亮直系父级导线，便于在深层树中快速辨识归属。"),
            new("完整状态支持", "全面支持 Selected、Expanded、Disabled、Badge 计数与 StatusState 状态徽标。")
        ],
        DoDonts =
        [
            new("层级构建模式", "DO: 通过 Children 构建层级结构，无需手工写 Depth 与 HasChildren。", "DON'T: 同时手写 Parent / Depth / HasChildren 来模拟树。", "由 Runtime 自动推导可避免层级不一致与折叠错乱。"),
            new("信息密度控制", "DO: 严格使用 28 DIP 紧凑行高，保持桌面引擎级高信息承载量。", "DON'T: 随意放大行高至 40+ DIP 将其按钮化。", "稀疏树结构会极大地削弱深层级结构的纵览效率。"),
            new("状态表达", "DO: 消费 Runtime 原生 Badge 与 Status 属性。", "DON'T: 在模板中手工画独立外部徽标。", "原生徽标与树节点具有严格的对齐与选中适配逻辑。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
