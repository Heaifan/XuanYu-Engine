namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public string SceneId => _documentSession.SceneId;
    public long SceneRevision => _sceneState.SpatialRevision;
    public int SceneEntityCount => _sceneState.RenderSnapshot.Entities.Count;
}
