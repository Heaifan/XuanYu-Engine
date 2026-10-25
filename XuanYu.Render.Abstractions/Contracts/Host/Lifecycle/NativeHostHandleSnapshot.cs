namespace XuanYu.Render.Abstractions;

// VK3-A-R1：从 XuanYu.Render.Vulkan 迁入的纯生命周期快照。
// 不含任何 Vulkan / Silk.NET 依赖，仅打包 HWND / 尺寸 / DPI / 生命周期状态。
public sealed record NativeHostHandleSnapshot(
    NativeHostLifecycleState State,
    nint Hwnd,
    int Width,
    int Height,
    double DpiScale,
    bool IsValid,
    uint Version,
    DateTimeOffset CapturedAt);
