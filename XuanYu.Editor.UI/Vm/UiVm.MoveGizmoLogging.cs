using XuanYu.Core.Gizmo;
using XuanYu.Core.Scene;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void LogTransformCaptureRejected(string activeTool, string entity)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "【WORLD-A-R0-R2】变换捕获拒绝",
            $"ActiveTool={activeTool}; Entity={entity}; 原因=当前工具未实现真实捕获");
        RefreshLogBindings();
    }

    void LogTransformCaptureBegin(string activeTool, string sessionTool, string entity, MoveGizmoAxis axis)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "【WORLD-A-R0-R2】变换捕获开始",
            $"ActiveTool={activeTool}; SessionTool={sessionTool}; Entity={entity}; Axis={axis}");
        RefreshLogBindings();
    }

    void LogTransformCaptureCommit(EditorInteractionSnapshot snap, SceneTransformCommitResult commit)
    {
        if (snap.OwnerTool != "移动" || !commit.Changed) return;
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "【WORLD-A-R0-R2】变换捕获提交",
            $"SessionTool={snap.OwnerTool}; Entity={commit.EntityKey}; Before={commit.Before.Position}; After={commit.After.Position}");
        RefreshLogBindings();
    }

    void LogTransformCaptureCancel(string reason, EditorInteractionSnapshot snap)
    {
        if (snap.OwnerTool != "移动") return;
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "【WORLD-A-R0-R2】变换捕获取消",
            $"SessionTool={snap.OwnerTool}; Session={snap.SessionId}; 原因={reason}");
        RefreshLogBindings();
    }

    void LogMoveGizmoEnd(string result, EditorInteractionSnapshot snap)
    {
        if (snap.OwnerTool != "移动" || !snap.StartSnapshot.StartsWith("Entity=", StringComparison.Ordinal)) return;
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            $"【ARCH-C-R5】移动工具会话{result}",
            $"{snap.StartSnapshot}; Session={snap.SessionId}; Position={_sceneState.RenderSnapshot.Entity.Transform.Position}");
        RefreshLogBindings();
    }
}
