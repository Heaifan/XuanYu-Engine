using System.Globalization;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5（二次纠偏，按用户方案）：地图表单三级校验。
// 输入阶段：轻量规则（非法字符/NaN/Infinity/明显超界）；值仍非法时错误不得消失；
//          输入中态（空/-/./1. 等临时文本）不清除已有错误。
// 失焦阶段：完整单字段校验（格式 + 范围）。
// 提交阶段：全部字段 + 跨字段关系（MapSession 业务兜底）+ 定位第一处错误 + 页面汇总。
// 校验失败不清空输入；范围边界与领域权威一致（MapDefinitionValidator 100~1000000 米）。
public sealed partial class UiVm
{
    string _mapWidthText = "10000";
    public string MapWidthText { get => _mapWidthText; set { _mapWidthText = value; OnPropertyChanged(nameof(MapWidthText)); ValidateMapFieldOnInput("宽度", value); OnPropertyChanged(nameof(MapStatusText)); } }
    string _mapDepthText = "10000";
    public string MapDepthText { get => _mapDepthText; set { _mapDepthText = value; OnPropertyChanged(nameof(MapDepthText)); ValidateMapFieldOnInput("深度", value); OnPropertyChanged(nameof(MapStatusText)); } }
    string _mapBaseHeightText = "0";
    public string MapBaseHeightText { get => _mapBaseHeightText; set { _mapBaseHeightText = value; OnPropertyChanged(nameof(MapBaseHeightText)); ValidateMapFieldOnInput("基础高度", value); OnPropertyChanged(nameof(MapStatusText)); } }
    public string MapEditError { get; private set; } = "";

    public string MapWidthError { get; private set; } = "";
    public string MapDepthError { get; private set; } = "";
    public string MapBaseHeightError { get; private set; } = "";
    public string FirstInvalidField { get; private set; } = "";
    public string FormErrorSummary => MapEditError; // 页面问题汇总（提交级）

    // 输入阶段：轻量校验（非法字符/NaN/Infinity/明显超界）。
    // 非法 → 错误保持（不得消失）；合法 → 立即清除；输入中态 → 保持现状。
    public void ValidateMapFieldOnInput(string field, string text)
    {
        if (IsIncompleteNumberInput(text)) return; // 输入中态（空/-/./1. 等）：不清除已有错误
        if (!TryParseMeters(text, field, out var value, out var error))
        {
            SetFieldError(field, error);
            return;
        }
        if (IsSizeField(field) && IsOutOfRange(value))
        {
            SetFieldError(field, RangeError(field));
            return;
        }
        SetFieldError(field, "");
    }

    // 失焦/提交共用：完整单字段校验（格式 + 范围）；返回空串表示通过
    public string ValidateMapField(string field, string text, out double meters)
    {
        meters = 0;
        if (!TryParseMeters(text, field, out meters, out var error))
        {
            SetFieldError(field, error);
            return error;
        }
        if (IsSizeField(field) && IsOutOfRange(meters))
        {
            var rangeError = RangeError(field);
            SetFieldError(field, rangeError);
            return rangeError;
        }
        SetFieldError(field, "");
        return "";
    }
}
