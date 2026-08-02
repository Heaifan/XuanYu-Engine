using XuanYu.Editor.Assets;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly SceneStaticModelCatalog _staticModelCatalog = new();
    readonly StaticModelAuthoringService _staticModelAuthoring = new();
    readonly Dictionary<AssetId, RenderStaticModelResource> _staticModelResources = new();

    public bool ImportStaticModel(string path)
    {
        if (HasBlockingInput) return false;
        var result = _staticModelAuthoring.Import(path, _sceneState, _staticModelCatalog);
        if (!result.Succeeded)
        {
            LogStaticModelImportFailure(result);
            FooterMessage = result.UserMessage;
            return false;
        }

        var entity = result.Entity!.Value;
        var binding = _staticModelCatalog.TryGetByEntity(entity.EntityKey, out var found)
            ? found : new SceneStaticModelBinding(entity.EntityKey, result.AssetId, result.SourcePath);
        var resource = StaticModelRenderAdapter.ToRenderResource(
            result.Model!, new RenderStaticModelKey(result.AssetId.Value),
            (int)_staticModelCatalog.Revision);
        _staticModelResources[result.AssetId] = resource;

        _historyOwner.PushEntry(new AddEntityHistoryEntry(entity, binding));
        SelectEntity(entity.EntityKey, "导入 GLB");
        FooterMessage = $"静态模型导入成功：{entity.Name}";
        LogStaticModelImportSuccess(result, resource);
        RaiseDocumentChanged();
        RefreshWorldProjectionBindings();
        return true;
    }

    void LogStaticModelImportSuccess(StaticModelAuthorResult result, RenderStaticModelResource resource)
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Import,
            "静态模型导入成功",
            $"实体={result.Entity!.Value.EntityKey}；资产={result.AssetId}；路径={result.SourcePath}；顶点={resource.Vertices.Count}；索引={resource.Indices.Count}");
        RefreshLogBindings();
    }

    void LogStaticModelImportFailure(StaticModelAuthorResult result)
    {
        _logBus.Warning(EditorLogSource.Editor, EditorLogCategory.Import,
            "静态模型导入失败",
            $"原因={result.UserMessage}；路径={result.SourcePath}");
        RefreshLogBindings();
    }
}
