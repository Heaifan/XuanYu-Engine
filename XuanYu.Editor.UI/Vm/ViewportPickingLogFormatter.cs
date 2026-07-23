using XuanYu.Core.Picking;

namespace XuanYu.Editor.UI;

public static class ViewportPickingLogFormatter
{
    public static string Message(ViewportPickingResult result)
    {
        var value = result.EntityKey is { } key ? EditorDisplayText.Entity(key) : "未命中";
        return $"视口拾取完成；结果={value}";
    }

    public static string Detail(ViewportPickingResult result, double x, double y, double dpi)
    {
        var stats = result.Raycast.Stats;
        return $"请求序号={result.RequestSequence}；视口版本={result.ViewportRevision}；空间版本={result.SpatialRevision}；逻辑坐标=({x:F1},{y:F1})；DPI={dpi:F2}；候选={stats.CandidateCount}；精确检测={stats.NarrowPhaseTestCount}；真实命中={stats.HitCount}";
    }
}
