using XuanYu.Editor.SceneDocument;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly SceneDocumentSession _documentSession = new();
    readonly SceneStorageService _sceneStorage = new();
    readonly IWorldPartitionStrategy _partitionStrategy = new GridWorldPartitionStrategy(regionSize: 5);
    readonly SceneStateOwner _sceneState;

    public event Action<string>? FileCommandRequested;
    public bool IsSceneDirty => _documentSession.IsDirty(_historyOwner.Count);
    public string CurrentScenePath => _documentSession.CurrentPath ?? "";
    public string DocumentWindowTitle => $"玄域引擎编辑器 v0.2.21.3-fix - {DocumentTitle}";
    public string DocumentTitle => $"{(string.IsNullOrWhiteSpace(CurrentScenePath) ? "未命名场景" : Path.GetFileName(CurrentScenePath))}{(IsSceneDirty ? "*" : "")}";

    public void NewBlankScene()
    {
        CancelInteraction("新建场景");
        _historyOwner.Clear();
        _documentSession.MarkNew();
        _sceneState.ReplaceEntities([]);
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "新建场景");
        FooterMessage = "已创建空白未命名场景。";
        RaiseDocumentChanged();
        RefreshWorldProjectionBindings();
    }

    public async Task<bool> OpenSceneAsync(string path)
    {
        FooterState = "状态：正在加载";
        LogSceneLoadStart(path);
        var result = await _sceneStorage.LoadAsync(path, stage => LogSceneLoadStage(stage));
        if (!result.Succeeded || result.Value is null)
        {
            _documentSession.MarkError(result.Message);
            FooterMessage = result.Message;
            FooterState = "状态：加载失败";
            LogSceneLoadFailure(path, result);
            RaiseDocumentChanged();
            return false;
        }
        LogSceneLoadStage("BuildCandidate");
        IReadOnlyList<WorldEntitySnapshot> entities;
        try { entities = SceneDocumentWorldBridge.ToWorld(result.Value, _partitionStrategy); }
        catch (Exception ex) { return FailCandidateBuild(path, ex); }
        LogSceneLoadStage("ReplaceWorld");
        _sceneState.ReplaceEntities(entities);
        CancelInteraction("打开场景");
        _historyOwner.Clear();
        _documentSession.MarkLoaded(path, result.Value);
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "打开场景");
        FooterMessage = "场景已打开。";
        FooterState = "状态：就绪";
        LogSceneLoadSuccess(path, entities.Count);
        RaiseDocumentChanged();
        RefreshWorldProjectionBindings();
        return true;
    }

    public bool CommitActiveInteractionForSave()
    {
        if (!_editorState.InteractionSnapshot.HasCapture) return false;
        CommitInteraction();
        return true;
    }

    void RaiseDocumentChanged()
    {
        OnPropertyChanged(nameof(IsSceneDirty));
        OnPropertyChanged(nameof(CurrentScenePath));
        OnPropertyChanged(nameof(DocumentTitle));
        OnPropertyChanged(nameof(DocumentWindowTitle));
        OnPropertyChanged(nameof(TransformHistoryCount));
        OnPropertyChanged(nameof(TransformRedoCount));
    }
}
