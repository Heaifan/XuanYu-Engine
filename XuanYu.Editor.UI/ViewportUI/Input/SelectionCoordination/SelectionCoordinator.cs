using XuanYu.Core.Identity;
using XuanYu.Editor;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.Editor.UI;

internal sealed class SelectionCoordinator(
    EditorStateOwner selection,
    SceneStateOwner scene)
{
    public SelectionTransition Select(SelectEditorItemCommand command, EntityId? activeEntity)
    {
        var changed = selection.Select(command);
        var activeChanged = activeEntity is { } key && scene.SetActiveEntity(key, publish: false);
        return new(changed, activeChanged);
    }

    public EditorStateChangedResult? Clear(ClearEditorSelectionCommand command) =>
        selection.Clear(command);

    public bool IsEntityValid(EntityId key) => scene.TryGetEntity(key, out _);
}

internal sealed record SelectionTransition(
    EditorStateChangedResult? Changed,
    bool ActiveEntityChanged);
