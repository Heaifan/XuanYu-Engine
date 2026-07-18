namespace XuanYu.Core.Spatial;

public readonly record struct SpatialQueryStats(
    long SpatialRevision,
    int TotalEntityCount,
    int VisitedNodeCount,
    int CandidateCount)
{
    public string ToChineseProbe()
    {
        return $"【ARCH-C-R2-D】空间查询完成；索引代际={SpatialRevision}；总实体={TotalEntityCount}；访问节点={VisitedNodeCount}；候选实体={CandidateCount}";
    }
}
