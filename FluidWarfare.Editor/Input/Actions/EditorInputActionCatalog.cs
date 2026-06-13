using FluidWarfare.Editor.Input.Bindings;

namespace FluidWarfare.Editor.Input.Actions;

/// <summary>
/// 全部编辑器动作声明与 Blender 默认绑定。
/// </summary>
public static class EditorInputActionCatalog
{
    /// <summary>所有动作定义。</summary>
    public static IReadOnlyList<EditorInputActionDefinition> All { get; } = BuildAll();

    /// <summary>Blender 默认绑定集。</summary>
    public static EditorInputBindingSet BlenderPreset { get; } = BuildBlenderPreset();

    /// <summary>Blender 默认绑定的平铺列表（每个动作的 PrimaryGesture 和 SecondaryGesture）。</summary>
    public static IReadOnlyList<EditorInputBinding> BlenderDefaultBindings { get; } = BuildBlenderDefaultBindings();

    /// <summary>按 ID 查找动作。</summary>
    public static EditorInputActionDefinition? FindById(string id) =>
        All.FirstOrDefault(a => a.Id == id);

    /// <summary>按上下文分组。</summary>
    public static IReadOnlyList<EditorInputActionDefinition> GetByContext(EditorInputActionContext context) =>
        All.Where(a => a.Context == context).ToList();

    private static List<EditorInputActionDefinition> BuildAll()
    {
        var list = new List<EditorInputActionDefinition>();

        // ─── 全局 ────────────────────────────────────────────
        list.Add(new("editor.open_preferences", "打开偏好设置", "全局",
            EditorInputActionContext.Global, EditorInputValueKind.Trigger));
        list.Add(new("tool.cancel_current", "取消当前工具", "全局",
            EditorInputActionContext.Global, EditorInputValueKind.Trigger));

        // ─── 视口导航 ───────────────────────────────────────
        list.Add(new("viewport.orbit", "视口环绕旋转", "视口导航",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.PointerDelta));
        list.Add(new("viewport.pan", "视口平移", "视口导航",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.PointerDelta));
        list.Add(new("viewport.dolly", "视口推拉", "视口导航",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.PointerDelta));
        list.Add(new("viewport.zoom", "视口缩放", "视口导航",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.WheelDelta));
        list.Add(new("viewport.frame_all", "查看全部", "视口导航",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger));
        list.Add(new("viewport.frame_selected", "聚焦所选", "视口导航",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger));
        list.Add(new("viewport.toggle_projection", "切换透视/正交", "视口导航",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger));

        // ─── 标准视图 ───────────────────────────────────────
        list.Add(new("viewport.view_front", "前视图", "标准视图",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger));
        list.Add(new("viewport.view_back", "后视图", "标准视图",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger));
        list.Add(new("viewport.view_right", "右视图", "标准视图",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger));
        list.Add(new("viewport.view_left", "左视图", "标准视图",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger));
        list.Add(new("viewport.view_top", "顶视图", "标准视图",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger));
        list.Add(new("viewport.view_bottom", "底视图", "标准视图",
            EditorInputActionContext.Viewport3D, EditorInputValueKind.Trigger));

        // ─── 工具 ──────────────────────────────────────────
        list.Add(new("tool.select", "选择工具", "工具",
            EditorInputActionContext.Global, EditorInputValueKind.Trigger));
        list.Add(new("tool.move", "移动工具", "工具",
            EditorInputActionContext.Global, EditorInputValueKind.Trigger));

        // ─── Transform ──────────────────────────────────────
        list.Add(new("transform.apply", "应用 Transform", "Transform",
            EditorInputActionContext.InspectorTransform, EditorInputValueKind.Trigger));
        list.Add(new("transform.reset_draft", "重置 Transform 草稿", "Transform",
            EditorInputActionContext.InspectorTransform, EditorInputValueKind.Trigger));
        list.Add(new("ground_placement.begin", "地面放置", "Transform",
            EditorInputActionContext.ActiveEditorTool, EditorInputValueKind.Trigger));

        return list;
    }

    private static List<EditorInputBinding> BuildBlenderDefaultBindings()
    {
        var list = new List<EditorInputBinding>();

        // 全局
        list.Add(new() { ActionId = "editor.open_preferences",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Comma", EditorInputModifiers.Control) });
        list.Add(new() { ActionId = "tool.cancel_current",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Escape") });

        // 视口导航
        list.Add(new() { ActionId = "viewport.orbit",
            PrimaryGesture = new(EditorInputDevice.Mouse, "Middle", kind: EditorInputGestureKind.MouseDrag) });
        list.Add(new() { ActionId = "viewport.pan",
            PrimaryGesture = new(EditorInputDevice.Mouse, "Middle", EditorInputModifiers.Shift, EditorInputGestureKind.MouseDrag) });
        list.Add(new() { ActionId = "viewport.dolly",
            PrimaryGesture = new(EditorInputDevice.Mouse, "Middle", EditorInputModifiers.Control, EditorInputGestureKind.MouseDrag) });
        list.Add(new() { ActionId = "viewport.zoom",
            PrimaryGesture = new(EditorInputDevice.Wheel, "Y", kind: EditorInputGestureKind.MouseWheel) });
        list.Add(new() { ActionId = "viewport.frame_all",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Home") });
        list.Add(new() { ActionId = "viewport.frame_selected",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Decimal") });
        list.Add(new() { ActionId = "viewport.toggle_projection",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad5") });

        // 标准视图
        list.Add(new() { ActionId = "viewport.view_front",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad1") });
        list.Add(new() { ActionId = "viewport.view_back",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad1", EditorInputModifiers.Control) });
        list.Add(new() { ActionId = "viewport.view_right",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad3") });
        list.Add(new() { ActionId = "viewport.view_left",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad3", EditorInputModifiers.Control) });
        list.Add(new() { ActionId = "viewport.view_top",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad7") });
        list.Add(new() { ActionId = "viewport.view_bottom",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Numpad7", EditorInputModifiers.Control) });

        // 工具
        list.Add(new() { ActionId = "tool.select",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "S") });
        list.Add(new() { ActionId = "tool.move",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "G") });

        // Transform
        list.Add(new() { ActionId = "transform.apply",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Enter") });
        list.Add(new() { ActionId = "transform.reset_draft",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Escape") });

        return list;
    }

    private static EditorInputBindingSet BuildBlenderPreset()
    {
        // 地面放置（无默认快捷键）
        return new EditorInputBindingSet
        {
            Preset = "blender",
            Overrides = Array.Empty<EditorInputBindingOverride>()
        };
    }
}
