using System.Windows.Input;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    ICommand? _addSelectedRoadVertexCommand;
    ICommand? _deleteSelectedRoadVertexCommand;

    public bool IsRoadGeometrySelected => _selectedMapGeometry?.Kind == MapGeometryFeatureKind.Road;
    public bool CanAddSelectedRoadVertex => IsRoadGeometrySelected && _selectedMapGeometryVertexIndex >= 0;
    public bool CanDeleteSelectedRoadVertex => CanAddSelectedRoadVertex && RoadPoints().Length > 2;
    public ICommand AddSelectedRoadVertexCommand => _addSelectedRoadVertexCommand ??=
        new RelayCommand(_ => AddSelectedRoadVertex());
    public ICommand DeleteSelectedRoadVertexCommand => _deleteSelectedRoadVertexCommand ??=
        new RelayCommand(_ => DeleteSelectedRoadVertex());

    void AddSelectedRoadVertex()
    {
        if (!CanAddSelectedRoadVertex) return;
        var points = RoadPoints();
        var segment = _selectedMapGeometryVertexIndex < points.Length - 1
            ? _selectedMapGeometryVertexIndex : points.Length - 2;
        var point = new MapPoint((points[segment].X + points[segment + 1].X) / 2,
            (points[segment].Y + points[segment + 1].Y) / 2);
        if (!MapRoadId.TryParse(_selectedMapGeometry!.Value.FeatureId, out var roadId)) return;
        if (MapSession.InsertRoadVertex(roadId, segment, point).IsSuccess)
        {
            _selectedMapGeometryVertexIndex = segment + 1;
            RefreshMapGeometryDisplay();
        }
    }

    void DeleteSelectedRoadVertex()
    {
        if (!CanDeleteSelectedRoadVertex || !MapRoadId.TryParse(_selectedMapGeometry!.Value.FeatureId, out var roadId)) return;
        if (MapSession.DeleteRoadVertex(roadId, _selectedMapGeometryVertexIndex).IsSuccess)
        {
            _selectedMapGeometryVertexIndex = Math.Min(_selectedMapGeometryVertexIndex, RoadPoints().Length - 1);
            RefreshMapGeometryDisplay();
        }
    }

    System.Collections.Immutable.ImmutableArray<MapPoint> RoadPoints() =>
        MapSession.CurrentMap.Roads.First(road => road.RoadId.ToString() == _selectedMapGeometry!.Value.FeatureId).Points;
}
