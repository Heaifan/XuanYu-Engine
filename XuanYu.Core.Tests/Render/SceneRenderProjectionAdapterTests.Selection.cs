using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using Xunit;

namespace XuanYu.Core.Tests.Render;

public sealed partial class SceneRenderProjectionAdapterTests
{
    // R4-R3：轮廓高亮目标必须等价于“当前选中实体”，且与工具/层级树来源无关。
    // 以下断言直接验证渲染层消费的高亮目标（RenderEntityProjection.IsSelected）。

    [Fact]
    public void Selected_entity_is_marked_IsSelected_true_and_others_false()
    {
        var a = EntityAt(1, Vector3d.Zero);
        var b = EntityAt(2, new Vector3d(5, 0, 0));
        var snapshot = new SceneRenderSnapshot(
            a, IsSelected: true, RenderEntities: [a, b], Camera: TestCamera());

        var projection = SceneRenderProjectionAdapter.TryCreate(snapshot).Projection;

        Assert.True(projection.Entities.Single(e => e.Key == EntityId.FromInt(1)).IsSelected);
        Assert.False(projection.Entities.Single(e => e.Key == EntityId.FromInt(2)).IsSelected);
    }

    [Fact]
    public void Switching_selection_to_B_marks_only_B()
    {
        var a = EntityAt(1, Vector3d.Zero);
        var b = EntityAt(2, new Vector3d(5, 0, 0));
        // 选中实体切换为 B（旋转工具下点击 B、或层级树点 B 都走同一选择源）。
        var snapshot = new SceneRenderSnapshot(
            b, IsSelected: true, RenderEntities: [a, b], Camera: TestCamera());

        var projection = SceneRenderProjectionAdapter.TryCreate(snapshot).Projection;

        Assert.False(projection.Entities.Single(e => e.Key == EntityId.FromInt(1)).IsSelected);
        Assert.True(projection.Entities.Single(e => e.Key == EntityId.FromInt(2)).IsSelected);
    }

    [Fact]
    public void No_selection_marks_all_IsSelected_false()
    {
        var a = EntityAt(1, Vector3d.Zero);
        var b = EntityAt(2, new Vector3d(5, 0, 0));
        // 空白取消选择：HasSelection=false → selected=false，所有实体均不高亮。
        var snapshot = new SceneRenderSnapshot(
            a, IsSelected: false, RenderEntities: [a, b], Camera: TestCamera());

        var projection = SceneRenderProjectionAdapter.TryCreate(snapshot).Projection;

        Assert.All(projection.Entities, e => Assert.False(e.IsSelected));
    }
}
