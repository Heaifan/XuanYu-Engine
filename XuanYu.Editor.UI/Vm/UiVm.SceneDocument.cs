using XuanYu.Editor.SceneDocument;
using XuanYu.Render.Abstractions;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly SceneDocumentSession _documentSession = new();
    readonly SceneStorageService _sceneStorage = new();
    readonly SceneDocumentSaveTransaction _saveTransaction;
    readonly SceneDocumentLoadTransaction _loadTransaction;
    readonly IWorldPartitionStrategy _partitionStrategy = new GridWorldPartitionStrategy(regionSize: 5);
    readonly SceneStateOwner _sceneState;
    public event Action<string>? FileCommandRequested;
    public bool IsSceneDirty => _documentSession.IsDirty(_historyOwner.CurrentRevision);
    public string CurrentScenePath => _documentSession.CurrentPath ?? "";
    public string DocumentWindowTitle => $"玄域引擎编辑器 v0.2.24.1-rz - {DocumentTitle}";
    public string DocumentTitle => $"{DocumentFileName}{(IsSceneDirty ? "（未保存）" : "")}";
    public string DocumentFileName =>
        string.IsNullOrWhiteSpace(CurrentScenePath) ? "未命名场景" : Path.GetFileName(CurrentScenePath);

    public async Task<bool> OpenSceneAsync(string path)
    {
        SetSceneBusy(true);
        FooterState = "状态：正在加载";
        LogSceneLoadStart(path);
        var candidate = await _loadTransaction.BuildCandidateAsync(path);
        if (!candidate.Succeeded || candidate.Value is null)
        {
            SetSceneBusy(false);
            _documentSession.MarkError(candidate.Message);
            FooterMessage = candidate.Message;
            FooterState = "状态：加载失败";
            LogSceneLoadFailure(path, candidate);
            RaiseDocumentChanged();
            await _dialogService.ShowErrorAsync("打开场景失败",
                "无法打开所选场景。\n场景文件内容无效或版本不受支持。");
            return false;
        }

        var value = candidate.Value;
        LogSceneLoadStage("ReplaceWorld");
        CancelActiveInput("打开场景");
        _sceneState.ReplaceEntities(value.Entities);
        _staticModelCatalog.ReplaceAll(value.Bindings, value.Models);
        _staticModelResources.Clear();
        foreach (var (assetId, model) in value.Models)
        {
            _staticModelResources[assetId] = StaticModelRenderAdapter.ToRenderResource(
                model, new RenderStaticModelKey(assetId.Value), (int)_staticModelCatalog.Revision);
        }

        ResetCameraForSceneReplacement(value.Entities.Count > 0);
        _historyOwner.Clear();
        _documentSession.MarkLoaded(path, value.Snapshot);
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "打开场景");
        FooterMessage = value.HasUnavailableAssets ? "场景已打开，部分资源不可用。" : "场景已打开。";
        LogSceneLoadSuccess(path, value.Entities.Count);
        SetSceneBusy(false);
        RaiseDocumentChanged();
        ShowTemporaryDocumentStatus("状态：场景加载成功");
        RefreshWorldProjectionBindings();
        if (value.HasUnavailableAssets) await ShowAssetSummaryAsync(value);
        return true;
    }

    async Task ShowAssetSummaryAsync(SceneLoadCandidate value)
    {
        var message = $"部分模型文件缺失或无法读取。\n相关实体已保留，并使用占位边界显示。\n\n缺失：{value.MissingCount}\n读取失败：{value.FailedCount}";
        await _dialogService.ShowWarningAsync("场景已打开，但部分资源不可用", message);
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
