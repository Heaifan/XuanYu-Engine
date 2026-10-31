using System.Text.RegularExpressions;

namespace XuanYu.Editor.UI;

public static class DiagnosticId
{
    static readonly Regex Pattern = new(
        @"^XYE\.[A-Z]+(?:_[A-Z]+)*(?:\.[A-Z]+(?:_[A-Z]+)*){0,3}$",
        RegexOptions.CultureInvariant);

    public static void Validate(string value)
    {
        if (!Pattern.IsMatch(value))
            throw new ArgumentException($"诊断 ID 无效：{value}", nameof(value));
    }
}
