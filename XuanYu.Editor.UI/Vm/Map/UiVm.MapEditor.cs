using System.Globalization;
using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

// MAP-DOC-A-R1：地图基础入口（地图内容仍由 MapSession 权威，身份/容器投影自 MapManifest）。
public sealed partial class UiVm
{
    public string MapName => CurrentMapManifest.Name;
    public string MapPath => CurrentMapManifestPath;
    public string MapIdText => CurrentMapManifest.Id;
    public string MapSizeText =>
        $"{MapSession.CurrentMap.SizeMeters.Width:0.####} × {MapSession.CurrentMap.SizeMeters.Depth:0.####} 米";
    public bool HasMap => true; // D2 会话语义：编辑器恒有当前地图（初始默认 10 km）。
    public void NewMap()
    {
        var result = MapSession.CreateNewMap();
        if (!result.IsSuccess)
        {
            FailEdit(result.Error?.Message ?? "");
            return;
        }

        ResetMapManifestFromCurrentMap();
        SyncPropertyTexts();
        MapEditError = ""; FooterMessage = "地图已新建（未保存）。";
        RaiseMapDocumentChanged();
    }

    // 应用修改：单次原子提交（UpdateMapProperties 一个历史节点，失败整体拒绝零污染）。
    // D5 纠偏：字段级校验（每字段独立错误）+ 提交定位第一处错误 + 校验失败不清空输入。
    public void ApplyMapProperties()
    {
        var widthError = ValidateMapField("宽度", MapWidthText, out var width);
        var depthError = ValidateMapField("深度", MapDepthText, out var depth);
        var heightError = ValidateMapField("基础高度", MapBaseHeightText, out var height);
        SetFieldError("宽度", widthError);
        SetFieldError("深度", depthError);
        SetFieldError("基础高度", heightError);
        if (widthError.Length > 0 || depthError.Length > 0 || heightError.Length > 0)
        {
            FirstInvalidField = widthError.Length > 0 ? "宽度"
                : depthError.Length > 0 ? "深度" : "基础高度";
            MapEditError = widthError + depthError + heightError;
            RaiseMapDocumentChanged();
            return;
        }
        FirstInvalidField = "";

        var before = $"{MapSession.CurrentMap.SizeMeters.Width:0.####}×{MapSession.CurrentMap.SizeMeters.Depth:0.####}";
        LogMapPropertiesStarted(MapSession.CurrentMap.MapId.Value, before,
            MapSession.CurrentMap.Surface.BaseHeightMeters, $"{width:0.####}×{depth:0.####}", height,
            MapSession.CurrentStateId, MapSession.ChangeSequence);
        var result = MapSession.UpdateMapProperties(width, depth, height);
        if (!result.IsSuccess)
        {
            LogMapPropertiesFailed(result.Error?.Code ?? "Unknown", result.Error?.Message ?? "",
                before, MapSession.CurrentStateId, MapSession.ChangeSequence);
            FailEdit(result.Error?.Message ?? "");
            return;
        }

        var after = $"{MapSession.CurrentMap.SizeMeters.Width:0.####}×{MapSession.CurrentMap.SizeMeters.Depth:0.####}";
        LogMapPropertiesSucceeded(MapSession.CurrentMap.MapId.Value, after,
            MapSession.CurrentMap.Surface.BaseHeightMeters, MapSession.CurrentStateId,
            MapSession.ChangeSequence, MapSession.CanUndo, MapSession.CanRedo);
        MapEditError = ""; FooterMessage = "地图属性已应用。"; RaiseMapDocumentChanged();
    }


    public void FocusMap()
    {
        ApplyMapViewFraming();
    }

    void RaiseMapDocumentChanged()
    {
        OnPropertyChanged(nameof(MapName));
        OnPropertyChanged(nameof(MapPath));
        OnPropertyChanged(nameof(MapIdText));
        OnPropertyChanged(nameof(MapIdDisplay));
        OnPropertyChanged(nameof(MapSizeText));
        OnPropertyChanged(nameof(MapStatusText));
        OnPropertyChanged(nameof(CurrentMapManifest));
        OnPropertyChanged(nameof(MapManifestIdText));
        OnPropertyChanged(nameof(MapManifestCoordinateSystemText));
        OnPropertyChanged(nameof(DatasetCount));
        OnPropertyChanged(nameof(DatasetEmptyState));
    }
    void FailEdit(string message)
    {
        MapEditError = message;
        OnPropertyChanged(nameof(MapEditError));
    }

}
