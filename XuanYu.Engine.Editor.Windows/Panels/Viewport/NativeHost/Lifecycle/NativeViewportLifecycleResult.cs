namespace XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost.Lifecycle;

/// <summary>NativeHost 创建结果。</summary>
sealed record NativeViewportLifecycleResult(nint WindowHandle, nint InstanceHandle, bool Success, string? ErrorMessage)
{
    public static readonly NativeViewportLifecycleResult Failed = new(0, 0, false, null);
}
