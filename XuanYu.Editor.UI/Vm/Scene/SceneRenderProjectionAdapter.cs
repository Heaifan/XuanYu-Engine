using XuanYu.Core.Identity;
using XuanYu.Core.Map;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Editor.Assets;
using XuanYu.Render.Abstractions;
using XuanYu.World;

namespace XuanYu.Editor.UI;

public static class SceneRenderProjectionAdapter
{
    public static RenderProjectionResult TryCreate(
        SceneRenderSnapshot snapshot,
        double rotateGizmoWorldRadius = 1.2,
        double scaleGizmoWorldAxisLength = 1.2,
        Vector3d gizmoRotation = default,
        EditorViewportAssistState assist = default,
        double moveGizmoWorldAxisLength = 1.2,
        SceneStaticModelCatalog? staticModelCatalog = null,
        IReadOnlyDictionary<AssetId, RenderStaticModelResource>? staticModelResources = null,
        MapRenderSnapshot map = default,
        double viewportDpiScale = 1.0,
        IReadOnlyList<RenderVectorOverlayResource>? vectorOverlays = null,
        ScaleIndicatorOverlayProjection scaleIndicator = default)
    {
        if (snapshot.Camera is not { } camera)
        {
            return RenderProjectionResult.Fail("Render Projection 缺少显式 Camera。");
        }

        var selectedKey = snapshot.IsSelected ? snapshot.Entity.EntityKey : EntityId.None;
        var entities = new List<RenderEntityProjection>(snapshot.Entities.Count);
        foreach (var e in snapshot.Entities)
        {
            var t = snapshot.TransformFor(e);
            if (e.Type == WorldEntityTypes.StaticModel)
            {
                if (staticModelCatalog is null ||
                    !staticModelCatalog.TryGetByEntity(e.EntityKey, out var binding) ||
                    staticModelResources is null ||
                    !staticModelResources.TryGetValue(binding.AssetId, out var resource))
                {
                    // 绑定缺失（导入事务中间帧 / 资产缺失）时跳过该实体，不让整帧投影失败。
                    // 持久缺失的诊断与恢复由场景加载事务负责（D4），D3 导入路径总是先建实体后绑定。
                    continue;
                }

                entities.Add(new RenderEntityProjection(e.EntityKey, t.Position, t.Rotation, t.Scale,
                    e.EntityKey == selectedKey, RenderEntityType.StaticModel, resource.Key));
                continue;
            }

            var type = e.Type == WorldEntityTypes.Cube
                ? RenderEntityType.Cube : RenderEntityType.LegacyMinimalTriangle;
            entities.Add(new RenderEntityProjection(e.EntityKey, t.Position, t.Rotation, t.Scale,
                e.EntityKey == selectedKey, type));
        }

        var projection = new RenderProjection(
            new RenderCameraProjection(
                camera.Position, camera.Forward, camera.Up,
                camera.VerticalFovDegrees, camera.NearPlane,
                camera.FarPlane, camera.Revision,
                camera.Mode, camera.OrthographicScale),
            entities,
            snapshot.ShowMoveGizmo,
            snapshot.RenderPosition,
            RotateGizmoVisible: snapshot.ShowRotateGizmo,
            RotateGizmoWorldRadius: rotateGizmoWorldRadius,
            ScaleGizmoVisible: snapshot.ShowScaleGizmo,
            ScaleGizmoWorldRadius: scaleGizmoWorldAxisLength,
            GizmoRotation: gizmoRotation,
            Assist: assist,
            MoveGizmoWorldRadius: moveGizmoWorldAxisLength,
            StaticModels: staticModelResources?.Values.OrderBy(r => r.Key.Value).ToArray(),
            VectorOverlays: vectorOverlays,
            Map: map,
            ViewportDpiScale: viewportDpiScale,
            ScaleIndicator: scaleIndicator);
        return RenderProjectionResult.Ok(projection);
    }
}
