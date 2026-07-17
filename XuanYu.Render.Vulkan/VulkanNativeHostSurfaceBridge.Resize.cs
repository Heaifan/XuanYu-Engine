namespace XuanYu.Render.Vulkan;

public sealed partial class VulkanNativeHostSurfaceBridge
{
    public void Resize(int width, int height)
    {
        if (_instanceOwner is null || _surfaceOwner is null || _renderSession is null || _failed)
        {
            Emit(VulkanBridgeLogFormatter.ResizedSkipped(width, height));
            return;
        }
        if (_renderSession.IsFailed)
        {
            _failed = true;
            Emit(VulkanBridgeLogFormatter.SessionFailed(_renderSession.FailureReason ?? "未知原因"));
            return;
        }
        Emit(VulkanBridgeLogFormatter.Resized(width, height));
        if (_renderSession.Resize(width, height)) return;
        _failed = true;
        Emit(VulkanBridgeLogFormatter.ResizeFailed());
        Detach();
    }
}
