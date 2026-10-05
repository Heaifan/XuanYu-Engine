using System.Collections.Immutable;
using XuanYu.Core.Results;
using XuanYu.Editor.Drawing;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed record RoadDrawingMetadata(
    MapLayerId LayerId, string DisplayName, string Kind, string DatasetId);

public sealed record RoadDrawingCommitResult(
    bool IsSuccess, MapRoad? Road, string? ErrorCode, string? ErrorMessage)
{
    public static RoadDrawingCommitResult Failed(string code, string message) =>
        new(false, null, code, message);
}

public sealed class RoadDrawingAdapter(MapEditSession mapSession)
{
    readonly MapEditSession _mapSession = mapSession;

    public RoadDrawingCommitResult Commit(
        DrawingCommitRequest request, RoadDrawingMetadata metadata)
    {
        if (request.PrimitiveKind != DrawingPrimitiveKind.Polyline)
            return RoadDrawingCommitResult.Failed("InvalidRoadDrawing", "道路绘制需要折线会话。");
        var points = request.Points.ToImmutableArray();
        var validation = DrawingValidation.Validate(request.PrimitiveKind, points);
        if (!validation.IsValid)
            return RoadDrawingCommitResult.Failed(validation.FailureCategory.ToString(), "道路节点无效。");
        var layer = MapLayerRules.Find(_mapSession.CurrentMap.Layers, metadata.LayerId);
        if (layer is not { Kind: MapLayerKind.Region })
            return RoadDrawingCommitResult.Failed("InvalidRoadLayer", "道路数据集图层无效。");
        var road = new MapRoad(MapRoadId.New(), metadata.LayerId, metadata.DisplayName,
            metadata.Kind, points);
        EngineResult result = _mapSession.CreateRoad(road);
        return result.IsSuccess ? new(true, road, null, null) :
            RoadDrawingCommitResult.Failed(result.Error?.Code ?? "RoadCommitFailed",
                result.Error?.Message ?? "道路创建失败。");
    }
}
