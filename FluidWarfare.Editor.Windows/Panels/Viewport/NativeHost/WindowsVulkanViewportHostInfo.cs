namespace FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;

public sealed record WindowsVulkanViewportHostInfo(
    WindowsVulkanViewportHostState State,
    string Message,
    string PlatformText,
    bool HasWindowHandle,
    nint WindowHandle,
    nint InstanceHandle)
{
    public bool IsCreated => State == WindowsVulkanViewportHostState.Created;

    public static WindowsVulkanViewportHostInfo NotCreated { get; } =
        new(
            WindowsVulkanViewportHostState.NotCreated,
            "Windows Vulkan 视口子窗口尚未创建。",
            "Windows",
            false,
            0,
            0);
}
