namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    static string Phase2DQuickStart(string id) => id switch
    {
        "XYUI-2-19" => "<xy:XYColorPicker Color=\"{Binding PrimaryColor}\" Mode=\"RGBA\" />",
        "XYUI-2-20" => "<xy:XYBoolProperty Label=\"Visible\" Value=\"{Binding IsVisible}\" />",
        "XYUI-2-21" => "<xy:XYNumberProperty Label=\"Mass\" Value=\"{Binding Mass}\" Suffix=\"kg\" />",
        "XYUI-2-22" => "<xy:XYVectorProperty Label=\"Position\" Dimension=\"Vector3\" X=\"{Binding PosX}\" Y=\"{Binding PosY}\" Z=\"{Binding PosZ}\" />",
        "XYUI-2-23" => "<xy:XYEnumProperty Label=\"Blend Mode\" ItemsSource=\"{Binding BlendModes}\" SelectedItem=\"{Binding CurrentBlendMode}\" />",
        "XYUI-2-24" => "<xy:XYReferenceProperty Label=\"Material\" Reference=\"{Binding MaterialRef}\" ExpectedType=\"Material\" />",
        _ => ""
    };

    static IReadOnlyList<XYUIDocRule> Phase2DCoreRules(string id) => id switch
    {
        "XYUI-2-19" => [
            new("组件定义", "颜色拾取控件，提供 RGB/RGBA 双模式、色相环与明度/饱和度二维色域调节。"),
            new("弹出与生命周期", "点击触发浮层展示；支持 Esc 关闭、失焦关闭与外部点击安全关闭。"),
            new("格式校验", "HEX 支持 6 位与 8 位十六进制输入，非法输入予以高亮提示并保留原值。")
        ],
        "XYUI-2-20" => [
            new("组件定义", "属性面板专用的二元布尔控制行，内部严格复用 XYSwitch 真实控件。"),
            new("行级点击", "点击属性行文本或开关本体均能正确切换状态，且保证单次命中只切换一次。"),
            new("状态保护", "只读与禁用状态下阻断空格键与鼠标点击交互，保留既有真值呈现。")
        ],
        "XYUI-2-21" => [
            new("组件定义", "单行数值属性控制行，内部严格复用 XYNumberField 真实输入框。"),
            new("标签微调", "支持在属性标签上按住水平拖动（Scrubbing，每 4 DIP 一步长）快速调整数值。"),
            new("统一真值", "Value、Min/Max、Step 等约束与内部 NumberField 双向保持严格同步。")
        ],
        "XYUI-2-22" => [
            new("组件定义", "多维向量属性控制行（支持 Vector2/3/4），各轴严格复用 XYNumberField。"),
            new("独立编辑", "每一轴拥有独立输入与微调能力，修改某一轴不影响其他轴的既有数值。"),
            new("响应式自适应", "宽屏横向平铺，中屏与紧凑模式自适应折行与纵向堆叠，防止轴被压扁。")
        ],
        "XYUI-2-23" => [
            new("组件定义", "枚举类型离散选项控制行，内部严格复用 XYSelect 真实选择控件。"),
            new("原生交互", "继承 XYSelect 完整交互规范（Enter/Space 展开、上下键导航、Esc 关闭）。"),
            new("真实数据源", "选项集合通过 ItemsSource 驱动绑定，支持只读模式与选项变化事件派发。")
        ],
        "XYUI-2-24" => [
            new("组件定义", "资源与实体引用属性行，内置定位、浏览与清除三组动作按钮，支持弹出选择浮层。"),
            new("引用状态机", "严格区分 Empty、Resolved、Missing 与 TypeMismatch 四种语义状态。"),
            new("拖拽与指派", "支持从外部拖入目标资源对象，类型不匹配时阻断指派并标记类型警告。")
        ],
        _ => []
    };
}
