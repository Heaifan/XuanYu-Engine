namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3DocumentationCatalog
{
    static XYUI1ComponentDocument BuildStepsDoc(string id, string type) => new(
        id, "步骤导航", "Steps",
        "用完成、当前、待执行、警告与错误状态表达多阶段任务进度的向导导航，支持横向与纵向布局及自适应排布。",
        "用于项目创建向导、资源导入流程、多阶段任务配置等不可跳序或递进流程；与空间层级回溯的面包屑明确分离。",
        () => XYUI3GalleryCatalog.CreatePreview(id),
        ["<c:XYSteps Orientation=\"Horizontal\"><c:XYStepNode Label=\"项目初始化\" State=\"Completed\" /></c:XYSteps>"],
        [new("Horizontal Flow", "横向多阶段向导，顶层圆点标记与底层标签居中同轴", "Standard Wizard"), new("Vertical Steps", "纵向步骤排布，左侧导线连通，右侧文字对齐", "Inspector / Setup Flow")],
        [new("Completed", "蓝色圆底白色对勾矢量标记"), new("Current", "深色圆圈内嵌活动 Accent 核心圆点"), new("Pending", "灰色空心圆形边框"), new("Warning / Error", "黄色警示或红色错误矢量状态"), new("Disabled", "CanNavigate=false 阻断非法阶段点击跳转")],
        Properties(id),
        [new("XY.Surface.Panel", "Panel", "步骤容器底色"), new("XY.Brush.Accent.Default", "Accent", "当前活动步骤与已连通导线"), new("XY.Border.Color.Subtle", "Subtle", "未完成阶段淡灰导线")],
        type)
    {
        CanonicalIdentity = "3.14 · Steps / 步骤导航",
        Category = "XYUI-3 · 导航与切换",
        Acceptance = "UI + INTERACTION IMPLEMENTED · AWAITING USER VISUAL ACCEPTANCE · AWAITING USER INTERACTION ACCEPTANCE",
        QuickStartXaml = """
<c:XYSteps Orientation="Horizontal">
    <c:XYStepNode Label="项目初始化" State="Completed" />
    <c:XYStepNode Label="地图设置" State="Completed" />
    <c:XYStepNode Label="数据配置" State="Current" />
    <c:XYStepNode Label="完整性校验" State="Warning" />
    <c:XYStepNode Label="打包发布" State="Pending" CanNavigate="False" />
</c:XYSteps>
""",
        CoreRules =
        [
            new("线性流程语义边界", "Steps 专职表达连续任务与向导流程，严禁与空间树层级回溯的 Breadcrumb 混淆使用。"),
            new("五态完整视觉映射", "精准表达 Completed (对勾)、Current (中心圆点)、Pending (浅边框)、Warning (警示) 与 Error (错误)。"),
            new("双向同轴连通导线", "Horizontal 与 Vertical 双向均由导线精确贯穿节点圆心，已完成阶段连线高亮，待执行阶段弱化。"),
            new("导航权限阻断支持", "节点支持 CanNavigate 独立控制，未就绪或受阻阶段阻止用户随意跨步跳转。")
        ],
        DoDonts =
        [
            new("组件认知", "DO: 将「第一步→第二步→第三步」做成 Steps。", "DON'T: 使用面包屑 Breadcrumb 表达不可逆的向导流程。", "向导流程需要明确的状态反馈与阶段阻断。"),
            new("导线连贯性", "DO: 导线根据前驱节点状态无缝着色连通。", "DON'T: 导线断裂或与文字产生层叠遮挡。", "导线断层会破坏连续向导的心理预期。"),
            new("布局选择", "DO: 宽视口用 Horizontal，侧边栏或多步列表用 Vertical。", "DON'T: 在窄视口强行横排导致文字拥挤重叠。", "自适应布局能保证各种窗口尺寸下的可读性。")
        ],
        LiveExamplesFactory = () => XYUI3LiveExamplesFactory.CreateLiveExamples(id)!,
        CompositionFactory = () => XYUI3LiveExamplesFactory.CreateComposition(id)!
    };
}
