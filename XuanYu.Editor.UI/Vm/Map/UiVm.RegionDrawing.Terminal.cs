using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _terminatingRegionDrawing;

    void TerminateRegionDrawing(EditorPointerEventKind terminal, string? error = null)
    {
        if (_terminatingRegionDrawing) return;
        _terminatingRegionDrawing = true;
        try
        {
            ReleaseRegionPointerCapture(terminal);
            _regionDrawing.Cancel();
            ClearRegionDrawingSnap();
            RaiseRegionDrawingBindings();
            EndDrawingTransaction();
            if (IsRegionDrawingTool) SelectTool("选择", logTool: false);
            if (error is not null)
            {
                FooterState = "状态：错误";
                FooterMessage = error;
                NotifyError(error);
                LogRegionDrawingError(error);
            }
            PublishSceneRenderSnapshot();
        }
        finally { _terminatingRegionDrawing = false; }
    }
}
