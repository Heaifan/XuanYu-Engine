using XuanYu.Core.Space;
using Avalonia.Input;
using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RegionDrawingSnapRuntimeTests
{
    [Fact]
    public void Native_alt_down_and_up_refresh_snap_without_pointer_move()
    {
        var vm = BeginSnapPreview();
        vm.ViewportInput.Dispatch(NativeKeyboardEventAdapter.Convert(
            new NativeKeyMessage(NativeKeyMessage.KeyDown, 0x12, 0, false, false, false, true, false),
            new("native")));
        Assert.False(vm.IsRegionDrawingSnapActive);
        vm.ViewportInput.Dispatch(NativeKeyboardEventAdapter.Convert(
            new NativeKeyMessage(NativeKeyMessage.KeyUp, 0x12, 0, false, false, false, false, false),
            new("native")));
        Assert.True(vm.IsRegionDrawingSnapActive);
    }

    [Fact]
    public void Avalonia_alt_down_and_up_refresh_snap_without_pointer_move()
    {
        var vm = BeginSnapPreview();
        vm.ViewportInput.Dispatch(AvaloniaKeyboardEventAdapter.Convert(
            new AvaloniaKeySample((int)Key.LeftAlt, AvaloniaKeyAction.Down, AvaloniaKeyModifiers.Alt, false),
            new("avalonia")));
        Assert.False(vm.IsRegionDrawingSnapActive);
        vm.ViewportInput.Dispatch(AvaloniaKeyboardEventAdapter.Convert(
            new AvaloniaKeySample((int)Key.RightAlt, AvaloniaKeyAction.Up, AvaloniaKeyModifiers.None, false),
            new("avalonia")));
        Assert.True(vm.IsRegionDrawingSnapActive);
    }

    [Theory]
    [InlineData((int)Key.LeftAlt)]
    [InlineData((int)Key.RightAlt)]
    public void Avalonia_alt_keys_normalize_to_editor_alt(int key)
    {
        var result = AvaloniaKeyboardEventAdapter.Convert(
            new AvaloniaKeySample(key, AvaloniaKeyAction.Down, AvaloniaKeyModifiers.Alt, false),
            new("avalonia"));
        Assert.Equal(0x12, result.Key.VirtualKeyCode);
    }

    [Fact]
    public void Focus_lost_cancels_region_draft_and_clears_snap()
    {
        var vm = BeginSnapPreview();
        vm.ViewportInput.Dispatch(Pointer(EditorPointerEventKind.FocusLost, 0, 0));
        Assert.False(vm.IsRegionDrawingSnapActive);
        Assert.False(vm.IsRegionDrawingDraftActive);
    }

    static UiVm BeginSnapPreview()
    {
        var vm = CreateWithExistingRegion(out var target);
        vm.UpdateViewportFrame(800, 600);
        vm.SelectToolCommand.Execute("区域绘制");
        var start = FindHit(vm, 500, 500);
        vm.ViewportInput.Dispatch(Pointer(EditorPointerEventKind.Pressed, start.X, start.Y));
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var screen = projection.ProjectWorldPoint(new(target.X, target.Y,
            vm.MapSession.CurrentMap.Surface.BaseHeightMeters));
        vm.ViewportInput.Dispatch(Pointer(EditorPointerEventKind.Move, screen.X + 4, screen.Y));
        Assert.True(vm.IsRegionDrawingSnapActive);
        return vm;
    }

    static EditorPointerEvent Pointer(EditorPointerEventKind kind, double x, double y) => new(
        kind, new(x, y), EditorPointerButtons.Left, EditorPointerModifiers.None, 0, 1,
        new("test"), 1);
}
