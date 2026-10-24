using System.IO;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render.Grid;

public sealed class ReverseZWorldGridContractTests
{
    static string Root => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    static string Shader => File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Shaders", "editor_world_reference_grid.frag"));
    static string Pipeline => File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Grid.cs"));

    [Fact]
    public void GridShaderSourceDoesNotWriteFragmentDepth()
    {
        var shader = Shader;
        Assert.DoesNotContain("gl_FragDepth", shader);
        Assert.DoesNotContain("DEPTH_BIAS", shader);
        Assert.DoesNotContain("depth + bias", shader);
    }
    [Fact] public void GridNearFarConvention() => Assert.Contains("depth >= 0.0 && depth <= 1.0", Shader);
    [Fact] public void GridShaderSourceHasNoDistanceCutoffSymbol() => Assert.DoesNotContain("gridMaxDistance", Shader);
    [Fact] public void NoDistanceHardCutoff() => Assert.DoesNotContain("distToCamera", Shader);
    [Fact] public void PerspectivePersistence() => Assert.Contains("(-renderOrigin.z - nearWorld.z) / rayDirection.z", Shader);
    [Fact] public void OrthographicPersistence() => Assert.Contains("pc.gridState.y", Shader);

    [Fact]
    public void GridFriendlySeriesContinuity()
    {
        var levels = ReferenceGridScale.FromIdealSpacing(10_000_000.0);
        Assert.Equal(2.0, levels.CoarseSpacing / levels.FineSpacing);
    }

    [Fact]
    public void GridCrossFadeContinuity()
    {
        var levels = ReferenceGridScale.FromIdealSpacing(150.0);
        Assert.Equal(1.0, levels.FineWeight + levels.CoarseWeight, 6);
        Assert.True(levels.FineWeight > 0.0 && levels.CoarseWeight > 0.0);
    }

    [Fact] public void GridShaderSourceContainsFineContribution() => Assert.Contains("fineContribution", Shader);
    [Fact] public void GridShaderSourceContainsCoarseContribution() => Assert.Contains("coarseContribution", Shader);

    [Fact]
    public void GridPipelineSourceDisablesDepthTestWithoutFragmentWrite()
    {
        Assert.Contains("depthTest: false", Pipeline);
        Assert.Contains("ShaderBytecodeWorldReferenceGridFrag.Code", Pipeline);
    }
}
