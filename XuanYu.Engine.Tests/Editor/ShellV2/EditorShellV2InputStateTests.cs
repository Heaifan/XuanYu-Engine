using XuanYu.Engine.Editor.Windows.ShellV2.Composition.Input;

namespace XuanYu.Engine.Tests.Editor.ShellV2;

/// <summary>EditorShellV2InputState 纯状态机测试。不依赖 Avalonia 窗口。</summary>
public sealed class EditorShellV2InputStateTests
{
    [Fact]
    public void InitialState_AllFalse()
    {
        var s = new EditorShellV2InputState();
        Assert.False(s.IsShiftDown);
        Assert.False(s.IsMiddleDown);
        Assert.False(s.IsOrbiting);
        Assert.False(s.IsPanning);
    }

    [Fact]
    public void ShiftDown_SetsShift_DoesNotTriggerOrbitOrPan()
    {
        var s = new EditorShellV2InputState();
        s.OnShiftDown();
        Assert.True(s.IsShiftDown);
        Assert.False(s.IsMiddleDown);
        Assert.False(s.IsOrbiting);
        Assert.False(s.IsPanning);
    }

    [Fact]
    public void MiddleDown_WithoutShift_TriggersOrbit()
    {
        var s = new EditorShellV2InputState();
        s.OnMiddleDown(100, 200);
        Assert.True(s.IsMiddleDown);
        Assert.True(s.IsOrbiting);
        Assert.False(s.IsPanning);
        Assert.False(s.IsShiftDown);
    }

    [Fact]
    public void MiddleDown_WithShiftDown_TriggersPan()
    {
        var s = new EditorShellV2InputState();
        s.OnShiftDown();
        s.OnMiddleDown(100, 200);
        Assert.True(s.IsMiddleDown);
        Assert.True(s.IsShiftDown);
        Assert.False(s.IsOrbiting);
        Assert.True(s.IsPanning);
    }

    [Fact]
    public void MiddleUp_ClearsOrbitAndPan()
    {
        var s = new EditorShellV2InputState();
        s.OnMiddleDown(100, 200);
        Assert.True(s.IsOrbiting);
        s.OnMiddleUp();
        Assert.False(s.IsMiddleDown);
        Assert.False(s.IsOrbiting);
        Assert.False(s.IsPanning);
    }

    [Fact]
    public void MiddleUp_WithShift_KeepsShiftDown()
    {
        var s = new EditorShellV2InputState();
        s.OnShiftDown();
        s.OnMiddleDown(100, 200);
        Assert.True(s.IsPanning);
        s.OnMiddleUp();
        Assert.False(s.IsMiddleDown);
        Assert.False(s.IsPanning);
        Assert.True(s.IsShiftDown); // Shift 仍在按下
    }

    [Fact]
    public void ShiftUp_ClearsShift()
    {
        var s = new EditorShellV2InputState();
        s.OnShiftDown();
        s.OnMiddleDown(100, 200);
        Assert.True(s.IsPanning);
        s.OnShiftUp();
        Assert.False(s.IsShiftDown);
        Assert.True(s.IsMiddleDown); // 中键仍按下
        Assert.True(s.IsOrbiting);   // 不按 Shift → 切到 Orbit
        Assert.False(s.IsPanning);
    }

    [Fact]
    public void FocusLost_ClearsAll()
    {
        var s = new EditorShellV2InputState();
        s.OnShiftDown();
        s.OnMiddleDown(100, 200);
        Assert.True(s.IsPanning);
        s.OnFocusLost();
        Assert.False(s.IsShiftDown);
        Assert.False(s.IsMiddleDown);
        Assert.False(s.IsOrbiting);
        Assert.False(s.IsPanning);
    }

    [Fact]
    public void ShiftOnly_Move_DoesNotTriggerPan()
    {
        var s = new EditorShellV2InputState();
        s.OnShiftDown();
        s.OnMove(300, 400);
        // 只按 Shift 移动鼠标，不应 Pan
        Assert.False(s.IsOrbiting);
        Assert.False(s.IsPanning);
        Assert.True(s.IsShiftDown);
        // LastX/LastY 更新
        Assert.Equal(300, s.LastX);
        Assert.Equal(400, s.LastY);
    }

    [Fact]
    public void MiddleDown_RecordsLastPosition()
    {
        var s = new EditorShellV2InputState();
        s.OnMiddleDown(640, 360);
        Assert.Equal(640, s.LastX);
        Assert.Equal(360, s.LastY);
    }

    [Fact]
    public void Move_UpdatesLastPosition()
    {
        var s = new EditorShellV2InputState();
        s.OnMiddleDown(100, 200);
        s.OnMove(150, 250);
        Assert.Equal(150, s.LastX);
        Assert.Equal(250, s.LastY);
    }
}
