using Avalonia.Input;
using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class RegionDrawingSnapRuntimeTests
{
    [Fact]
    public void Native_alt_suppresses_edge_snap_without_pointer_move()
    {
        var vm = BeginEdgeSnapPreview();
        vm.ViewportInput.Dispatch(NativeKeyboardEventAdapter.Convert(
            new NativeKeyMessage(NativeKeyMessage.KeyDown, 0x12, 0, false, false, false, true, false), new("native")));
        Assert.Equal("Alt 已取消吸附", vm.RegionDrawingSnapStatus);
        vm.ViewportInput.Dispatch(NativeKeyboardEventAdapter.Convert(
            new NativeKeyMessage(NativeKeyMessage.KeyUp, 0x12, 0, false, false, false, false, false), new("native")));
        Assert.Equal("边吸附", vm.RegionDrawingSnapStatus);
    }

    [Fact]
    public void Avalonia_alt_suppresses_edge_snap_without_pointer_move()
    {
        var vm = BeginEdgeSnapPreview();
        vm.ViewportInput.Dispatch(AvaloniaKeyboardEventAdapter.Convert(
            new AvaloniaKeySample((int)Key.LeftAlt, AvaloniaKeyAction.Down, AvaloniaKeyModifiers.Alt, false), new("avalonia")));
        Assert.Equal("Alt 已取消吸附", vm.RegionDrawingSnapStatus);
        vm.ViewportInput.Dispatch(AvaloniaKeyboardEventAdapter.Convert(
            new AvaloniaKeySample((int)Key.RightAlt, AvaloniaKeyAction.Up, AvaloniaKeyModifiers.None, false), new("avalonia")));
        Assert.Equal("边吸附", vm.RegionDrawingSnapStatus);
    }

    static UiVm BeginEdgeSnapPreview()
    {
        var vm = CreateWithExistingRegion(out _);
        vm.UpdateViewportFrame(800, 600);
        vm.SelectToolCommand.Execute("区域绘制");
        vm.ViewportInput.Dispatch(Pointer(EditorPointerEventKind.Pressed, 500, 500));
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, Viewport);
        var start = vm.MapSession.CurrentMap.Regions[0].Vertices[0];
        var end = vm.MapSession.CurrentMap.Regions[0].Vertices[1];
        var midpoint = projection.ProjectWorldPoint(new((start.X + end.X) / 2,
            (start.Y + end.Y) / 2, vm.MapSession.CurrentMap.Surface.BaseHeightMeters));
        vm.ViewportInput.Dispatch(Pointer(EditorPointerEventKind.Move, midpoint.X, midpoint.Y + 4));
        Assert.Equal("边吸附", vm.RegionDrawingSnapStatus);
        return vm;
    }
}
