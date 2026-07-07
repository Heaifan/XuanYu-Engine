namespace XuanYu.Editor.UI;

/// <summary>
/// 尺寸变化快照：只保存尺寸相关数据，不含生命周期日志或合并逻辑。
/// </summary>
public readonly record struct NativeHostResizeSnapshot(
    int Width,
    int Height,
    double DpiScale,
    bool IsValid,
    nint Hwnd);
