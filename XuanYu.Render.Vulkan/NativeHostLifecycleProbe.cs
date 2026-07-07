namespace XuanYu.Render.Vulkan;

public sealed class NativeHostLifecycleProbe
{
    uint _version;

    public NativeHostHandleSnapshot Capture(NativeHostLifecycleState state, nint hwnd, int width, int height, double dpiScale, bool isValid) =>
        new(state, hwnd, width, height, dpiScale, isValid, ++_version, DateTimeOffset.Now);
}
