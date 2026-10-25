namespace XuanYu.Render.Abstractions;

// VK3-A-R1：从 XuanYu.Render.Vulkan 迁入的纯生命周期探针。
// 仅负责按生命周期阶段抓取快照，不含任何 Vulkan / Silk.NET 依赖。
public sealed class NativeHostLifecycleProbe
{
    uint _version;

    public NativeHostHandleSnapshot Capture(NativeHostLifecycleState state, nint hwnd, int width, int height, double dpiScale, bool isValid) =>
        new(state, hwnd, width, height, dpiScale, isValid, ++_version, DateTimeOffset.Now);
}
