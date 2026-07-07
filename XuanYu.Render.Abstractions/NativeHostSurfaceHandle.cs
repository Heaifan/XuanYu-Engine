namespace XuanYu.Render.Abstractions;

// NativeHost 交给渲染层的窗口交接句柄。
// 只携带创建 Win32 Surface 所需的最小信息，不含任何 Vulkan 类型。
// Hinstance 是 Win32SurfaceCreateInfoKHR 的必需字段，由 NativeHost 边界
// 从 Win32ViewportHost.ModuleHandle 提供，避免渲染层反向依赖 Editor.Win。
public readonly record struct NativeHostSurfaceHandle(
    nint Hwnd,
    nint Hinstance,
    int Width,
    int Height,
    double DpiScale);
