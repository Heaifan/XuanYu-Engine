using XuanYu.Core.Gizmo;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;

namespace XuanYu.Editor.Transform;

public sealed partial class TransformSession
{
    public RotateGizmoAxis? RotateAxis { get; private set; }

    // 旋转起始：与 Begin（移动）互斥，复用同一会话生命周期与提交/取消路径。
    public bool BeginRotate(long sessionId, SceneEntitySnapshot entity, RotateGizmoAxis axis)
    {
        if (sessionId <= 0) throw new ArgumentOutOfRangeException(nameof(sessionId));
        if (!entity.IsValid) return false;
        if (IsActive) return false;
        IsActive = true;
        SessionId = sessionId;
        RotateAxis = axis;
        StartSnapshot = new TransformStartSnapshot(entity.EntityKey, entity.Transform);
        Preview = new PreviewTransform(entity.Transform);
        return true;
    }
}
