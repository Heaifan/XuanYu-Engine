using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public void AddCubeEntity()
    {
        if (HasBlockingInput) return;
        var entity = _sceneState.AddCubeEntity();
        _historyOwner.PushEntry(new AddEntityHistoryEntry(entity));
        SelectEntity(entity.EntityKey, "添加立方体");
        FooterMessage = $"已添加立方体：{entity.Name}";
        RaiseDocumentChanged();
    }

    public bool DeleteSelectedEntity()
    {
        if (HasBlockingInput || !TrySelectedEntityKey(out var key) ||
            !_sceneState.TryGetEntity(key, out var entity)) return false;
        SceneStaticModelBinding? binding = _staticModelCatalog.TryGetByEntity(key, out var found) ? found : null;
        if (!_sceneState.DestroyEntity(key)) return false;
        if (binding is { } b) _staticModelCatalog.Remove(b.EntityId);
        _historyOwner.PushEntry(new DeleteEntityHistoryEntry(entity, binding));
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "删除实体");
        FooterMessage = $"已删除实体：{entity.Name}";
        RaiseDocumentChanged();
        return true;
    }

    public bool RenameSelectedEntity(string requestedName)
    {
        if (HasBlockingInput || !TrySelectedEntityKey(out var key) ||
            !_sceneState.TryGetEntity(key, out var before)) return false;
        if (!_sceneState.RenameEntity(key, requestedName, out var finalName)) return false;
        _historyOwner.PushEntry(new RenameEntityHistoryEntry(key, before.Name, finalName));
        SelectEntity(key, "重命名实体");
        FooterMessage = $"实体已重命名为：{finalName}";
        RaiseDocumentChanged();
        return true;
    }

    public bool BeginRenameSelectedEntity()
    {
        if (HasBlockingInput || _selectedHierarchyItem is not { IsEntity: true } node) return false;
        node.BeginRename();
        return true;
    }

    public bool BeginRenameFromShortcut() => BeginRenameSelectedEntity();

    public bool BeginRenameFromHierarchyContext() => BeginRenameSelectedEntity();

    public void CommitInlineRename(EditorTreeNode node)
    {
        if (!node.IsRenaming) return;
        var text = node.RenameText;
        node.EndRename();
        if (text.Trim().Length > 0) RenameSelectedEntity(text);
    }

    public void CancelInlineRename(EditorTreeNode node) => node.EndRename();

    void SelectEntity(EntityId key, string source)
    {
        var node = BuildHierarchyItems().FirstOrDefault(x => x.Key == key.ToString());
        if (node is null) return;
        _selectedProjectItem = null;
        OnPropertyChanged(nameof(SelectedProjectItem));
        SetHierarchySelection(node);
        FooterMessage = $"{source}：{node.Title}";
    }
}
