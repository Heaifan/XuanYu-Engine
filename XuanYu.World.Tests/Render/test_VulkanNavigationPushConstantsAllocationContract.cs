using System.IO;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanNavigationPushConstantsAllocationContractTests
{
    [Fact]
    public void Navigation_and_scale_push_constants_use_zeroed_stack_spans()
    {
        AssertZeroedStackSpan("VulkanClearFrameOwner.NavGizmo.cs");
        AssertZeroedStackSpan("VulkanClearFrameOwner.ScaleIndicator.cs");
    }

    static void AssertZeroedStackSpan(string fileName)
    {
        var path = Path.Combine(RepositoryRoot, "XuanYu.Render.Vulkan", "Render",
            "Overlay", "Navigation", fileName);
        var source = File.ReadAllText(path);

        Assert.Contains("stackalloc float[", source);
        Assert.Contains("scene.Clear();", source);
        Assert.DoesNotContain("new float[", source);
    }

    static string RepositoryRoot => Path.GetFullPath(TestSourceRoot.Root);
}
