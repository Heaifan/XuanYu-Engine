using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;
using XuanYu.Engine.Editor.Windows.Shell.Feedback;

namespace XuanYu.Engine.Editor.Windows.Shell.Diagnostics.Log;

/// <summary>日志路由。负责日志委托和 Diagnostics 薄转发。</summary>
sealed class EditorShellLogRoute(
    EditorFeedbackRoute feedback,
    EditorDiagnosticsRefreshRoute diagnosticsRoute,
    Func<bool> isSessionActive,
    Func<string> getRenderLastMode)
{
    public void Info(string message) => feedback.Info(message);
    public void Warn(string message) => feedback.Warn(message);
    public void Error(string message) => feedback.Error(message);

    public void RefreshDiagnostics() =>
        diagnosticsRoute.Refresh(isSessionActive(), getRenderLastMode());

    public void UpdateVulkanViewportHost() =>
        diagnosticsRoute.UpdateViewportHost();
}
