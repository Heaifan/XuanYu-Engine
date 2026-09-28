namespace XuanYu.World.Tests.Render;

public sealed class TerrainPreviewLightingContractTests
{
    [Fact]
    public void Terrain_fragment_shader_uses_fixed_editor_lambert_lighting()
    {
        var shader = File.ReadAllText(FindShader("terrain.frag"));

        Assert.Contains("normalize(inNormal)", shader);
        Assert.Contains("max(dot(normalize(inNormal), normalize(vec3(0.35, 0.55, 0.75))), 0.0)", shader);
        Assert.Contains("0.45 + 0.55 *", shader);
        Assert.Contains("vec3(0.24, 0.48, 0.32)", shader);
        Assert.DoesNotContain("camera", shader, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("viewProjection", shader, StringComparison.Ordinal);
    }

    static string FindShader(string name)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine(directory.FullName, "XuanYu.Render.Vulkan", "Shaders", name);
            if (File.Exists(path)) return path;
            directory = directory.Parent;
        }
        throw new FileNotFoundException(name);
    }
}
