namespace XuanYu.Engine.Editor.Windows.ShellV2.Composition.Input;

/// <summary>EditorShellV2 输入状态机。Shift 仅表示修饰键，不直接产生导航动作。</summary>
sealed class EditorShellV2InputState
{
    public bool IsShiftDown { get; private set; }
    public bool IsMiddleDown { get; private set; }
    public bool IsOrbiting => IsMiddleDown && !IsShiftDown;
    public bool IsPanning => IsMiddleDown && IsShiftDown;
    public int LastX, LastY;

    public void OnShiftDown() => IsShiftDown = true;
    public void OnShiftUp() => IsShiftDown = false;
    public void OnMiddleDown(int x, int y) { IsMiddleDown = true; LastX = x; LastY = y; }
    public void OnMiddleUp() { IsMiddleDown = false; }
    public void OnMove(int x, int y) { LastX = x; LastY = y; }
    public void OnFocusLost() { IsShiftDown = false; IsMiddleDown = false; }
}
