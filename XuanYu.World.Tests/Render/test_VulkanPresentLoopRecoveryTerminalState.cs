using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanPresentLoopRecoveryTerminalStateTests
{
    [Theory]
    [InlineData(false, VulkanPresentLoopState.Stopped)]
    [InlineData(true, VulkanPresentLoopState.Failed)]
    public void Worker_exit_preserves_terminal_outcome(bool failed, VulkanPresentLoopState expected)
    {
        var state = VulkanPresentLoopStatePolicy.AfterWorkerExit(failed);
        Assert.Equal(expected, state);
        Assert.Equal(!failed, VulkanPresentLoopStatePolicy.CanStart(state));
    }

    [Theory]
    [InlineData(VulkanPresentLoopState.Stopped, true)]
    [InlineData(VulkanPresentLoopState.Running, true)]
    [InlineData(VulkanPresentLoopState.Failed, false)]
    public void Failed_loop_cannot_be_restarted(VulkanPresentLoopState state, bool expected)
    {
        Assert.Equal(expected, VulkanPresentLoopStatePolicy.CanStart(state));
    }

    [Fact]
    public void Acquire_recovery_failure_is_promoted_to_fatal()
    {
        var source = File.ReadAllText(Path.Combine(
            TestSourceRoot.Root,
            "XuanYu.Render.Vulkan", "Render", "Frame", "PresentLoop", "Execution", "VulkanPresentLoopFrame.cs"));

        Assert.Contains("decision == AcquireDecision.StopOrFatal", source);
        Assert.Contains("Fatal(\"AcquireNextImage 恢复失败\")", source);
    }

    [Fact]
    public void Out_of_date_callback_failure_returns_stop_or_fatal()
    {
        var callbackCount = 0;
        var decision = VulkanAcquireResultPolicy.Decide(Result.ErrorOutOfDateKhr, () =>
        {
            callbackCount++;
            return false;
        });

        Assert.Equal(AcquireDecision.StopOrFatal, decision);
        Assert.Equal(1, callbackCount);
    }
}
