using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class VulkanNativeHost
{
    void TryAttach(NativeHostHandleSnapshot snapshot)
    {
        if (IsRendererReady || !snapshot.IsValid) return;
        var handle = NativeHostSurfaceContract.ToSurfaceHandle(snapshot);
        if (handle.Width <= 1 || handle.Height <= 1) return;
        ApplyNativeSize(handle.Width, handle.Height);
        _bridge ??= CreateBridge();
        if (!_bridge.Attach(handle)) return;
        _lastBridgeWidth = handle.Width;
        _lastBridgeHeight = handle.Height;
        IsRendererReady = true;
        RendererReady?.Invoke(this, EventArgs.Empty);
    }
}
