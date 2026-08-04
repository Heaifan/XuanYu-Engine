namespace XuanYu.Core.Tests.Render;

public sealed class StaticModelDepthRegressionTests
{
    [Fact]
    public void Background_shader_stays_at_far_depth()
    {
        var shader = File.ReadAllText(FindRepoFile("XuanYu.Render.Vulkan", "Shaders", "scene.vert"));

        Assert.Contains("clipPos = vec4(p[vi], 1.0, 1.0);", shader);
        Assert.DoesNotContain("clipPos = vec4(p[vi], 0.98, 1.0);", shader);
    }

    static string FindRepoFile(params string[] parts)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var path = Path.Combine([dir.FullName, .. parts]);
            if (File.Exists(path)) return path;
            dir = dir.Parent;
        }
        throw new FileNotFoundException(string.Join("/", parts));
    }
}
