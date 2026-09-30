using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class CameraWheelAdapterContractTests
{
    [Fact]
    public void Native_wheel_reaches_camera_mutation()
    {
        var vm = new UiVm(null, () => true);
        var before = vm.RenderSnapshot.CameraState.Position;
        var native = new NativePointerMessage(NativePointerMessage.Wheel,
            120 << 16, 10, 20, 0, 0, 0, 0);

        NativeViewportInputForwarder.Forward(vm.ViewportInput.Sink, native, new("native"));

        Assert.NotEqual(before, vm.RenderSnapshot.CameraState.Position);
    }

    [Fact]
    public void Avalonia_wheel_reaches_camera_mutation()
    {
        var vm = new UiVm(null, () => true);
        var before = vm.RenderSnapshot.CameraState.Position;
        var sample = new AvaloniaPointerSample(EditorPointerEventKind.Wheel,
            new(10, 20), EditorPointerButtons.None, EditorPointerModifiers.None, 1, 1);

        AvaloniaViewportInputForwarder.Forward(vm.ViewportInput.Sink, sample, new("avalonia"), 1);

        Assert.NotEqual(before, vm.RenderSnapshot.CameraState.Position);
    }

    [Fact]
    public void Router_wheel_path_leaves_camera_gesture_idle()
    {
        var vm = new UiVm(null, () => true);
        vm.ViewportInput.Sink.Handle(new EditorPointerEvent(EditorPointerEventKind.Wheel, new(1, 2),
            EditorPointerButtons.None, EditorPointerModifiers.None, 1, 1, new("router"), 1));

        Assert.False(vm.ViewportInput.Router.State.IsActive);
        Assert.False(vm.IsCameraNavigationActive);
    }
}
