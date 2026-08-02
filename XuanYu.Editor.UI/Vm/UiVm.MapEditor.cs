using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

// 复用 D2 MapDocumentOwner/MapStorageService 与 D3 WorldMapStateOwner，不建第二套系统。
public sealed partial class UiVm
{
    readonly MapDocumentOwner _mapDocument = new();
    readonly MapStorageService _mapStorage = new();

    public MapDocumentOwner MapDocument => _mapDocument;
    public string MapName => _mapDocument.CurrentMap?.Name ?? "未加载地图";
    public string MapPath => _mapDocument.CurrentPath ?? "";
    public string MapIdText => _mapDocument.CurrentMap?.MapId.ToString() ?? "—";
    public string MapSizeText => _mapDocument.CurrentMap is { } m
        ? $"{m.SizeMeters.Width} × {m.SizeMeters.Depth} 米"
        : "—";
    public string MapStatusText => !_mapWorld.HasMap ? "未加载"
        : _mapDocument.IsDirty ? "未保存" : "已保存";

    public void NewMap()
    {
        var doc = XuanYu.Editor.MapDocument.MapDocument.CreateNew("TestBattlefield", 2000, 2000);
        _mapDocument.New(doc);
        SyncMapToWorld(doc);
        FooterMessage = "地图已新建（未保存）。";
        RaiseMapDocumentChanged();
    }

    public async Task<bool> OpenMapAsync(string path)
    {
        var result = await _mapStorage.LoadAsync(path);
        if (!result.Succeeded || result.Value is null)
        {
            _mapDocument.MarkError(result.Message);
            FooterMessage = result.Message;
            FooterState = "状态：地图打开失败";
            RaiseMapDocumentChanged();
            return false;
        }

        var doc = result.Value;
        _mapDocument.Load(path, doc);
        SyncMapToWorld(doc);
        FooterMessage = $"地图已打开：{doc.Name}。";
        FooterState = "状态：就绪";
        RaiseMapDocumentChanged();
        return true;
    }

    public async Task<bool> SaveMapAsync(string path)
    {
        if (_mapDocument.CurrentMap is not { } doc) return false;
        var result = await _mapStorage.SaveAsync(path, doc);
        if (!result.Succeeded || result.Value is null)
        {
            _mapDocument.MarkError(result.Message);
            FooterMessage = result.Message;
            FooterState = "状态：地图保存失败";
            RaiseMapDocumentChanged();
            return false;
        }

        _mapDocument.Save(path);
        FooterMessage = $"地图已保存：{doc.Name}。";
        FooterState = "状态：就绪";
        RaiseMapDocumentChanged();
        return true;
    }

    public void UnloadMapFromEditor()
    {
        UnloadMap();
        _mapDocument.Unload();
        FooterMessage = "地图已卸载。";
        RaiseMapDocumentChanged();
    }

    public void FocusMap()
    {
        if (!_mapWorld.HasMap) return;
        ApplyMapViewFraming();
        FooterMessage = "相机已从斜上方取景整张地图。";
    }

    void SyncMapToWorld(XuanYu.Editor.MapDocument.MapDocument doc)
    {
        _mapWorld.Load(MapDocumentWorldBridge.ToWorldState(doc));
        OnPropertyChanged(nameof(HasMap)); ApplyMapViewFraming();
        PublishSceneRenderSnapshot();
    }
    void RaiseMapDocumentChanged()
    {
        OnPropertyChanged(nameof(MapName));
        OnPropertyChanged(nameof(MapPath));
        OnPropertyChanged(nameof(MapIdText));
        OnPropertyChanged(nameof(MapSizeText));
        OnPropertyChanged(nameof(MapStatusText));
    }
}
