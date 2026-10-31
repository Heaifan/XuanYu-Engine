using XuanYu.Editor.SceneDocument;
using XuanYu.Render.Abstractions;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly SceneDocumentCoordinator _sceneDocumentCoordinator;
    SceneDocumentSession DocumentSession => _sceneDocumentCoordinator.Session;
    readonly IWorldPartitionStrategy _partitionStrategy = new GridWorldPartitionStrategy(regionSize: 5);
    readonly SceneStateOwner _sceneState;
    public event Action<string>? FileCommandRequested;
    public bool IsSceneDirty => DocumentSession.IsDirty(_historyOwner.CurrentRevision);
    public string CurrentScenePath => DocumentSession.CurrentPath ?? "";
    public string DocumentWindowTitle => $"玄域引擎编辑器 {System.Reflection.CustomAttributeExtensions.GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>(typeof(UiVm).Assembly)?.InformationalVersion ?? "未知版本"} - {DocumentTitle}";
    public string DocumentTitle => $"{DocumentFileName}{(IsSceneDirty ? "（未保存）" : "")}";
    public string DocumentFileName =>
        string.IsNullOrWhiteSpace(CurrentScenePath) ? "未命名场景" : Path.GetFileName(CurrentScenePath);

    public Task<bool> OpenSceneAsync(string path) => _sceneDocumentCoordinator.OpenAsync(path,
        StartSceneLoad, FailSceneLoad, RetrySceneLoadAsync, LogSceneReplace,
        ReplaceSceneCandidate, CompleteSceneReplacementAsync, CompleteSceneLoad, ReportUnavailableAssetsAsync);

    void LogSceneReplace(string _) => LogSceneLoadStage("ReplaceWorld");

    void ReplaceSceneCandidate(SceneLoadCandidate value)
    {
        CancelActiveInput("打开场景");
        _sceneState.ReplaceEntities(value.Entities);
        _staticModelCatalog.ReplaceAll(value.Bindings, value.Models);
        _staticModelResources.Clear();
        foreach (var (assetId, model) in value.Models)
            _staticModelResources[assetId] = StaticModelRenderAdapter.ToRenderResource(
                model, new RenderStaticModelKey(assetId.Value), (int)_staticModelCatalog.Revision);
        ResetCameraForSceneReplacement(value.Entities.Count > 0);
        _historyOwner.Clear();
    }

    async Task CompleteSceneReplacementAsync(SceneLoadCandidate value)
    {
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "打开场景");
        await ResolveMapReferenceAsync(value.Snapshot, DocumentSession.CurrentPath ?? "");
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
        OnPropertyChanged(nameof(DocumentFileName)); OnPropertyChanged(nameof(DocumentWindowTitle));
        OnPropertyChanged(nameof(ProjectItems));
        OnPropertyChanged(nameof(TransformHistoryCount));
        OnPropertyChanged(nameof(TransformRedoCount));
        RaiseDocumentStatusChanged();
    }
}
