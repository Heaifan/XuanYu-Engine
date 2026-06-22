using FluidWarfare.Editor.Input.Bindings;

namespace FluidWarfare.Editor.Input.Actions;

/// <summary>全部编辑器动作声明与 Blender 默认绑定。</summary>
public static class EditorInputActionCatalog
{
    public static IReadOnlyList<EditorInputActionDefinition> All { get; } = BuildAll();
    public static EditorInputBindingSet BlenderPreset { get; } = BuildBlenderPreset();
    public static IReadOnlyList<EditorInputBinding> BlenderDefaultBindings { get; } = BuildBlenderDefaultBindings();
    public static EditorInputActionDefinition? FindById(string id) => All.FirstOrDefault(a => a.Id == id);
    public static IReadOnlyList<EditorInputActionDefinition> GetByContext(EditorInputActionContext c) => All.Where(a => a.Context == c).ToList();

    static List<EditorInputActionDefinition> BuildAll() => [
        new("editor.open_preferences", "打开偏好设置", "全局", EditorInputActionContext.Global, EditorInputValueKind.Trigger),
        new("tool.cancel_current", "取消当前工具", "全局", EditorInputActionContext.Global, EditorInputValueKind.Trigger),
        new("viewport.orbit", "视口环绕旋转", "视口导航", EditorInputActionContext.Viewport3D, EditorInputValueKind.PointerDelta),
        new("viewport.pan", "视口平移", "视口导航", EditorInputActionContext.Viewport3D, EditorInputValueKind.PointerDelta),
        new("viewport.dolly", "视口推拉", "视口导航", EditorInputActionContext.Viewport3D, EditorInputValueKind.PointerDelta),
        new("viewport.zoom", "视口缩放", "视口导航", EditorInputActionContext.Viewport3D, EditorInputValueKind.WheelDelta),
        new("viewport.frame_all", "查看全部", "视口导航", EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger),
        new("viewport.frame_selected", "聚焦所选", "视口导航", EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger),
        new("viewport.toggle_projection", "切换透视/正交", "视口导航", EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger),
        new("viewport.view_front", "前视图", "标准视图", EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger),
        new("viewport.view_back", "后视图", "标准视图", EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger),
        new("viewport.view_right", "右视图", "标准视图", EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger),
        new("viewport.view_left", "左视图", "标准视图", EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger),
        new("viewport.view_top", "顶视图", "标准视图", EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger),
        new("viewport.view_bottom", "底视图", "标准视图", EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger),
        new("tool.select", "选择工具", "工具", EditorInputActionContext.Global, EditorInputValueKind.Trigger),
        new("tool.move", "移动工具", "工具", EditorInputActionContext.Global, EditorInputValueKind.Trigger),
        new("transform.apply", "应用 Transform", "Transform", EditorInputActionContext.InspectorTransform, EditorInputValueKind.Trigger),
        new("transform.reset_draft", "重置 Transform 草稿", "Transform", EditorInputActionContext.InspectorTransform, EditorInputValueKind.Trigger),
        new("ground_placement.begin", "地面放置", "Transform", EditorInputActionContext.ActiveEditorTool, EditorInputValueKind.Trigger),
    ];

    static List<EditorInputBinding> BuildBlenderDefaultBindings() => [
        new() { ActionId = "editor.open_preferences", PrimaryGesture = new(EditorInputDevice.Keyboard, "Comma", EditorInputModifiers.Control) },
        new() { ActionId = "tool.cancel_current", PrimaryGesture = new(EditorInputDevice.Keyboard, "Escape") },
        new() { ActionId = "viewport.orbit", PrimaryGesture = new(EditorInputDevice.Mouse, "Middle", kind: EditorInputGestureKind.MouseDrag) },
        new() { ActionId = "viewport.pan", PrimaryGesture = new(EditorInputDevice.Mouse, "Middle", EditorInputModifiers.Shift, EditorInputGestureKind.MouseDrag) },
        new() { ActionId = "viewport.dolly", PrimaryGesture = new(EditorInputDevice.Mouse, "Middle", EditorInputModifiers.Control, EditorInputGestureKind.MouseDrag) },
        new() { ActionId = "viewport.zoom", PrimaryGesture = new(EditorInputDevice.Wheel, "Y", kind: EditorInputGestureKind.MouseWheel) },
        new() { ActionId = "viewport.frame_all", PrimaryGesture = new(EditorInputDevice.Keyboard, "Home") },
        new() { ActionId = "viewport.frame_selected", PrimaryGesture = new(EditorInputDevice.Keyboard, "Decimal") },
        new() { ActionId = "viewport.toggle_projection", PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad5") },
        new() { ActionId = "viewport.view_front", PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad1") },
        new() { ActionId = "viewport.view_back", PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad1", EditorInputModifiers.Control) },
        new() { ActionId = "viewport.view_right", PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad3") },
        new() { ActionId = "viewport.view_left", PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad3", EditorInputModifiers.Control) },
        new() { ActionId = "viewport.view_top", PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad7") },
        new() { ActionId = "viewport.view_bottom", PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad7", EditorInputModifiers.Control) },
        new() { ActionId = "tool.select", PrimaryGesture = new(EditorInputDevice.Keyboard, "S") },
        new() { ActionId = "tool.move", PrimaryGesture = new(EditorInputDevice.Keyboard, "G") },
        new() { ActionId = "transform.apply", PrimaryGesture = new(EditorInputDevice.Keyboard, "Enter") },
        new() { ActionId = "transform.reset_draft", PrimaryGesture = new(EditorInputDevice.Keyboard, "Escape") },
    ];

    static EditorInputBindingSet BuildBlenderPreset() => new() { Preset = "blender", Overrides = [] };
}
