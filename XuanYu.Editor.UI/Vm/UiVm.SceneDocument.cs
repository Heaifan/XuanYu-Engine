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
    public bool IsSceneDirty => _documentSession.IsDirty(_historyOwner.CurrentRevision);
    public string CurrentScenePath => _documentSession.CurrentPath ?? "";
    public string DocumentWindowTitle => $"玄域引擎编辑器 v0.2.21.11-fix - {DocumentTitle}";
    public string DocumentTitle => $"{DocumentFileName}{(IsSceneDirty ? "（未保存）" : "")}";
    public string DocumentFileName =>
        string.IsNullOrWhiteSpace(CurrentScenePath) ? "未命名场景" : Path.GetFileName(CurrentScenePath);

    public void NewBlankScene()
    {
        CancelActiveInput("新建场景");
        _historyOwner.Clear();
        _documentSession.MarkNew();
        _sceneState.ReplaceEntities([]);
        ResetCameraForSceneReplacement();
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "新建场景");
        FooterMessage = "已创建空白未命名场景。";
        RaiseDocumentChanged();
        RefreshWorldProjectionBindings();
    }

    public async Task<bool> OpenSceneAsync(string path)
    {
        SetSceneBusy(true);
        FooterState = "状态：正在加载";
        LogSceneLoadStart(path);
        var result = await _sceneStorage.LoadAsync(path, stage => LogSceneLoadStage(stage));
        if (!result.Succeeded || result.Value is null)
        {
            SetSceneBusy(false);
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
        catch (Exception ex) { SetSceneBusy(false); return FailCandidateBuild(path, ex); }
        LogSceneLoadStage("ReplaceWorld");
        CancelActiveInput("打开场景");
        _sceneState.ReplaceEntities(entities);
        ResetCameraForSceneReplacement(entities.Count > 0);
        _historyOwner.Clear();
        _documentSession.MarkLoaded(path, result.Value);
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "打开场景");
        FooterMessage = "场景已打开。";
        LogSceneLoadSuccess(path, entities.Count);
        SetSceneBusy(false);
        RaiseDocumentChanged();
        ShowTemporaryDocumentStatus("状态：场景加载成功");
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
        ClearTransientDocumentStatusForDirty();
        OnPropertyChanged(nameof(IsSceneDirty));
        OnPropertyChanged(nameof(CurrentScenePath));
        OnPropertyChanged(nameof(DocumentTitle));
        OnPropertyChanged(nameof(DocumentFileName));
        OnPropertyChanged(nameof(DocumentWindowTitle));
        OnPropertyChanged(nameof(TransformHistoryCount));
        OnPropertyChanged(nameof(TransformRedoCount));
        RaiseDocumentStatusChanged();
    }
}
