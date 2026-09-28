namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool HasTransformableSelection =>
        TrySelectedEntityKey(out var key) && _sceneState.TryGetEntity(key, out _);

    public bool CanTransformSelectedEntity =>
        IsMapWorkspace && HasTransformableSelection;

    bool CanSelectTool(object? value)
    {
        var tool = EditorToolText.FromText(value?.ToString() ?? string.Empty);
        if (tool is EditorToolId.Move or EditorToolId.Rotate or EditorToolId.Scale)
            return CanTransformSelectedEntity;
        if (!CanUseEditTools) return false;
        return true;
    }
}
