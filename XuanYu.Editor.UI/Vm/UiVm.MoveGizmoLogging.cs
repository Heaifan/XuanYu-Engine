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
        if (snap.OwnerTool != "移动" || !commit.Changed) return;
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "变换捕获提交",
            $"会话工具={snap.OwnerTool}；实体={EditorDisplayText.Entity(commit.EntityKey)}；之前位置={EditorDisplayText.Position(commit.Before.Position)}；之后位置={EditorDisplayText.Position(commit.After.Position)}");
        RefreshLogBindings();
    }

    void LogTransformCaptureCancel(string reason, EditorInteractionSnapshot snap)
    {
        if (snap.OwnerTool != "移动") return;
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "变换捕获取消",
            $"会话工具={snap.OwnerTool}；会话={snap.SessionId}；原因={reason}");
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
