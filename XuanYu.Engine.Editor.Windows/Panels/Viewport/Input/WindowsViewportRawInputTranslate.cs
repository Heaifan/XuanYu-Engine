using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Bindings;
using FluidWarfare.Editor.Input.Runtime;

namespace FluidWarfare.Editor.Windows.Panels.Viewport.Input;

/// <summary>原始输入事件 → EditorInputMatch 翻译。通过 EditorInputBindingSnapshot 解析手势签名。</summary>
public sealed class WindowsViewportRawInputTranslate
{
    static readonly bool s_trace = Environment.GetEnvironmentVariable("FW_INPUT_TRACE") == "1";
    EditorInputBindingSnapshot _snapshot;
    int _lastMouseX, _lastMouseY;

    public WindowsViewportRawInputTranslate(EditorInputBindingSnapshot s)
        => _snapshot = s ?? throw new ArgumentNullException(nameof(s));

    public int Revision => _snapshot.Revision;

    public void OnSnapshotReplaced(EditorInputBindingSnapshot s)
        => _snapshot = s ?? throw new ArgumentNullException(nameof(s));

    void Trace(string m) { if (s_trace) System.Diagnostics.Debug.WriteLine(m); }

    EditorInputMatch MakeMatch(EditorInputActionDefinition a) => new()
    {
        ActionId = a.Id, Definition = a, ValueKind = a.ValueKind,
        DeltaX = 0, DeltaY = 0, BindingRevision = _snapshot.Revision
    };

    /// <summary>键盘键按下（修饰键已被门面过滤）。通过 KeyCodeMapper 转换后查表。</summary>
    public EditorInputMatch OnRawKeyDown(int vk, int px, int py, EditorInputModifiers mods)
    {
        var code = Win32KeyCodeMapper.Map(vk);
        if (code is null) return EditorInputMatch.NoMatch;
        var a = _snapshot.Resolve(WindowsViewportGestureMatch.BuildSignature(
            EditorInputDevice.Keyboard, code, EditorInputGestureKind.KeyPress, mods));
        return a is null ? EditorInputMatch.NoMatch : MakeMatch(a);
    }

    /// <summary>鼠标按钮按下。解析为 MouseDrag 手势，锁定拖拽绑定。</summary>
    public EditorInputMatch OnRawPointerButtonDown(int btn, int x, int y, EditorInputModifiers mods)
    {
        _lastMouseX = x; _lastMouseY = y;
        var code = WindowsViewportGestureMatch.ButtonCodeToName(btn);
        var sig = WindowsViewportGestureMatch.BuildSignature(
            EditorInputDevice.Mouse, code, EditorInputGestureKind.MouseDrag, mods);
        Trace($"[InputTrace-Translator] OnPointerButtonDown btn={btn}->\"{code}\" sig=\"{sig}\"");
        _snapshot.BeginDrag(sig, x, y);
        var a = _snapshot.Resolve(sig);
        if (a is null) { Trace($"[InputTrace-Translator] Resolve sig=\"{sig}\" → null"); return EditorInputMatch.NoMatch; }
        Trace($"[InputTrace-Translator] Resolve sig=\"{sig}\" → action=\"{a.Id}\"");
        return new EditorInputMatch { ActionId = a.Id, Definition = a, ValueKind = EditorInputValueKind.PointerDelta, DeltaX = 0, DeltaY = 0, BindingRevision = _snapshot.Revision };
    }

    /// <summary>鼠标移动。拖拽中通过 GetActiveDragDefinition O(1) 获取定义。</summary>
    public EditorInputMatch OnRawPointerMoved(int x, int y)
    {
        var dx = x - _lastMouseX; var dy = y - _lastMouseY;
        _lastMouseX = x; _lastMouseY = y;
        var d = _snapshot.GetActiveDragDefinition();
        return d is null ? EditorInputMatch.NoMatch
            : new EditorInputMatch { ActionId = d.Id, Definition = d, ValueKind = EditorInputValueKind.PointerDelta, DeltaX = dx, DeltaY = dy, BindingRevision = _snapshot.Revision };
    }

    /// <summary>鼠标按钮抬起。结束拖拽锁定。</summary>
    public void OnRawPointerButtonUp() => _snapshot.EndDrag();

    /// <summary>鼠标滚轮。使用 Win32 消息的 packedModifiers（不依赖可能过期的 _currentModifiers）。</summary>
    public EditorInputMatch OnRawMouseWheel(int delta, int packedMods, int px, int py)
    {
        var mkCtrl = (packedMods & 0x0008) != 0;
        var mkShift = (packedMods & 0x0004) != 0;
        EditorInputModifiers mods = EditorInputModifiers.None;
        if (mkCtrl) mods |= EditorInputModifiers.Control;
        if (mkShift) mods |= EditorInputModifiers.Shift;
        var sig = WindowsViewportGestureMatch.BuildSignature(
            EditorInputDevice.Wheel, "Y", EditorInputGestureKind.MouseWheel, mods);
        Trace($"[InputTrace-Translator] OnMouseWheel delta={delta} mods={mods} sig=\"{sig}\"");
        var a = _snapshot.Resolve(sig);
        if (a is null) { Trace($"[InputTrace-Translator] Resolve wheel sig=\"{sig}\" → null"); return EditorInputMatch.NoMatch; }
        Trace($"[InputTrace-Translator] Resolve wheel sig=\"{sig}\" → action=\"{a.Id}\"");
        return new EditorInputMatch { ActionId = a.Id, Definition = a, ValueKind = EditorInputValueKind.WheelDelta, WheelDelta = delta / 120.0f, BindingRevision = _snapshot.Revision };
    }

    /// <summary>取消活动拖拽（上下文切换或 hot-reload）。</summary>
    public void CancelActiveDrag() => _snapshot.EndDrag();

    /// <summary>清除拖拽状态（焦点丢失时调用）。</summary>
    public void ClearDrag() => _snapshot.EndDrag();
}
