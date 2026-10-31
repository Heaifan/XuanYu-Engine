using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void BeginInteraction()
    {
        if (!CanBeginMoveInteraction()) return;
        var result = BeginEditorInteraction(new BeginInteractionCommand(ActiveTool,
            SelectionTitle, EditorInteractionPointerSnapshot.Empty));
        if (result is null) return;
        FooterState = "状态：捕获中";
        FooterMessage = $"交互开始：{ActiveTool}";
        LogInteraction("开始捕获", $"Session={result.Snapshot.SessionId}");
        RaiseInteractionChanged();
    }

    EditorInteractionChangedResult? BeginEditorInteraction(
        BeginInteractionCommand command)
    {
        var result = _editorState.Begin(command);
        if (result is not null) ViewportInput.BeginInteractionEpoch();
        return result;
    }
}
