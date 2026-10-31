using XuanYu.Editor.Layering;

namespace XuanYu.Editor.UI;

sealed class EditorLayerProviderAdapter : IEditorLayerProvider
{
    readonly UiVm _owner;
    readonly bool _region;

    public EditorLayerProviderAdapter(UiVm owner, bool region) { _owner = owner; _region = region; }

    public IReadOnlyList<EditorLayerItem> Items => (_region
        ? _owner.LayerItems.Where(x => x.IsRegion)
        : _owner.LayerItems).Select(ToItem).ToArray();

    public string EmptyStateTitle => "图层";
    public string EmptyStateMessage => _region ? "当前没有可用区域图层" : "当前没有可用图层";

    public EditorLayerCommandResult Add() => Execute(_owner.AddLayer);
    public EditorLayerCommandResult Delete(string id) => SelectAnd(id, _owner.DeleteLayer);
    public EditorLayerCommandResult Rename(string id, string name) => SelectAnd(id, () => _owner.CommitLayerRename(name));
    public EditorLayerCommandResult SetVisible(string id, bool visible) => SelectAnd(id, () => _owner.SetLayerVisibility(Parse(id), visible));
    public EditorLayerCommandResult SetLocked(string id, bool locked) => SelectAnd(id, () => _owner.SetLayerLock(Parse(id), locked));
    public EditorLayerCommandResult SetActive(string id) => SelectAnd(id, _owner.SetActiveLayer);
    public EditorLayerCommandResult Move(string id, int targetIndex) => SelectAnd(id, () => _owner.CommitLayerDrag(id, targetIndex));

    EditorLayerItem ToItem(MapLayerRowViewModel row, int index) => new(
        row.LayerId.Value, row.Name, row.KindTagText, index, row.IsVisible, row.IsLocked,
        row.IsActive, !row.IsSystem, row.IsRegion && _owner.UserLayerCount > 1, row.IsDragEnabled);

    EditorLayerCommandResult SelectAnd(string id, Action action)
    {
        var row = _owner.LayerItems.FirstOrDefault(x => x.LayerId.Value == id && (!_region || x.IsRegion));
        if (row is null) return EditorLayerCommandResult.Failure("图层不存在。");
        _owner.SelectedLayer = row; action(); return EditorLayerCommandResult.Success();
    }

    static EditorLayerCommandResult Execute(Action action) { action(); return EditorLayerCommandResult.Success(); }
    static XuanYu.World.Map.MapLayerId Parse(string id) => XuanYu.World.Map.MapLayerId.TryParse(id, out var value) ? value : XuanYu.World.Map.MapLayerId.New();
}
