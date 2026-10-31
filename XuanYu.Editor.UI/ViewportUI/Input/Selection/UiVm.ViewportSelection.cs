using XuanYu.Core.Picking;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void ApplyViewportSelection(ViewportPickingResult result)
    {
        if (!result.HasHit)
        {
            ApplySelectionCommand(new ClearEditorSelectionCommand(), "视口空白");
            return;
        }

        if (result.EntityKey is not { } entityKey ||
            !_sceneState.TryGetEntity(entityKey, out var entity))
        {
            ApplySelectionCommand(new ClearEditorSelectionCommand(), "视口失效命中");
            return;
        }
        ApplySelection("视口", new EditorTreeNode(
            entity.EntityKey.ToString(), entity.Name, entity.Type,
            $"MainWorld/{entity.EntityKey}", 1, "entity"));
    }
}
