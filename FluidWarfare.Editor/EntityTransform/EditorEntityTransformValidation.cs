using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.EntityTransform;

/// <summary>
/// Transform 输入校验。
/// 解析 X/Y/Z 文本，检查 NaN/Infinity/空值，返回中文错误。
/// </summary>
public static class EditorEntityTransformValidation
{
    /// <summary>
    /// 校验三个坐标输入。
    /// </summary>
    /// <param name="xText">X 坐标文本。</param>
    /// <param name="yText">Y 坐标文本。</param>
    /// <param name="zText">Z 坐标文本。</param>
    /// <param name="position">成功解析后的位置。</param>
    /// <param name="errorMessage">失败时的中文错误。</param>
    /// <returns>解析是否成功。</returns>
    public static bool TryParse(
        string xText, string yText, string zText,
        out Vector3d position,
        out string errorMessage)
    {
        position = default;
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(xText))
        {
            errorMessage = "X 坐标不能为空。";
            return false;
        }
        if (string.IsNullOrWhiteSpace(yText))
        {
            errorMessage = "Y 坐标不能为空。";
            return false;
        }
        if (string.IsNullOrWhiteSpace(zText))
        {
            errorMessage = "Z 坐标不能为空。";
            return false;
        }

        if (!TryParseDouble(xText, out var x, out var xErr))
        {
            errorMessage = xErr ?? "X 坐标必须是有效数字。";
            return false;
        }
        if (!TryParseDouble(yText, out var y, out var yErr))
        {
            errorMessage = yErr ?? "Y 坐标必须是有效数字。";
            return false;
        }
        if (!TryParseDouble(zText, out var z, out var zErr))
        {
            errorMessage = zErr ?? "Z 坐标必须是有效数字。";
            return false;
        }

        if (double.IsNaN(x) || double.IsNaN(y) || double.IsNaN(z))
        {
            errorMessage = "坐标值不能为 NaN。";
            return false;
        }
        if (double.IsInfinity(x) || double.IsInfinity(y) || double.IsInfinity(z))
        {
            errorMessage = "坐标值不能为无穷。";
            return false;
        }

        position = new Vector3d(x, y, z);
        return true;
    }

    private static bool TryParseDouble(string text, out double value, out string? error)
    {
        // Accept both current culture and invariant (dot) decimal formats
        if (double.TryParse(text,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.CurrentCulture,
                out value))
        {
            error = null;
            return true;
        }

        if (double.TryParse(text,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out value))
        {
            error = null;
            return true;
        }

        error = null; // generic error message set by caller
        return false;
    }
}
