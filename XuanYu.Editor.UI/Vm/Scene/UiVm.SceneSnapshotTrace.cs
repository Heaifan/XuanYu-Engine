namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    int _renderSnapshotPublishCount;
    int _lastLoggedRenderEntityCount = -1;

    void TraceRenderSnapshotPublish(int entityCount)
    {
        _renderSnapshotPublishCount++;
        if (_renderSnapshotPublishCount != 1) return;
        _lastLoggedRenderEntityCount = entityCount;
        TraceSelection("PublishSceneRenderSnapshot摘要", 1,
            $"次数={_renderSnapshotPublishCount}；实体数={entityCount}");
    }
}
