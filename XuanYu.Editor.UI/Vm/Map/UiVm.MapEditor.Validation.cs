using System.Globalization;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5（纠偏）：地图属性表单字段级校验。
// 每个输入框只绑定自身错误；ValidateOnInput（输入即清除）、ValidateOnLostFocus（失焦校验）、
// ValidateOnSubmit（提交全校验 + 定位第一处错误）；校验失败不清空输入。
public sealed partial class UiVm
{
    string _mapWidthText = "10000";
    public string MapWidthText { get => _mapWidthText; set { _mapWidthText = value; OnPropertyChanged(nameof(MapWidthText)); ClearFieldErrorOnInput("宽度"); } }
    string _mapDepthText = "10000";
    public string MapDepthText { get => _mapDepthText; set { _mapDepthText = value; OnPropertyChanged(nameof(MapDepthText)); ClearFieldErrorOnInput("深度"); } }
    string _mapBaseHeightText = "0";
    public string MapBaseHeightText { get => _mapBaseHeightText; set { _mapBaseHeightText = value; OnPropertyChanged(nameof(MapBaseHeightText)); ClearFieldErrorOnInput("基础高度"); } }
    public string MapEditError { get; private set; } = "";

    public string MapWidthError { get; private set; } = "";
    public string MapDepthError { get; private set; } = "";
    public string MapBaseHeightError { get; private set; } = "";
    public string FirstInvalidField { get; private set; } = "";
    public string FormErrorSummary => MapEditError; // 页面问题汇总（提交级）

    // ValidateOnInput：修改字段后立即清除该字段错误（错误随输入即时消除）
    void ClearFieldErrorOnInput(string field)
    {
        if (field == "宽度" && MapWidthError.Length > 0) { MapWidthError = ""; OnPropertyChanged(nameof(MapWidthError)); }
        else if (field == "深度" && MapDepthError.Length > 0) { MapDepthError = ""; OnPropertyChanged(nameof(MapDepthError)); }
        else if (field == "基础高度" && MapBaseHeightError.Length > 0) { MapBaseHeightError = ""; OnPropertyChanged(nameof(MapBaseHeightError)); }
    }

    // ValidateOnLostFocus / ValidateOnSubmit：单字段校验（失焦）与提交共用；返回空串表示通过
    public string ValidateMapField(string field, string text, out double meters)
    {
        meters = 0;
        if (!TryParseMeters(text, field, out meters, out var error))
        {
            SetFieldError(field, error);
            return error;
        }
        SetFieldError(field, "");
        return "";
    }

    void SetFieldError(string field, string error)
    {
        if (field == "宽度") { MapWidthError = error; OnPropertyChanged(nameof(MapWidthError)); }
        else if (field == "深度") { MapDepthError = error; OnPropertyChanged(nameof(MapDepthError)); }
        else { MapBaseHeightError = error; OnPropertyChanged(nameof(MapBaseHeightError)); }
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
