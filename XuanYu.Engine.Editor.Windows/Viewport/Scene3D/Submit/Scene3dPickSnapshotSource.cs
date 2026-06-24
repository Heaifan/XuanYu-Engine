using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Selection.Presented;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;

namespace XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Submit;

/// <summary>Pick Snapshot 构建。封装 PresentedScenePickSnapshotBuilder.Build。</summary>
public static class Scene3dPickSnapshotSource
{
    public static PresentedScenePickSnapshot Build(
        RenderScene renderScene, int renderSeq, int cameraRevision,
        PresentedMoveGizmoSnapshot presentedGizmo)
    {
        return PresentedScenePickSnapshotBuilder.Build(
            renderScene, renderSeq, cameraRevision,
            presentedGizmo.ViewportWidth, presentedGizmo.ViewportHeight);
    }
}
