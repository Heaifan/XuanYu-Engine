using System.Globalization;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5（二次纠偏，用户方案）：字段校验规则（解析/范围/输入中态）。
// 范围边界与领域权威一致（MapDefinitionValidator 100~1000000 米）；
// 基础高度仅要求有限数字（领域 ValidateSurface 无范围约束）。
public sealed partial class UiVm
{
    // 尺寸字段（宽/深）受领域范围约束；基础高度仅要求有限数字（领域 ValidateSurface）
    static bool IsSizeField(string field) => field is "宽度" or "深度";

    static bool IsOutOfRange(double meters) =>
        meters < MapDefinitionValidator.MinSizeMeters || meters > MapDefinitionValidator.MaxSizeMeters;

    static string RangeError(string fieldName) =>
        $"{fieldName}必须位于 {MapDefinitionValidator.MinSizeMeters}～{MapDefinitionValidator.MaxSizeMeters} 米之间。";

    // 输入中态：空文本、尾随符号（-、.、1.、1e、1e- 等编辑中的临时文本）
    static bool IsIncompleteNumberInput(string text)
    {
        if (text.Length == 0) return true;
        if (text is "-" or "." or "+" or "-." or "+.") return true;
        var last = text[^1];
        if (last is '.' or 'e' or 'E') return true;
        if (last is '-' or '+') return text.Length >= 2 && text[^2] is 'e' or 'E';
        return false;
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
