namespace XuanYu.Render.Vulkan.Render.VectorOverlay;

internal static class VulkanVectorOverlayBufferReusePolicy
{
    public static bool CanReuse(ulong capacityBytes, int requiredBytes) =>
        requiredBytes > 0 && (ulong)requiredBytes <= capacityBytes;
}
