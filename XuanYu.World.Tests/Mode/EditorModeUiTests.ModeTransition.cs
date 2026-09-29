using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Mode;

public sealed partial class EditorModeUiTests
{
    [Fact]
    public void Mode_change_cancels_production_capture_and_clears_gizmo_hover()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        vm.ViewportInput.Sink.Handle(new EditorPointerEvent(
            EditorPointerEventKind.Pressed, new(300, 300), EditorPointerButtons.Middle,
            EditorPointerModifiers.None, 0, 1, new("test"), 1));
        vm.SetNavigationGizmoHover(2);
        vm.SetNavigationGizmoCenterHover(true);

        Assert.True(vm.ViewportInput.Router.State.IsActive);
        Assert.True(vm.IsCameraNavigationActive);
        Assert.True(vm.ToggleEditorMode());
        Assert.Equal(ViewportGestureState.Idle, vm.ViewportInput.Router.State);
        Assert.False(vm.IsCameraNavigationActive);
        var assist = vm.RenderProjection.Projection.AssistState;
        Assert.Equal(-1, assist.NavGizmoHoverIndex);
        Assert.False(assist.NavGizmoCenterHover);
        Assert.Equal(-1, assist.NavGizmoPressedIndex);
    }
}
