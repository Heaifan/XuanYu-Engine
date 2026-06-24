namespace XuanYu.Engine.Editor.Windows.Shell.Input;

/// <summary>视口输入事件类型。Route 根据 Kind 决定分发路径。</summary>
public enum EditorViewportInputKind
{
    KeyDown,
    KeyUp,
    PointerDown,
    PointerMoved,
    PointerUp,
    MouseWheel,
    FocusLost
}
