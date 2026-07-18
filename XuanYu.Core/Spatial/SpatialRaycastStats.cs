namespace XuanYu.Core.Spatial;

public readonly record struct SpatialRaycastStats(
    long SpatialRevision,
    int TotalEntityCount,
    int VisitedNodeCount,
    int CandidateCount,
    int NarrowPhaseTestCount,
    int HitCount)
{
    public string ToChineseProbe()
    {
        return $"【ARCH-C-R2-E】射线命中查询完成；索引代际={SpatialRevision}；总实体={TotalEntityCount}；访问节点={VisitedNodeCount}；候选={CandidateCount}；精确检测={NarrowPhaseTestCount}；真实命中={HitCount}";
    }
}
