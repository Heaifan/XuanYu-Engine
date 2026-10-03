using XuanYu.Core.Scene;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;
using XuanYu.Editor.UI;
using Xunit.Abstractions;

namespace XuanYu.World.Tests.Render;

public sealed class WorldGridStartupContractTests
{
    readonly ITestOutputHelper _output;

    public WorldGridStartupContractTests(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Empty_scene_publishes_grid_to_runtime_and_command_plan()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var result = vm.RenderProjection;
        var queue = new LatestRenderProjectionQueue();
        queue.Publish(result);
        Assert.True(queue.TryConsume(out var consumed));
        var state = StartupGridState.From(consumed.Projection);
        _output.WriteLine($"StartupGridState: GridAvailable={state.GridAvailable}; " +
            $"GridDrawPlanned={state.GridDrawPlanned}; CameraValid={state.CameraValid}; " +
            $"WorldPlaneVisible={state.WorldPlaneVisible}");
        Assert.Equal(0, consumed.Projection.EntityCount);
        Assert.Contains(RenderDrawPlan.GetFrameDrawPlan(consumed.Projection),
            x => x.Kind == RenderDrawKind.EditorReferenceGrid);
        var draw = Read("XuanYu.Render.Vulkan", "Render", "Scene", "VulkanClearFrameOwner.Draw.cs");
        var dispatch = Read("XuanYu.Render.Vulkan", "Render", "Scene", "VulkanClearFrameOwner.DrawDispatch.cs");
        Assert.Contains("RenderDrawPlan.GetFrameDrawPlan(_renderProjection)", draw);
        Assert.Contains("DrawReferenceGrid(cb)", dispatch);
        Assert.True(state.GridAvailable && state.GridDrawPlanned && state.CameraValid &&
            state.WorldPlaneVisible);
    }

    [Theory]
    [InlineData(ProjectionMode.Perspective)]
    [InlineData(ProjectionMode.Orthographic)]
    public void Empty_scene_has_grid_for_both_projection_modes(ProjectionMode mode)
    {
        var camera = mode == ProjectionMode.Orthographic
            ? new CameraState(new(4, -5, 3), new(-4, 5, -3), Vector3d.UnitZ,
                60, .1, 100, 1, mode, 20)
            : DefaultEditorCamera.Create(1);
        var result = SceneRenderProjectionAdapter.TryCreate(
            SceneRenderSnapshot.Empty with { Camera = camera },
            assist: EditorViewportAssistState.Default);
        var state = StartupGridState.From(result.Projection);
        Assert.True(state.GridAvailable && state.GridDrawPlanned && state.CameraValid);
    }

    [Fact]
    public void Terrain_changes_do_not_activate_or_remove_existing_grid()
    {
        var empty = Projection(null);
        var terrain = Projection(new TerrainRenderResource("dem", 1,
            new TerrainHeightfield(2, 2, [1, 2, 3, 4])));
        var plans = new[] { empty, terrain, Projection(null), terrain }
            .Select(x => RenderDrawPlan.GetFrameDrawPlan(x));
        Assert.All(plans, plan => Assert.Contains(plan,
            x => x.Kind == RenderDrawKind.EditorReferenceGrid));
        Assert.All(plans, plan => Assert.DoesNotContain(plan,
            x => x.Kind == RenderDrawKind.Terrain && x.VertexCount == 0));
    }

    [Fact]
    public void Disabled_grid_stays_disabled_when_terrain_is_added()
    {
        var projection = Projection(new TerrainRenderResource("dem", 1,
            new TerrainHeightfield(2, 2, [1, 2, 3, 4])), showGrid: false);
        Assert.DoesNotContain(RenderDrawPlan.GetFrameDrawPlan(projection),
            x => x.Kind == RenderDrawKind.EditorReferenceGrid);
    }

    static RenderProjection Projection(TerrainRenderResource? terrain, bool showGrid = true) =>
        new(DefaultEditorCamera.Create(1).ToRenderProjection(), [], false, default,
            Assist: new EditorViewportAssistState(showGrid), Terrain: terrain);

    static string Read(params string[] parts)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var path = Path.Combine([dir.FullName, .. parts]);
            if (File.Exists(path)) return File.ReadAllText(path);
            dir = dir.Parent;
        }
        throw new FileNotFoundException(string.Join("/", parts));
    }
}
