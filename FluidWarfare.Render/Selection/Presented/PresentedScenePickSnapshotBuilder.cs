using FluidWarfare.Render.Scene;

namespace FluidWarfare.Render.Selection.Presented;

/// <summary>
/// 从 RenderScene + CameraRevision 构建 PresentedScenePickSnapshot。
/// 在 Present 成功后调用，确保快照与用户看到的画面一致。
/// </summary>
public static class PresentedScenePickSnapshotBuilder
{
    public static PresentedScenePickSnapshot Build(
        RenderScene scene,
        long frameIndex,
        int cameraRevision,
        int viewportWidth,
        int viewportHeight)
    {
        var entities = new List<PresentedEntityBounds>(scene.Objects.Count);
        foreach (var obj in scene.Objects)
        {
            if (obj.VisualKind != RenderObjectVisualKind.UnitMarker) continue;
            if (obj.SelectionBounds is null) continue;

            entities.Add(new PresentedEntityBounds(
                obj.EntityId.Value,
                obj.SelectionBounds,
                ComputeViewDepth(obj.Placement),
                obj.Position.GetHashCode()));
        }

        return new PresentedScenePickSnapshot(
            frameIndex, cameraRevision,
            viewportWidth, viewportHeight,
            true, entities);
    }

    private static float ComputeViewDepth(Scene.RenderUnitPlacement? placement)
    {
        // 简单 Z 深度，具体深度由 Picking 时实时计算
        return placement is null ? 0f : (float)placement.VisualCenter.Z;
    }
}
