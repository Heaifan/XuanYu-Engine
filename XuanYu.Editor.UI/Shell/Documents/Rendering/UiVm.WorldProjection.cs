using XuanYu.Core.Identity;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    IReadOnlyList<EditorTreeNode> BuildHierarchyItems() => _projectTreePresenter.BuildHierarchy(_sceneState.Entities);

    bool TrySelectedEntityKey(out EntityId key)
    {
        key = EntityId.None;
        var text = SelectionKey;
        return TryEntityKey(text, out key);
    }

    bool TryEntityKey(string text, out EntityId key)
    {
        key = EntityId.None;
        const string prefix = "EntityId(";
        if (!text.StartsWith(prefix, StringComparison.Ordinal) || !text.EndsWith(')')) return false;
        var number = text.Substring(prefix.Length, text.Length - prefix.Length - 1);
        if (!int.TryParse(number, out var value) || value <= 0) return false;
        key = EntityId.FromInt(value);
        return true;
    }

    void RefreshWorldProjectionBindings()
    {
        TraceSelection("RefreshWorldProjectionBindings", 1,
            $"实体数={_sceneState.RenderSnapshot.Entities.Count}");
        ClearInvalidEntitySelection("世界实体失效");
        SynchronizeSelectionProjection();
        OnPropertyChanged(nameof(HierarchyItems));
        OnPropertyChanged(nameof(InspectorFields));
        OnPropertyChanged(nameof(DebugObjectItems));
        PublishSceneRenderSnapshot();
    }
}
