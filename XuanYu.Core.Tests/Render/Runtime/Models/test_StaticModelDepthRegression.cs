namespace XuanYu.Core.Tests.Render;

public sealed class StaticModelDepthSourceContractTests
{
    [Fact]
    public void Background_shader_source_stays_at_far_depth()
    {
        var shader = File.ReadAllText(Path.Combine(TestSourceRoot.Root, "XuanYu.Render.Vulkan", "Shaders", "Sources", "SceneTerrain", "scene.vert"));

        Assert.Contains("clipPos = vec4(p[vi], 1.0, 1.0);", shader);
        Assert.DoesNotContain("clipPos = vec4(p[vi], 0.98, 1.0);", shader);
    }

}
