namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Overlay 资源 + 帧状态释放步骤。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private void DisposeOverlayResourcesStep()
    {
        if (_overlayResources is not null)
        {
            _overlayResources.Dispose();
            _overlayResources = null;
        }
    }

    private void ClearFrameOverlayState()
    {
        _pendingOverlayLayout = null;
        _lastPresentedOverlaySnapshot = Overlay.PresentedNavigationOverlaySnapshot.Empty;
        _lastOverlayVertexCount = 0;
    }
}
