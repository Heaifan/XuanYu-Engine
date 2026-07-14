namespace XuanYu.Editor.UI;

public sealed partial class EditorStateOwner
{
    EditorToolSnapshot _toolSnapshot = EditorToolSnapshot.Initial;

    public EditorToolSnapshot ToolSnapshot => _toolSnapshot;

    public EditorToolChangedResult? ChangeTool(ChangeEditorToolCommand command)
    {
        EnsureWriteThread();
        var tool = EditorToolText.FromText(command.ToolText);
        var old = _toolSnapshot;
        if (old.ActiveTool == tool && old.CaptureState == EditorToolCaptureState.None)
        {
            return null;
        }

        _toolSnapshot = new EditorToolSnapshot(
            old.Revision + 1,
            tool,
            EditorToolCaptureState.None);
        return new EditorToolChangedResult(
            old.Revision,
            _toolSnapshot.Revision,
            old,
            _toolSnapshot);
    }
}
