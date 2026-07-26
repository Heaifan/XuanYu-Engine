namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool HasMoveCapture => _editorState.InteractionSnapshot.HasCapture;

    bool HasBlockingInput => HasMoveCapture || IsCameraNavigationActive;

    bool CanChangeToolNow(string name)
    {
        if (!HasBlockingInput) return true;
        FooterState = "状态：捕获中";
        FooterMessage = $"当前输入会话未结束，不能切换到{name}。";
        return false;
    }

    bool CanPickViewportSelection() =>
        IsSelectTool && !HasMoveCapture && !IsCameraNavigationActive;

    bool CanBeginMoveInteraction() =>
        IsMoveTool && HasSelection && !IsCameraNavigationActive;
}
