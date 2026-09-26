using XuanYu.Editor.MapDocument;
using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public enum InspectorObjectKind { Empty, Map, Dataset, Terrain, Marker, Road, Region, Entity }

public sealed partial class UiVm
{
    public InspectorObjectKind InspectorIdentity => ResolveInspectorSelection();
    public bool IsMapInspector => InspectorIdentity == InspectorObjectKind.Map;
    public bool IsDatasetInspector => InspectorIdentity == InspectorObjectKind.Dataset;
    public bool IsTerrainInspectorKind => InspectorIdentity == InspectorObjectKind.Terrain;
    public bool IsRoadInspector => InspectorIdentity == InspectorObjectKind.Road;
    public bool IsRegionInspector => InspectorIdentity == InspectorObjectKind.Region;
    public bool IsFeatureInspector => IsRoadInspector || IsRegionInspector;

    public InspectorEditTarget CreateInspectorEditTarget(string propertyKey) =>
        new(InspectorIdentity, InspectorObjectId(), propertyKey);

    string InspectorObjectId() => InspectorIdentity switch
    {
        InspectorObjectKind.Entity => SelectionKey,
        InspectorObjectKind.Dataset => SelectedDataset?.Id ?? "",
        InspectorObjectKind.Terrain => SelectedDataset?.Id ?? "",
        InspectorObjectKind.Marker or InspectorObjectKind.Road or InspectorObjectKind.Region =>
            _selectedMapGeometry?.FeatureId ?? "",
        _ => ""
    };

    InspectorObjectKind ResolveInspectorSelection()
    {
        if (_selectedMapGeometry is { Kind: MapGeometryFeatureKind.Marker }) return InspectorObjectKind.Marker;
        if (_selectedMapGeometry is { Kind: MapGeometryFeatureKind.Road }) return InspectorObjectKind.Road;
        if (_selectedMapGeometry is { Kind: MapGeometryFeatureKind.Region }) return InspectorObjectKind.Region;
        if (IsEntityInspector) return InspectorObjectKind.Entity;
        if (SelectedDataset is { Type: MapDatasetTypes.TerrainArea } && _terrainInspectorMetadata is not null)
            return InspectorObjectKind.Terrain;
        if (SelectedDataset is not null) return InspectorObjectKind.Dataset;
        if (MapSession.Selection.Kind == MapSelectionKind.None) return InspectorObjectKind.Empty;
        if (MapSession.Selection.Kind == MapSelectionKind.Map) return InspectorObjectKind.Map;
        if (MapSession.Selection.Kind == MapSelectionKind.Region) return InspectorObjectKind.Region;
        return InspectorObjectKind.Empty;
    }

    void RaiseInspectorSelectionBindings()
    {
        ResetInspectorSectionsIfIdentityChanged();
        OnPropertyChanged(nameof(InspectorIdentity)); OnPropertyChanged(nameof(IsMapInspector)); OnPropertyChanged(nameof(IsDatasetInspector));
        OnPropertyChanged(nameof(IsTerrainInspectorKind)); OnPropertyChanged(nameof(IsTerrainInspector));
        OnPropertyChanged(nameof(IsMarkerInspector)); OnPropertyChanged(nameof(IsRoadInspector));
        OnPropertyChanged(nameof(IsRegionInspector)); OnPropertyChanged(nameof(IsFeatureInspector)); OnPropertyChanged(nameof(IsInspectorEmpty));
        OnPropertyChanged(nameof(HasInspectorSelection)); OnPropertyChanged(nameof(InspectorSelectionTitle));
        OnPropertyChanged(nameof(InspectorSelectionSubtitle)); OnPropertyChanged(nameof(InspectorSectionTitle));
        OnPropertyChanged(nameof(IsMapWorkspaceInspectorVisible)); OnPropertyChanged(nameof(IsLayerInspectorVisible));
        OnPropertyChanged(nameof(IsGenericInspectorVisible)); OnPropertyChanged(nameof(IsRegionInspectorPlaceholderVisible));
        RefreshInspectorNavigation();
        RaiseDiagnosticIdentityBindings();
    }
}
