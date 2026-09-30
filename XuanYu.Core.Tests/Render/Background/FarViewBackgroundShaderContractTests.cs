using System.IO;

namespace XuanYu.Core.Tests.Render.Background;

public sealed class FarViewBackgroundShaderSourceContractTests
{
    private static string Root => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    private static string Shader() => File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Shaders", "scene.frag"));
    private static string Bytecode() => File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Pipeline", "ShaderBytecode.Frag.cs"));

    [Fact]
    public void Background_shader_source_does_not_define_fake_ground_layers()
    {
        var shader = Shader();

        Assert.DoesNotContain("groundFar", shader);
        Assert.DoesNotContain("groundNear", shader);
        Assert.DoesNotContain("groundColor", shader);
        Assert.DoesNotContain("groundNearness", shader);
        Assert.DoesNotContain("参考地面", shader);
    }

    [Fact]
    public void Background_shader_source_reconstructs_near_and_far_from_same_ndc_xy()
    {
        var shader = Shader();

        Assert.Contains("vec4 nearWorld = vInvViewProjection * vec4(vBackgroundNdc, 1.0, 1.0);", shader);
        Assert.Contains("vec4 farWorld = vInvViewProjection * vec4(vBackgroundNdc, 0.0, 1.0);", shader);
        Assert.Contains("dir = normalize(far - near);", shader);
        Assert.DoesNotContain("vec4 camWorld", shader);
        Assert.DoesNotContain("vec4(0.0, 0.0, 0.0, 1.0)", shader);
    }

    [Fact]
    public void Background_shader_source_guards_homogeneous_w_and_finite_values()
    {
        var shader = Shader();

        Assert.Contains("abs(nearWorld.w)", shader);
        Assert.Contains("abs(farWorld.w)", shader);
        Assert.Contains("isFinite", shader);
        Assert.Contains("safeDirectionFallback", shader);
    }

    [Fact]
    public void Embedded_background_bytecode_source_marker_matches_shader_source()
    {
        var shader = Shader();
        var bytecode = Bytecode();

        Assert.Contains("FAR-VIEW-A", shader);
        Assert.Contains("FAR-VIEW-A", bytecode);
        Assert.Contains("0x07230203u", bytecode);
        Assert.DoesNotContain("groundFar", bytecode);
        Assert.DoesNotContain("groundNear", bytecode);
    }
}
