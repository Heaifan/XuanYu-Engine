using System.Text.Json.Nodes;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.World;

public sealed class GlbImportTests
{
    readonly GlbImportService _service = new();

    [Fact]
    public void Imports_indexed_triangle_into_owned_static_model_data()
    {
        var result = Import(GlbFactory.Triangle());
        Assert.True(result.Succeeded);
        Assert.Equal(3, result.Model!.Vertices.Count);
        Assert.Equal([0u, 1u, 2u], result.Model.Indices);
        Assert.Single(result.Model.Primitives);
    }

    [Fact]
    public void Missing_uv_uses_default_uv_and_warning_once()
    {
        var result = Import(GlbFactory.Triangle(uvs: false));
        Assert.True(result.Succeeded);
        Assert.All(result.Model!.Vertices, v => Assert.Equal(StaticModelUv.Zero, v.Uv0));
        Assert.Single(result.Model.Warnings, x => x.Code == StaticModelImportWarningCode.MissingUvUsedDefault);
    }

    [Fact]
    public void Unindexed_triangle_gets_sequential_indices()
    {
        var result = Import(GlbFactory.Triangle(indices: false));
        Assert.True(result.Succeeded);
        Assert.Equal([0u, 1u, 2u], result.Model!.Indices);
    }

    [Fact]
    public void Supports_32_bit_indices_and_multiple_primitives()
    {
        var result = Import(GlbFactory.MultiPrimitive());
        Assert.True(result.Succeeded);
        Assert.Equal(6, result.Model!.Vertices.Count);
        Assert.Equal(2, result.Model.Primitives.Count);
        Assert.Equal(3, result.Model.Primitives[1].FirstIndex);
        Assert.Equal(new StaticModelColor(0, 1, 0, 1), result.Model.Primitives[1].BaseColorFactor);
    }

    [Fact]
    public void Converts_gltf_y_up_to_xuanyu_z_up_and_computes_bounds_after_node_translation()
    {
        var result = Import(GlbFactory.Triangle(translation: [1, 2, 3]));
        Assert.True(result.Succeeded);
        Assert.Equal(1, result.Model!.LocalBounds.Min.X);
        Assert.Equal(-3, result.Model.LocalBounds.Min.Y);
        Assert.Equal(2, result.Model.LocalBounds.Min.Z);
        Assert.Equal(3, result.Model.LocalBounds.Max.X);
        Assert.Equal(5, result.Model.LocalBounds.Max.Z);
    }

    [Fact]
    public void Unsupported_primitive_mode_without_triangles_fails()
    {
        var result = Import(GlbFactory.Triangle(mode: 0));
        Assert.False(result.Succeeded);
        Assert.Equal(StaticModelImportErrorCode.NoRenderablePrimitive, result.ErrorCode);
    }

    [Fact]
    public void Required_extension_fails_before_data_escape()
    {
        var root = new JsonObject { ["extensionsRequired"] = new JsonArray("KHR_draco_mesh_compression") };
        var result = Import(GlbFactory.Triangle(extraRoot: root));
        Assert.False(result.Succeeded);
        Assert.Equal(StaticModelImportErrorCode.UnsupportedRequiredExtension, result.ErrorCode);
    }

    [Fact]
    public void Invalid_header_returns_container_error()
    {
        var result = Import(GlbFactory.InvalidHeader());
        Assert.False(result.Succeeded);
        Assert.Equal(StaticModelImportErrorCode.InvalidGlbHeader, result.ErrorCode);
    }

    [Fact]
    public void Public_static_model_contract_does_not_expose_third_party_types()
    {
        var types = typeof(StaticModelData).Assembly.GetExportedTypes()
            .Where(t => t.Namespace == "XuanYu.Editor.Assets" && t.Name.StartsWith("StaticModel"));
        Assert.DoesNotContain(types.SelectMany(t => t.GetProperties()).Select(p => p.PropertyType.FullName),
            name => name?.StartsWith("SharpGLTF.") == true);
    }

    StaticModelImportResult Import(byte[] glb) => _service.ImportBytes(glb, "test.glb");
}
