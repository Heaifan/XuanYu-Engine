using XuanYu.Editor.Input;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport;

public sealed class MapContextMenuRouterTests
{
    [Fact]
    public void Composition_routes_right_press_to_context_consumer_before_map_edit()
    {
        var composition = UiVmViewportInputComposition.Create(new UiVm(null, seedInitialScene: false));
        var context = Assert.Single(composition.Consumers, item => item.Owner == GestureOwner.ContextMenu);
        var right = new EditorPointerEvent(EditorPointerEventKind.Pressed, new(4, 5),
            EditorPointerButtons.Right, EditorPointerModifiers.None, 0, 1, new("test"), 1);

        Assert.True(context.CanBegin(right, ViewportGestureState.Idle));
        Assert.Equal(GestureOwner.ContextMenu, composition.Consumers[0].Owner);
    }
}
