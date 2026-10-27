using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.World.Tests.Render.StaticModels;

public sealed class VulkanStaticModelIndexValidationTests
{
    [Fact]
    public void Rejects_index_equal_to_vertex_count()
    {
        var model = new RenderStaticModelResource(
            new RenderStaticModelKey("test:bad-index"), 1,
            [new RenderStaticModelVertex(default, default, 0, 0)],
            [1u],
            [new RenderStaticModelPrimitive(0, 1, 0, RenderStaticModelColor.Neutral)],
            default);

        Assert.False(VulkanStaticModelValidator.Validate(model, out var error));
        Assert.Equal("index outside vertex range", error);
    }

    [Fact]
    public void Accepts_last_valid_vertex_index()
    {
        var model = new RenderStaticModelResource(
            new RenderStaticModelKey("test:last-valid-index"), 1,
            [new RenderStaticModelVertex(default, default, 0, 0)],
            [0u],
            [new RenderStaticModelPrimitive(0, 1, 0, RenderStaticModelColor.Neutral)],
            default);

        Assert.True(VulkanStaticModelValidator.Validate(model, out var error), error);
    }

    [Fact]
    public void Rejects_primitive_range_that_would_overflow()
    {
        var model = new RenderStaticModelResource(
            new RenderStaticModelKey("test:overflow-range"), 1,
            [new RenderStaticModelVertex(default, default, 0, 0)], [0u],
            [new RenderStaticModelPrimitive(int.MaxValue, int.MaxValue, 0, RenderStaticModelColor.Neutral)],
            default);

        Assert.False(VulkanStaticModelValidator.Validate(model, out var error));
        Assert.Equal("primitive overflow", error);
    }
}
