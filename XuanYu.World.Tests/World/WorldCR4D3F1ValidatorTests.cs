using XuanYu.Editor.Assets;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.World.Tests.World;

public sealed class WorldCR4D3F1ValidatorTests
{
    readonly GlbImportService _import = new();

    [Fact]
    public void Out_of_range_index_is_rejected()
    {
        // SharpGLTF 边界会先拒绝索引越界 GLB（ParserFailure）；
        // 玄域 InvalidIndex 是第二道防御。任一层拒绝都满足“越界不被接受”。
        var result = _import.ImportBytes(WorldCR4D1GlbFactory.BadIndexTriangle(), "bad.glb");

        Assert.False(result.Succeeded);
        Assert.NotEqual(StaticModelImportErrorCode.None, result.ErrorCode);
    }

    [Fact]
    public void Internal_importer_rejects_out_of_range_index()
    {
        // 绕过 SharpGLTF 预校验，直接验证玄域导入边界的 InvalidIndex 防御路径。
        var bytes = WorldCR4D1GlbFactory.BadIndexTriangle();
        GlbContainerReader.Read(bytes, out var container);
        var result = new GltfStaticModelImporter(container!, "bad.glb", bytes.Length).Import();

        Assert.False(result.Succeeded);
        Assert.Equal(StaticModelImportErrorCode.InvalidIndex, result.ErrorCode);
    }

    [Fact]
    public void Overflowing_global_index_is_rejected()
    {
        var builder = new StaticModelBuilder();
        builder.AddPrimitive([Vertex()], [0u], StaticModelColor.Neutral);
        // baseVertex=1，索引 uint.MaxValue → 1 + uint.MaxValue 溢出。
        Assert.Throws<OverflowException>(() =>
            builder.AddPrimitive([Vertex()], [uint.MaxValue], StaticModelColor.Neutral));
    }

    [Fact]
    public void Normalized_resource_passes_vulkan_validator()
    {
        var model = Import(WorldCR4D1GlbFactory.MultiPrimitive());
        var resource = StaticModelRenderAdapter.ToRenderResource(
            model, new RenderStaticModelKey("f1:multi"), 1);

        Assert.True(VulkanStaticModelValidator.Validate(resource, out var error), error);
    }

    [Fact]
    public void Non_zero_base_vertex_resource_is_still_rejected()
    {
        var resource = new RenderStaticModelResource(
            new RenderStaticModelKey("f1:bad"), 1,
            [new RenderStaticModelVertex(default, default, 0, 0)],
            [0u],
            [new RenderStaticModelPrimitive(0, 1, 3, RenderStaticModelColor.Neutral)],
            default);

        Assert.False(VulkanStaticModelValidator.Validate(resource, out var error));
        Assert.Equal("non-zero BaseVertex not supported", error);
    }

    static StaticModelData Import(byte[] glb) =>
        new GlbImportService().ImportBytes(glb, "t.glb").Model!;

    static StaticModelVertex Vertex() =>
        new(new(0, 0, 0), new(0, 0, 1), StaticModelUv.Zero);
}
