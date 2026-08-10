using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport;

public sealed class NativePointerRoutePolicyTests
{
    [Fact]
    public void Middle_move_wins_over_region_preview()
    {
        var message = Move(buttons: 0x0010);

        var route = NativePointerRoutePolicy.Resolve(message, false, true);

        Assert.Equal(NativePointerRoute.CameraPreview, route);
    }

    [Fact]
    public void Active_camera_wins_even_when_middle_flag_is_missing()
    {
        var route = NativePointerRoutePolicy.Resolve(Move(), true, true);

        Assert.Equal(NativePointerRoute.CameraPreview, route);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Shift_middle_move_is_still_camera_preview(bool shift)
    {
        var buttons = 0x0010 | (shift ? 0x0004 : 0);

        var route = NativePointerRoutePolicy.Resolve(Move(buttons), false, true);

        Assert.Equal(NativePointerRoute.CameraPreview, route);
    }

    [Fact]
    public void Region_preview_only_wins_without_camera_input()
    {
        var route = NativePointerRoutePolicy.Resolve(Move(), false, true);

        Assert.Equal(NativePointerRoute.RegionPreview, route);
    }

    [Fact]
    public void Active_navigation_gizmo_wins_over_region_preview()
    {
        var route = NativePointerRoutePolicy.Resolve(
            Move(buttons: 0x0001), false, true, navGizmoPressed: true);

        Assert.Equal(NativePointerRoute.LeftPreview, route);
    }

    [Fact]
    public void Middle_down_and_up_have_dedicated_routes()
    {
        Assert.Equal(NativePointerRoute.MiddleDown, NativePointerRoutePolicy.Resolve(
            Message(NativePointerMessage.MiddleDown), false, false));
        Assert.Equal(NativePointerRoute.MiddleUp, NativePointerRoutePolicy.Resolve(
            Message(NativePointerMessage.MiddleUp), true, false));
    }

    static NativePointerMessage Move(int buttons = 0) => Message(NativePointerMessage.Move, buttons);

    static NativePointerMessage Message(uint message, int buttons = 0) =>
        new(message, buttons, 0, 0, 0, 0, 0, 0);
}
