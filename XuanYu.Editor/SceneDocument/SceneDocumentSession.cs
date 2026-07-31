namespace XuanYu.Editor.SceneDocument;

public sealed class SceneDocumentSession
{
    string _sceneId = Guid.NewGuid().ToString("N");
    string _sceneName = "未命名场景";
    long _savedRevision;

    public string? CurrentPath { get; private set; }
    public bool IsUntitled => string.IsNullOrWhiteSpace(CurrentPath);
    public string SceneId => _sceneId;
    public string SceneName => _sceneName;
    public string LastError { get; private set; } = "";

    public bool IsDirty(long revision) => revision != _savedRevision;

    public void MarkSaved(string path, SceneDocumentSnapshot snapshot, long revision)
    {
        CurrentPath = path;
        _sceneId = snapshot.SceneId;
        _sceneName = snapshot.SceneName;
        _savedRevision = revision;
        LastError = "";
    }

    public void MarkLoaded(string path, SceneDocumentSnapshot snapshot)
    {
        CurrentPath = path;
        _sceneId = snapshot.SceneId;
        _sceneName = snapshot.SceneName;
        _savedRevision = 0;
        LastError = "";
    }

    public void MarkNew()
    {
        CurrentPath = null;
        _sceneId = Guid.NewGuid().ToString("N");
        _sceneName = "未命名场景";
        _savedRevision = 0;
        LastError = "";
    }

    public void MarkError(string message) => LastError = message;
}
