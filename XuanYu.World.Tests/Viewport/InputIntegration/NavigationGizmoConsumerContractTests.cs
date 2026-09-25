using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Consumers;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class NavigationGizmoConsumerContractTests
{
    [Fact]
    public void Navigation_owner_is_a_distinct_production_gesture_owner()
    {
        Assert.NotEqual(GestureOwner.None, GestureOwner.Navigation);
        Assert.NotEqual(GestureOwner.Gizmo, GestureOwner.Navigation);
    }

    [Fact]
    public void Navigation_consumer_has_priority_over_picking_but_not_active_owner()
    {
        var navigation = new NavigationViewportInputConsumer(new NavigationProbe(true));
        Assert.True(navigation.BeginPriority > 100);
        Assert.Equal(GestureOwner.Navigation, navigation.Owner);
    }

    sealed class NavigationProbe(bool claim) : INavigationViewportInputHandler
    {
        public bool CanClaim(EditorPointerEvent pointer) => claim;
        public void Observe(EditorPointerEvent pointer) { }
        public void Begin(XuanYu.Editor.Input.Lifecycle.ViewportGestureContext context) { }
        public void Update(XuanYu.Editor.Input.Lifecycle.ViewportGestureContext context) { }
        public void Commit(XuanYu.Editor.Input.Lifecycle.ViewportGestureContext context) { }
        public void Cancel(XuanYu.Editor.Input.Lifecycle.ViewportCancellationContext context) { }
    }
}
