using System.Globalization;
using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D3：地图属性入口（唯一数据源 = MapSession；保存/打开按钮禁用防 v1 双权威，D6 恢复）。
public sealed partial class UiVm
{
    public string MapName => MapSession.CurrentMap.DisplayName;
    public string MapPath => MapSession.CurrentFilePath ?? "";
    public string MapIdText => MapSession.CurrentMap.MapId.ToString();
    public string MapSizeText =>
        $"{MapSession.CurrentMap.SizeMeters.Width:0.####} × {MapSession.CurrentMap.SizeMeters.Depth:0.####} 米";
    public string MapStatusText => MapSession.IsDirty ? "未保存" : "已保存";
    public bool HasMap => true; // D2 会话语义：编辑器恒有当前地图（初始默认 10 km）。
    string _mapWidthText = "10000"; public string MapWidthText { get => _mapWidthText; set { _mapWidthText = value; OnPropertyChanged(nameof(MapWidthText)); } }
    string _mapDepthText = "10000"; public string MapDepthText { get => _mapDepthText; set { _mapDepthText = value; OnPropertyChanged(nameof(MapDepthText)); } }
    string _mapBaseHeightText = "0"; public string MapBaseHeightText { get => _mapBaseHeightText; set { _mapBaseHeightText = value; OnPropertyChanged(nameof(MapBaseHeightText)); } }
    public string MapEditError { get; private set; } = "";

    public void NewMap()
    {
        var result = MapSession.CreateNewMap();
        if (!result.IsSuccess)
        {
            FailEdit(result.Error?.Message ?? "");
            return;
        }

        SyncPropertyTexts();
        MapEditError = ""; FooterMessage = "地图已新建（未保存）。";
        RaiseMapDocumentChanged();
    }

    // 应用修改：单次原子提交（UpdateMapProperties 一个历史节点，失败整体拒绝零污染）。
    public void ApplyMapProperties()
    {
        if (!TryParseMeters(MapWidthText, "宽度", out var width, out var error) ||
            !TryParseMeters(MapDepthText, "深度", out var depth, out error) ||
            !TryParseMeters(MapBaseHeightText, "基础高度", out var height, out error))
        {
            FailEdit(error);
            return;
        }

        var result = MapSession.UpdateMapProperties(width, depth, height);
        if (!result.IsSuccess)
        {
            FailEdit(result.Error?.Message ?? "");
            return;
        }

        MapEditError = ""; FooterMessage = "地图属性已应用。";
        RaiseMapDocumentChanged();
    }

    public void FocusMap()
    {
        ApplyMapViewFraming();
        FooterMessage = "相机已从斜上方取景整张地图。";
    }

    void RaiseMapDocumentChanged()
    {
        OnPropertyChanged(nameof(MapName));
        OnPropertyChanged(nameof(MapPath));
        OnPropertyChanged(nameof(MapIdText));
        OnPropertyChanged(nameof(MapSizeText));
        OnPropertyChanged(nameof(MapStatusText));
    }
    void FailEdit(string message)
    {
        MapEditError = message;
        OnPropertyChanged(nameof(MapEditError));
    }

    static bool TryParseMeters(string text, string fieldName, out double value, out string error)
    {
        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && double.IsFinite(value))
        {
            error = "";
            return true;
        }

        value = 0;
        error = $"{fieldName}必须是有限数字。";
        return false;
    }

    static string FormatMeters(double meters) => meters.ToString("0.####", CultureInfo.InvariantCulture);
}
