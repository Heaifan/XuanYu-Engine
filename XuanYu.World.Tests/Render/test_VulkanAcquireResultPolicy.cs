using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanAcquireResultPolicyTests
{
    [Theory]
    [InlineData(Result.Success, "Acquired")]
    [InlineData(Result.SuboptimalKhr, "Acquired")]
    [InlineData(Result.Timeout, "TimeoutRetry")]
    [InlineData(Result.ErrorOutOfDateKhr, "RetryAfterRecreate")]
    [InlineData(Result.ErrorDeviceLost, "StopOrFatal")]
    public void Acquire_result_policy_distinguishes_image_availability(Result result, string expected)
    {
        var recoverCalls = 0;
        var decision = VulkanAcquireResultPolicy.Decide(result, () => { recoverCalls++; return true; });
        Assert.Equal(expected, decision.ToString());
        Assert.Equal(result == Result.ErrorOutOfDateKhr ? 1 : 0, recoverCalls);
    }

    [Theory]
    [InlineData(true, "RetryAfterRecreate")]
    [InlineData(false, "StopOrFatal")]
    public void Out_of_date_never_becomes_an_acquired_image(bool recovered, string expected)
    {
        var decision = VulkanAcquireResultPolicy.Decide(Result.ErrorOutOfDateKhr, () => recovered);
        Assert.Equal(expected, decision.ToString());
        Assert.NotEqual(AcquireDecision.Acquired, decision);
    }

    [Fact]
    public void Repeated_same_size_recovery_retries_do_not_authorize_submission()
    {
        var resetFenceCount = 0;
        var submitCount = 0;
        var acquireCount = 0;
        for (var i = 0; i < 4; i++)
        {
            acquireCount++;
            var decision = VulkanAcquireResultPolicy.Decide(Result.ErrorOutOfDateKhr, () => true);
            if (decision == AcquireDecision.Acquired) { resetFenceCount++; submitCount++; }
        }
        Assert.Equal(4, acquireCount);
        Assert.Equal(0, resetFenceCount);
        Assert.Equal(0, submitCount);
    }

    [Theory]
    [InlineData(0u, 3, true, true)]
    [InlineData(2u, 3, true, true)]
    [InlineData(3u, 3, true, false)]
    [InlineData(0u, 3, false, false)]
    public void Only_an_index_from_the_current_swapchain_is_submit_eligible(
        uint index, int imageCount, bool currentSwapchain, bool expected)
    {
        Assert.Equal(expected, VulkanAcquireResultPolicy.HasValidImageIndex(index, imageCount, currentSwapchain));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(4, true)]
    [InlineData(5, false)]
    public void Recovery_retries_are_bounded(int retryCount, bool expected) =>
        Assert.Equal(expected, VulkanAcquireResultPolicy.CanRetryRecovery(retryCount));
}
