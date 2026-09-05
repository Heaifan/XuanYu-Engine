namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static IReadOnlyList<XYUIDocGuideItem> Phase2DHowToUse(string id) => id switch
    {
        "XYUI-2-19" => [
            new("适用场景 (Use when)", "用于颜色资产、环境光照、材质底色等需要可视化选色与数值调节的场景。"),
            new("透明度控制 (Alpha)", "当需要半透明通道时使用 Mode=\"RGBA\"；纯不透明颜色使用 Mode=\"RGB\"。"),
            new("面板交互 (Interaction)", "点击主按钮展开面板，支持色域拾取、色相/Alpha 滑块调节与 HEX 直接键入。")
        ],
        "XYUI-2-20" => [
            new("适用场景 (Use when)", "检查器（Inspector）中二元配置项控制，如「显示网格」、「启用阴影」、「开启碰撞」。"),
            new("行级触达 (Row Tap)", "用户既可点击右侧真实开关，也可直接点击整行标签快速切换。"),
            new("只读呈现 (ReadOnly)", "仅供观察的开关状态设置 IsReadOnly=\"True\"，保留当前视觉但阻断点击修改。")
        ],
        "XYUI-2-21" => [
            new("适用场景 (Use when)", "检查器中单行连续数值属性编辑，如「质量」、「速度」、「透明度」、「阻尼系数」。"),
            new("标签微调 (Label Scrub)", "用户按住属性名称左右拖动可快速无极微调，单击输入框则进入精细键盘编辑。"),
            new("范围与单位 (Constraints)", "务必配置合理 Minimum/Maximum 与 Step；单位后缀通过 Suffix 显露。")
        ],
        "XYUI-2-22" => [
            new("适用场景 (Use when)", "空间坐标与变换属性配置，如「位置 (Vector3)」、「缩放 (Vector3)」、「UV (Vector2)」。"),
            new("响应式折行 (Responsive)", "在不同面板宽度下自动切换单行横排、标签置顶或逐轴纵向堆叠。"),
            new("各轴独立 (Per-Axis)", "每一轴均为独立 XYNumberField，支持独立键盘编辑与局部微调。")
        ],
        "XYUI-2-23" => [
            new("适用场景 (Use when)", "离散枚举项配置，如「渲染模式 (Solid/Wireframe)」、「混合模式」、「材质品质」。"),
            new("控件复用 (Reuse)", "内部严格复用 XYSelect，享受原生下拉列表、键盘流转与安全失焦生命周期。"),
            new("数据驱动 (Binding)", "通过 ItemsSource 绑定枚举选项集合，通过 SelectedItem 双向同步当前值。")
        ],
        "XYUI-2-24" => [
            new("适用场景 (Use when)", "实体、资源与材质间依赖关系引用，如「Mesh 引用」、「材质槽位」、「父级节点」。"),
            new("状态识别 (States)", "明确区分正常 Resolved、未设置 Empty、资源丢失 Missing 与类型不匹配 TypeMismatch。"),
            new("快捷动作 (Actions)", "内置定位 Locate（聚焦对应资源）、浏览 Browse（弹出选择器）与清除 Clear（置空）。")
        ],
        _ => []
    };
}
