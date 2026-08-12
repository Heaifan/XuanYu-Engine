using XuanYu.Editor.Layering;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    IEditorLayerProvider? _mapLayerProvider;
    IEditorLayerProvider? _regionLayerProvider;

    public IEditorLayerProvider? CurrentLayerProvider => IsRegionEditMode || IsRoadEditMode
        ? _regionLayerProvider ??= new EditorLayerProviderAdapter(this, true)
        : IsMapEditMode ? _mapLayerProvider ??= new EditorLayerProviderAdapter(this, false) : null;

    public IReadOnlyList<MapLayerRowViewModel> CurrentLayerItems => IsRegionEditMode || IsRoadEditMode
        ? _layerItems.Where(item => item.IsRegion).ToArray() : Array.Empty<MapLayerRowViewModel>();

    public bool HasCurrentLayerItems => CurrentLayerItems.Count > 0;
    public bool HasCurrentLayerSelection => IsEditMode && CurrentLayerItems.Contains(SelectedLayer);
    public string CurrentLayerEmptyTitle => CurrentLayerProvider?.EmptyStateTitle ?? "图层";
    public string CurrentLayerEmptyMessage => CurrentLayerProvider?.EmptyStateMessage ?? "当前编辑模式不显示图层栏";

    void RaiseLayerContextBindings()
    {
        OnPropertyChanged(nameof(CurrentLayerProvider));
        OnPropertyChanged(nameof(CurrentLayerItems));
        OnPropertyChanged(nameof(HasCurrentLayerItems));
        OnPropertyChanged(nameof(HasCurrentLayerSelection));
        OnPropertyChanged(nameof(CurrentLayerEmptyTitle));
        OnPropertyChanged(nameof(CurrentLayerEmptyMessage));
    }
}
