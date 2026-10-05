using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class EditorWheelUsesObservationCenterTests
{
    [Fact]
    public void Editor_wheel_does_not_use_pointer_anchor()
    {
        var states = new[] { (20d, 300d), (400d, 300d), (780d, 300d), (400d, 20d), (400d, 580d) }
            .Select(point => WheelAt(point.Item1, point.Item2)).ToArray();

        Assert.Equal(states[0], states[1]);
        Assert.Equal(states[1], states[2]);
    }

    static CameraState WheelAt(double x, double y)
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        var wheel = new EditorPointerEvent(EditorPointerEventKind.Wheel, new(x, y),
            EditorPointerButtons.None, EditorPointerModifiers.None, 1, 1,
            new("test"), 1);
        vm.ViewportInput.Sink.Handle(wheel);
        return vm.RenderSnapshot.CameraState;
    }
}
