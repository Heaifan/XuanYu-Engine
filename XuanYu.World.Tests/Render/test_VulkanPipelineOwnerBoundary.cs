using System.Reflection;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanPipelineOwnerBoundaryTests
{
    [Fact]
    public void Resource_owner_has_no_pipeline_creation_entrypoint()
    {
        Assert.Null(typeof(VulkanGraphicsPipelineOwner).GetMethod(
            "Create", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
        Assert.NotNull(typeof(VulkanGraphicsPipelineOwner).GetMethod("Dispose"));
    }

    [Fact]
    public void Scene_pipeline_factory_is_a_separate_responsibility()
    {
        var factory = typeof(VulkanGraphicsPipelineOwner).Assembly.GetType(
            "XuanYu.Render.Vulkan.Pipeline.VulkanScenePipelineFactory");

        Assert.NotNull(factory);
        Assert.True(factory!.IsAbstract && factory.IsSealed);
        Assert.NotNull(factory.GetMethod("Create", BindingFlags.Static | BindingFlags.NonPublic));
    }
}
