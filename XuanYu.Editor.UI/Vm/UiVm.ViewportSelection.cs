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

        var entity = _sceneState.RenderSnapshot.Entity;
        if (entity.EntityKey != result.EntityKey)
            throw new InvalidOperationException("Picking 结果与当前场景实体不一致。");

        ApplySelectionCommand(new SelectEditorItemCommand(
            "视口", entity.EntityKey.ToString(), entity.Name, entity.Type,
            $"MainWorld/{entity.EntityKey}"));
    }
}
