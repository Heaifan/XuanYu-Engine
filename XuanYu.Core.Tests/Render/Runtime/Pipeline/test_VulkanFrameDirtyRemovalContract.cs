using System.Reflection;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Core.Tests.Render.Vulkan;

public sealed class VulkanFrameDirtyRemovalContractTests
{
    [Fact]
    public void Frame_state_does_not_expose_unconsumed_dirty_tracking()
    {
        Assert.Null(typeof(VulkanFrameState).GetProperty("FrameDirty"));
        Assert.DoesNotContain(typeof(VulkanFrameState).GetMethods(
            BindingFlags.Public | BindingFlags.Instance), method => method.Name == "MarkRecorded");
    }
}
