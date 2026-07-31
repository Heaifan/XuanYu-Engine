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
    public string DocumentWindowTitle => $"玄域引擎编辑器 v0.2.21.2-rz - {DocumentTitle}";
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
        var result = await _sceneStorage.LoadAsync(path);
        if (!result.Succeeded || result.Value is null)
        {
            _documentSession.MarkError(result.Message);
            FooterMessage = result.Message;
            FooterState = "状态：加载失败";
            RaiseDocumentChanged();
            return false;
        }
        CancelInteraction("打开场景");
        _historyOwner.Clear();
        var entities = SceneDocumentWorldBridge.ToWorld(result.Value, _partitionStrategy);
        _sceneState.ReplaceEntities(entities);
        _documentSession.MarkLoaded(path, result.Value);
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "打开场景");
        FooterMessage = "场景已打开。";
        FooterState = "状态：就绪";
        RaiseDocumentChanged();
        RefreshWorldProjectionBindings();
        return true;
    }

    public async Task<bool> SaveSceneAsync(string? path = null)
    {
        CommitActiveInteractionForSave();
        var target = path ?? _documentSession.CurrentPath;
        if (string.IsNullOrWhiteSpace(target)) return false;
        var snapshot = SceneDocumentWorldBridge.Capture(
            _sceneState, _documentSession.SceneId, _documentSession.SceneName);
        FooterState = "状态：正在保存";
        var result = await _sceneStorage.SaveAsync(target, snapshot);
        if (!result.Succeeded)
        {
            _documentSession.MarkError(result.Message);
            FooterMessage = result.Message;
            FooterState = "状态：保存失败";
            RaiseDocumentChanged();
            return false;
        }
        _documentSession.MarkSaved(target, snapshot, _historyOwner.Count);
        FooterMessage = "场景已保存。";
        FooterState = "状态：就绪";
        RaiseDocumentChanged();
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
