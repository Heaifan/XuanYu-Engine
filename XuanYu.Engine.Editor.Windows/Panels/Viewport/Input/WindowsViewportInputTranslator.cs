using XuanYu.Engine.Editor.Input.Bindings;
using XuanYu.Engine.Editor.Input.Runtime;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.Input;

/// <summary>
/// Win32 原始输入 → EditorInputMatch 的翻译器门面。
/// 协调修饰键状态跟踪、原始输入翻译、手势签名匹配三个子组件。
/// </summary>
public sealed class WindowsViewportInputTranslator
{
    private readonly WindowsViewportModifierState _mods = new();
    private readonly WindowsViewportRawInputTranslate _translate;

    public WindowsViewportInputTranslator(EditorInputBindingSnapshot initialSnapshot)
    {
        _translate = new WindowsViewportRawInputTranslate(
            initialSnapshot ?? throw new ArgumentNullException(nameof(initialSnapshot)));
    }

    /// <summary>当前修饰键状态（用于外部查询）。</summary>
    public EditorInputModifiers CurrentModifiers => _mods.CurrentModifiers;

    /// <summary>当前快照修订号。</summary>
    public int Revision => _translate.Revision;

    /// <summary>Hot-reload 时由 EditorInputService 调用以更新快照引用。</summary>
    public void OnSnapshotReplaced(EditorInputBindingSnapshot newSnapshot)
        => _translate.OnSnapshotReplaced(newSnapshot);

    /// <summary>处理键盘键按下事件。更新修饰键状态后委托翻译。</summary>
    public EditorInputMatch OnRawKeyDown(int virtualKeyCode, int pointerX, int pointerY)
    {
        _mods.UpdateModifierState(virtualKeyCode, pressed: true);
        if (WindowsViewportModifierState.IsModifierKey(virtualKeyCode))
            return EditorInputMatch.NoMatch;
        return _translate.OnRawKeyDown(virtualKeyCode, pointerX, pointerY, _mods.CurrentModifiers);
    }

    /// <summary>处理键盘键抬起事件（仅用于修饰键状态跟踪）。</summary>
    public void OnRawKeyUp(int virtualKeyCode)
        => _mods.UpdateModifierState(virtualKeyCode, pressed: false);

    /// <summary>处理鼠标按钮按下事件。</summary>
    public EditorInputMatch OnRawPointerButtonDown(int buttonCode, int x, int y)
        => _translate.OnRawPointerButtonDown(buttonCode, x, y, _mods.CurrentModifiers);

    /// <summary>处理鼠标移动事件。</summary>
    public EditorInputMatch OnRawPointerMoved(int x, int y)
        => _translate.OnRawPointerMoved(x, y);

    /// <summary>处理鼠标按钮抬起事件。</summary>
    public void OnRawPointerButtonUp(int buttonCode)
        => _translate.OnRawPointerButtonUp();

    /// <summary>处理鼠标滚轮事件。</summary>
    public EditorInputMatch OnRawMouseWheel(int delta, int packedModifiers, int pointerX, int pointerY)
        => _translate.OnRawMouseWheel(delta, packedModifiers, pointerX, pointerY);

    /// <summary>取消活动拖拽（用于上下文切换或 hot-reload）。</summary>
    public void CancelActiveDrag()
        => _translate.CancelActiveDrag();

    /// <summary>焦点丢失时重置修饰键状态并结束活动拖动。</summary>
    public void OnRawInputFocusLost()
    {
        _mods.Reset();
        _translate.ClearDrag();
    }
}
