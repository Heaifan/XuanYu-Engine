using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Bindings;
using FluidWarfare.Editor.Input.Runtime;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.Input;

/// <summary>
/// Win32 原始输入→EditorInputMatch 的翻译器。
/// 接收来自 WindowsVulkanViewportHostControl 的原始事件，
/// 通过 EditorInputBindingSnapshot 解析为动作匹配。
/// 拖拽状态由 snapshot.BeginDrag/EndDrag 管理，保持 O(1) 热路径。
/// </summary>
public sealed class WindowsViewportInputTranslator
{
    private EditorInputBindingSnapshot _snapshot;

    // 鼠标位置跟踪（用于 delta 计算）
    private int _lastMouseX;
    private int _lastMouseY;

    // 修饰键状态跟踪
    private EditorInputModifiers _currentModifiers;

    public WindowsViewportInputTranslator(EditorInputBindingSnapshot initialSnapshot)
    {
        _snapshot = initialSnapshot ?? throw new ArgumentNullException(nameof(initialSnapshot));
    }

    /// <summary>
    /// Hot-reload 时由 EditorInputService 调用以更新快照引用。
    /// </summary>
    public void OnSnapshotReplaced(EditorInputBindingSnapshot newSnapshot)
    {
        _snapshot = newSnapshot ?? throw new ArgumentNullException(nameof(newSnapshot));
    }

    /// <summary>当前修饰键状态（用于外部查询）。</summary>
    public EditorInputModifiers CurrentModifiers => _currentModifiers;

    /// <summary>当前快照修订号。</summary>
    public int Revision => _snapshot.Revision;

    // ─── 原始事件处理 ──────────────────────────────────────

    /// <summary>
    /// 处理键盘键按下事件。
    /// 修饰键更新自身状态，不产生 match。
    /// 其他键通过 KeyCodeMapper 转换后查表。
    /// </summary>
    public EditorInputMatch OnRawKeyDown(int virtualKeyCode, int pointerX, int pointerY)
    {
        // 更新修饰键状态
        UpdateModifierState(virtualKeyCode, pressed: true);

        // 修饰键本身不产生 match
        if (IsModifierKey(virtualKeyCode))
            return EditorInputMatch.NoMatch;

        var code = Win32KeyCodeMapper.Map(virtualKeyCode);
        if (code is null)
            return EditorInputMatch.NoMatch;

        var sig = BuildSignature(EditorInputDevice.Keyboard, code,
            EditorInputGestureKind.KeyPress, _currentModifiers);

        var action = _snapshot.Resolve(sig);
        if (action is null)
            return EditorInputMatch.NoMatch;

        return new EditorInputMatch
        {
            ActionId = action.Id,
            Definition = action,
            ValueKind = action.ValueKind,
            DeltaX = 0,
            DeltaY = 0,
            BindingRevision = _snapshot.Revision
        };
    }

    /// <summary>
    /// 处理键盘键抬起事件（仅用于修饰键状态跟踪）。
    /// </summary>
    public void OnRawKeyUp(int virtualKeyCode)
    {
        UpdateModifierState(virtualKeyCode, pressed: false);
    }

    /// <summary>
    /// 处理鼠标按钮按下事件。
    /// 解析为 MouseDrag 手势，锁定拖拽绑定。
    /// </summary>
    public EditorInputMatch OnRawPointerButtonDown(int buttonCode, int x, int y)
    {
        _lastMouseX = x;
        _lastMouseY = y;

        var code = ButtonCodeToName(buttonCode);
        var sig = BuildSignature(EditorInputDevice.Mouse, code,
            EditorInputGestureKind.MouseDrag, _currentModifiers);

        System.Diagnostics.Debug.WriteLine(
            $"[InputTrace-Translator] OnPointerButtonDown btn={buttonCode}->\"{code}\" sig=\"{sig}\"");

        // 锁定拖拽（O(1) 后续移动不再查表）
        _snapshot.BeginDrag(sig, x, y);

        var action = _snapshot.Resolve(sig);
        if (action is null)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[InputTrace-Translator] Resolve sig=\"{sig}\" → null (no match)");
            return EditorInputMatch.NoMatch;
        }

        System.Diagnostics.Debug.WriteLine(
            $"[InputTrace-Translator] Resolve sig=\"{sig}\" → action=\"{action.Id}\"");

        return new EditorInputMatch
        {
            ActionId = action.Id,
            Definition = action,
            ValueKind = EditorInputValueKind.PointerDelta,
            DeltaX = 0,
            DeltaY = 0,
            BindingRevision = _snapshot.Revision
        };
    }

    /// <summary>
    /// 处理鼠标移动事件。
    /// 如果在拖拽中，通过 GetActiveDragDefinition 获取完整动作定义（O(1) 零字典查找）。
    /// ExecuteInputAction 要求 Match 必须携带 Definition，否则直接丢弃。
    /// </summary>
    public EditorInputMatch OnRawPointerMoved(int x, int y)
    {
        var deltaX = x - _lastMouseX;
        var deltaY = y - _lastMouseY;
        _lastMouseX = x;
        _lastMouseY = y;

        // 没有活动拖拽 → 不产生任何 match
        var definition = _snapshot.GetActiveDragDefinition();
        if (definition is null)
            return EditorInputMatch.NoMatch;

        return new EditorInputMatch
        {
            ActionId = definition.Id,
            Definition = definition,
            ValueKind = EditorInputValueKind.PointerDelta,
            DeltaX = deltaX,
            DeltaY = deltaY,
            BindingRevision = _snapshot.Revision
        };
    }

    /// <summary>
    /// 处理鼠标按钮抬起事件。结束拖拽锁定。
    /// </summary>
    public void OnRawPointerButtonUp(int buttonCode)
    {
        _snapshot.EndDrag();
    }

    /// <summary>
    /// 处理鼠标滚轮事件。仅使用 Win32 消息发生时的修饰键快照。
    /// 不使用可能因焦点切换而过期的 _currentModifiers。
    /// </summary>
    public EditorInputMatch OnRawMouseWheel(int delta, int packedModifiers, int pointerX, int pointerY)
    {
        // 从 LOWORD(wParam) 提取 MK_* 标志。
        // 这些是 WM_MOUSEWHEEL 消息被发送时的真实修饰键状态，
        // 不依赖可能因焦点丢失而卡住的 _currentModifiers。
        var mkControl = (packedModifiers & 0x0008) != 0; // MK_CONTROL
        var mkShift = (packedModifiers & 0x0004) != 0;   // MK_SHIFT

        EditorInputModifiers mods = EditorInputModifiers.None;
        if (mkControl) mods |= EditorInputModifiers.Control;
        if (mkShift) mods |= EditorInputModifiers.Shift;

        var sig = BuildSignature(EditorInputDevice.Wheel, "Y",
            EditorInputGestureKind.MouseWheel, mods);

        System.Diagnostics.Debug.WriteLine(
            $"[InputTrace-Translator] OnMouseWheel delta={delta} mods={mods} sig=\"{sig}\"");

        var action = _snapshot.Resolve(sig);
        if (action is null)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[InputTrace-Translator] Resolve wheel sig=\"{sig}\" → null");
            return EditorInputMatch.NoMatch;
        }

        System.Diagnostics.Debug.WriteLine(
            $"[InputTrace-Translator] Resolve wheel sig=\"{sig}\" → action=\"{action.Id}\"");

        return new EditorInputMatch
        {
            ActionId = action.Id,
            Definition = action,
            ValueKind = EditorInputValueKind.WheelDelta,
            WheelDelta = delta / 120.0f,
            BindingRevision = _snapshot.Revision
        };
    }

    /// <summary>
    /// 取消活动拖拽（用于上下文切换或 hot-reload）。
    /// </summary>
    public void CancelActiveDrag()
    {
        _snapshot.EndDrag();
    }

    /// <summary>
    /// 焦点丢失时重置修饰键状态并结束活动拖动。
    /// 防止 Ctrl 因焦点转移到其他窗口而永久卡在 _currentModifiers 中。
    /// </summary>
    public void OnRawInputFocusLost()
    {
        System.Diagnostics.Debug.WriteLine(
            "[InputTrace-Translator] FocusLost — clearing modifiers and ending drag");
        _currentModifiers = EditorInputModifiers.None;
        _snapshot.EndDrag();
    }

    // ─── 工具方法 ──────────────────────────────────────────

    private void UpdateModifierState(int vk, bool pressed)
    {
        if (vk == VkShift)
            _currentModifiers = pressed
                ? _currentModifiers | EditorInputModifiers.Shift
                : _currentModifiers & ~EditorInputModifiers.Shift;
        else if (vk == VkControl)
            _currentModifiers = pressed
                ? _currentModifiers | EditorInputModifiers.Control
                : _currentModifiers & ~EditorInputModifiers.Control;
        else if (vk == VkMenu)
            _currentModifiers = pressed
                ? _currentModifiers | EditorInputModifiers.Alt
                : _currentModifiers & ~EditorInputModifiers.Alt;
    }

    private static bool IsModifierKey(int vk) =>
        vk == VkShift || vk == VkControl || vk == VkMenu;

    private static string BuildSignature(EditorInputDevice device, string code,
        EditorInputGestureKind kind, EditorInputModifiers modifiers)
    {
        // 必须与 EditorInputGesture.BuildSignature() 完全一致：
        //   None → "" 而不是 "0"
        //   非 None → 枚举字符串（"Control", "Shift" 等）
        var mod = modifiers == EditorInputModifiers.None ? "" : modifiers.ToString();
        return $"{mod}|{kind}|{device}|{code}";
    }

    private static string ButtonCodeToName(int code) => code switch
    {
        1 => "Left",
        2 => "Right",
        3 => "Middle",   // VK_MBUTTON = 0x04, but WM uses different numbering
        4 => "Middle",
        5 => "X1",
        6 => "X2",
        _ => $"Button{code}"
    };

    // Win32 VK 常量
    private const int VkShift = 0x10;
    private const int VkControl = 0x11;
    private const int VkMenu = 0x12;
}
