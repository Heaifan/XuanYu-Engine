using XuanYu.Core.Gizmo;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;

namespace XuanYu.Editor.Transform;

public sealed partial class TransformSession
{
    public ScaleGizmoHandle? ScaleHandle { get; private set; }

    // 缩放起始：与 Begin（移动）/ BeginRotate（旋转）互斥，复用同一会话生命周期与提交/取消路径。
    // 预览走既有 TryPreviewScale；提交/取消走既有 TryCommit/TryCancel（与轴类型无关）。
    public bool BeginScale(long sessionId, SceneEntitySnapshot entity, ScaleGizmoHandle handle)
    {
        if (sessionId <= 0) throw new ArgumentOutOfRangeException(nameof(sessionId));
        if (!entity.IsValid) return false;
        if (IsActive) return false;
        IsActive = true;
        SessionId = sessionId;
        ScaleHandle = handle;
        StartSnapshot = new TransformStartSnapshot(entity.EntityKey, entity.Transform);
        Preview = new PreviewTransform(entity.Transform);
        return true;
    }
}
