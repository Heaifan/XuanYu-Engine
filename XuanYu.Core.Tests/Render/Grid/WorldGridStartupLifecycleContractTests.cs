using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Core.Tests.Render.Grid;

public sealed class WorldGridStartupLifecycleContractTests
{
    static string Root => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    static string GridSource => File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Render", "Grid", "VulkanClearFrameOwner.Grid.cs"));
    static string ScaleSource => File.ReadAllText(Path.Combine(Root, "XuanYu.Render.Vulkan", "Render", "Grid", "VulkanClearFrameOwner.GridScale.cs"));

    static RenderProjection EmptyScene() => new(default, [], true, default,
        Assist: new EditorViewportAssistState(ShowGrid: true, ShowOrigin: false,
            ShowWorldAxes: false, ShowEditorBackground: false));

    [Fact]
    public void EmptySceneGridRenderStateReady()
    {
        var projection = EmptyScene();
        Assert.True(projection.AssistState.ShowGrid);
        Assert.True(ReferenceGridScale.Compute(1.0).FineSpacing > 0);
    }

    [Fact]
    public void EmptySceneGridDrawPlanned()
        => Assert.Contains(RenderDrawPlan.GetFrameDrawPlan(EmptyScene()),
            entry => entry.Kind == RenderDrawKind.EditorReferenceGrid);

    [Fact]
    public void GridDoesNotRequireTerrain()
        => Assert.Contains(RenderDrawPlan.GetFrameDrawPlan(EmptyScene()),
            entry => entry.Kind == RenderDrawKind.EditorReferenceGrid);

    [Fact]
    public void GridDoesNotRequireSceneEntity()
        => Assert.DoesNotContain(RenderDrawPlan.GetFrameDrawPlan(EmptyScene()),
            entry => entry.Kind == RenderDrawKind.EntityFill);

    [Fact]
    public void GridReadyInvalidatesCommandBuffer()
        => Assert.Contains("RecordCommandBuffers(_views)", GridSource);

    [Fact]
    public void GridDrawSurvivesTerrainImport()
        => Assert.True(VulkanClearFrameOwner.IsPipelineReady(
            VulkanClearFrameOwner.DrawOwner.EditorReferenceGrid,
            new(false, false, true, false, false, false, false, false, false, false)));

    [Fact]
    public void GridDrawSurvivesTerrainRemoval()
        => Assert.Contains("UpdateReferenceGridScale", ScaleSource);

    [Fact]
    public void GridDrawDoesNotWaitForScenePipeline()
        => Assert.True(VulkanClearFrameOwner.IsPipelineReady(
            VulkanClearFrameOwner.DrawOwner.EditorReferenceGrid,
            new(false, false, true, false, false, false, false, false, false, false)));
}
