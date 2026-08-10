namespace XuanYu.World.Tests.Render;

public sealed class MapGroundDiagnosticIsolationTests
{
    static string RenderVulkanFile(string path)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var full = Path.Combine(root, "XuanYu.Render.Vulkan", path);
        Assert.True(File.Exists(full), $"文件缺失：{full}");
        return File.ReadAllText(full);
    }

    [Fact]
    public void Map_ground_is_skipped_before_pipeline_binding_for_diagnosis()
    {
        var source = RenderVulkanFile(Path.Combine("Render", "Scene", "VulkanClearFrameOwner.Draw.cs"));
        var skip = source.IndexOf("if (draw.Kind == RenderDrawKind.MapGround) continue;", StringComparison.Ordinal);
        var bind = source.IndexOf("BindFramePipeline(cb, draw.Kind);", StringComparison.Ordinal);
        Assert.True(skip >= 0, "MapGround 必须在诊断轮中被跳过。");
        Assert.True(skip < bind, "MapGround 必须在绑定管线前跳过。");
    }
}
