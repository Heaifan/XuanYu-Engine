using XuanYu.Render.Vulkan.Render.Generation;

namespace XuanYu.Render.Vulkan.Render.Frame;

/// <summary>Pure ownership rules shared by the present loop and CPU contract tests.</summary>
internal static class VulkanFrameInFlightPolicy
{
    internal static int NextFrameIndex(int current, int frameCount)
    {
        if (frameCount is < 1 or > 2) throw new ArgumentOutOfRangeException(nameof(frameCount));
        if (current < 0 || current >= frameCount) throw new ArgumentOutOfRangeException(nameof(current));
        return (current + 1) % frameCount;
    }

    internal static int CommandBufferIndex(int frameIndex, uint imageIndex, int imageCount,
        int frameCount)
    {
        if (frameCount is < 1 or > 2 || frameIndex < 0 || frameIndex >= frameCount)
            throw new ArgumentOutOfRangeException(nameof(frameIndex));
        if (imageCount < 1 || imageIndex >= imageCount)
            throw new ArgumentOutOfRangeException(nameof(imageIndex));
        return checked(frameIndex * imageCount + (int)imageIndex);
    }

    internal static bool CanReuseFrame(bool graphicsPending, bool graphicsComplete,
        bool presentPending, bool presentComplete)
        => (!graphicsPending || graphicsComplete) && (!presentPending || presentComplete);

    internal static bool CanUseImage(uint imageIndex, uint imageCount, uint generation,
        uint activeGeneration, bool ownerComplete)
        => VulkanPresentGenerationPolicy.CanSelectImage(
            imageIndex, imageCount, generation == activeGeneration) && ownerComplete;

    internal static bool CanChangeFrameCount(int frameCount, int pendingFrameCount)
        => frameCount is >= 1 and <= 2 && pendingFrameCount == 0;

}
