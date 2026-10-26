namespace XuanYu.Render.Vulkan.Render.Generation;

public static class VulkanPresentGenerationPolicy
{
    public static bool CanSelectImage(uint imageIndex, uint imageCount, bool generationCurrent)
        => generationCurrent && imageIndex < imageCount;

    public static bool CanRetire(bool graphicsComplete, bool presentComplete,
        bool completionUnknown, bool deviceLost)
        => !deviceLost && !completionUnknown && graphicsComplete && presentComplete;
}
