using System.IO;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanSessionLogOwnershipContractTests
{
    [Fact]
    public void Session_disposal_log_uses_session_owned_formatter()
    {
        var sourceRoot = TestSourceRoot.Root;
        var clearFrame = File.ReadAllText(Path.Combine(sourceRoot, "XuanYu.Render.Vulkan", "Render",
            "ClearFrame", "Diagnostics", "VulkanClearFrameLogFormatter.cs"));
        var session = File.ReadAllText(Path.Combine(sourceRoot, "XuanYu.Render.Vulkan", "Session",
            "Runtime", "Lifecycle.cs"));
        var formatter = File.ReadAllText(Path.Combine(sourceRoot, "XuanYu.Render.Vulkan", "Session",
            "Diagnostics", "VulkanRenderSessionLogFormatter.cs"));

        Assert.DoesNotContain("SessionDisposed", clearFrame);
        Assert.Contains("VulkanRenderSessionLogFormatter.Disposed()", session);
        Assert.Contains("VulkanRenderSession】释放完成", formatter);
    }
}
