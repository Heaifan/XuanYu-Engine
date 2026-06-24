using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Presentation;

namespace XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Submit;

/// <summary>Gizmo 帧数据提交。执行 MoveGizmoFrameSource.Build + SetMoveGizmoVertices。
/// 是唯一被允许调用 SetMoveGizmoVertices 的模块。</summary>
public static class Scene3dGizmoSubmitSource
{
    public static MoveGizmoFrameResult Build(MoveGizmoFrameInput input, VulkanScene3dSession session)
    {
        var result = MoveGizmoFrameSource.Build(input);
        session.SetMoveGizmoVertices(result.IsEmpty ? null : result.Vertices);
        return result;
    }
}
