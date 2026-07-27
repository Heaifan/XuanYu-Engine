using XuanYu.Core.Scene;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void LogTransformCaptureRejected(string activeTool, string entity)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "变换捕获已拒绝",
            $"当前工具={activeTool}；实体={entity}；原因=当前工具未实现真实捕获");
        RefreshLogBindings();
    }

    void LogTransformCaptureBegin(string activeTool, string sessionTool, string entity, object axis)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "变换捕获开始",
            $"当前工具={activeTool}；会话工具={sessionTool}；实体={entity}；轴={axis}");
        RefreshLogBindings();
    }

    void LogTransformCaptureCommit(EditorInteractionSnapshot snap, SceneTransformCommitResult commit)
    {
        if (!commit.Changed) return;
        var key = EditorDisplayText.Entity(commit.EntityKey);
        if (snap.OwnerTool == "移动")
        {
            _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
                "变换捕获提交",
                $"会话工具=移动；实体={key}；之前位置={EditorDisplayText.Position(commit.Before.Position)}；之后位置={EditorDisplayText.Position(commit.After.Position)}");
        }
        else
        {
            _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
                "变换捕获提交",
                $"会话工具={snap.OwnerTool}；实体={key}；之前旋转=({commit.Before.Rotation.X:g},{commit.Before.Rotation.Y:g},{commit.Before.Rotation.Z:g})；之后旋转=({commit.After.Rotation.X:g},{commit.After.Rotation.Y:g},{commit.After.Rotation.Z:g})");
        }
        RefreshLogBindings();
    }

    void LogTransformCaptureCancel(string reason, EditorInteractionSnapshot snap)
    {
        if (!snap.HasCapture) return;
        var key = EditorDisplayText.Entity(_sceneState.RenderSnapshot.Entity.EntityKey);
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "变换捕获取消",
            $"会话工具={snap.OwnerTool}；实体={key}；会话={snap.SessionId}；原因={reason}");
        RefreshLogBindings();
    }

    void LogMoveGizmoEnd(string result, EditorInteractionSnapshot snap)
    {
        if (snap.OwnerTool != "移动" || !snap.StartSnapshot.StartsWith("实体=", StringComparison.Ordinal)) return;
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            $"移动工具会话{result}",
            $"{snap.StartSnapshot}；会话={snap.SessionId}；当前位置={EditorDisplayText.Position(_sceneState.RenderSnapshot.Entity.Transform.Position)}");
        RefreshLogBindings();
    }
}
