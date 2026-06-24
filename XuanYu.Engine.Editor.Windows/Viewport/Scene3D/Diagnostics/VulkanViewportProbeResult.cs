namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;

/// <summary>Vulkan 探测结果。</summary>
public readonly record struct ProbeResult(bool Success, string Message)
{
    public static readonly ProbeResult Ok = new(true, string.Empty);
    public static ProbeResult Failed(string msg) => new(false, msg);
}
