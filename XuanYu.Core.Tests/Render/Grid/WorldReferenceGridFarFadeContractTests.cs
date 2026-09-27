using System.IO;

namespace XuanYu.Core.Tests.Render;

public sealed class WorldReferenceGridFarFadeContractTests
{
    static string Root => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    static string Shader() => File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Shaders", "editor_world_reference_grid.frag"));
    static string Bytecode() => File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Pipeline", "ShaderBytecode.WorldReferenceGridFrag.cs"));

    [Fact]
    public void World_grid_has_independent_density_fades_for_fine_and_coarse_lines()
    {
        var shader = Shader();
        Assert.Contains("densityFade", shader);
        Assert.Contains("fineDensity", shader);
        Assert.Contains("coarseDensity", shader);
        Assert.Contains("smoothstep(6.0, 12.0", shader);
        Assert.Contains("return max(xLine, yLine) * density", shader);
        Assert.Contains("float fineLine = gridLine", shader);
        Assert.DoesNotContain("fineLine = axisLineMask", shader);
    }

    [Fact]
    public void World_grid_keeps_real_depth_without_distance_cutoff()
    {
        var shader = Shader();
        Assert.DoesNotContain("distToCamera", shader);
        Assert.DoesNotContain("gridMaxDistance", shader);
        Assert.DoesNotContain("distanceFade", shader);
        Assert.Contains("clipPosition", shader);
        Assert.Contains("gl_FragDepth", shader);
        Assert.Contains("depth >= 0.0 && depth <= 1.0", shader);
    }

    [Fact]
    public void World_grid_bytecode_is_marked_as_far_view_b_shader_output()
    {
        Assert.Contains("FAR-VIEW-B", Shader());
        Assert.Contains("FAR-VIEW-B", Bytecode());
        Assert.Contains("0x07230203u", Bytecode());
    }
}
