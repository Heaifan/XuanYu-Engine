namespace XuanYu.Render.Vulkan;

public sealed record NativeHostHandleSnapshot(
    NativeHostLifecycleState State,
    nint Hwnd,
    int Width,
    int Height,
    double DpiScale,
    bool IsValid,
    uint Version,
    DateTimeOffset CapturedAt);
