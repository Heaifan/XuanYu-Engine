using XuanYu.Core.Picking;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void ApplyViewportSelection(ViewportPickingResult result)
    {
        var request = _selectionCoordinator.ResolveViewportSelection(result);
        if (request.Node is null)
            ApplySelectionCommand(new ClearEditorSelectionCommand(), request.Source);
        else ApplySelection(request.Source, request.Node);
    }
}
