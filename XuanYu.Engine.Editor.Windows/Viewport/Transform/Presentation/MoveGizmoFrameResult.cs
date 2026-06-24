using XuanYu.Engine.Render.Vulkan.Scene3D.Overlay;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Presentation;

/// <summary>MoveGizmoFrameSource.Build 的纯输出。Vulkan 副作用由调用层执行。</summary>
public readonly record struct MoveGizmoFrameResult
{
    public VulkanOverlayVertex[] Vertices { get; }
    public PresentedMoveGizmoSnapshot PendingSnapshot { get; }
    public bool IsEmpty => Vertices.Length == 0;

    public MoveGizmoFrameResult(VulkanOverlayVertex[] vertices, PresentedMoveGizmoSnapshot pending)
    {
        Vertices = vertices; PendingSnapshot = pending;
    }

    public static readonly MoveGizmoFrameResult Empty = new([], PresentedMoveGizmoSnapshot.None);
}
