using System.Text.RegularExpressions;

namespace XYUI.Avalonia.Governance;

public enum XyuiMetricClassification { Tokenized, AllowedException, UnjustifiedMagicNumber }

public readonly record struct XyuiMetricFinding(
    string Metric, string Value, XyuiMetricClassification Classification, int Line);

public static class XyuiMetricGate
{
    static readonly Regex Assignment = new(
        @"(?<metric>Margin|Padding|Spacing|Height|Width|MinHeight|MinWidth|CornerRadius|BorderThickness)\s*=\s*(?:[""'](?<value>[^""']+)[""']|(?<value2>[^,;\r\n]+))",
        RegexOptions.Compiled);

    public static IReadOnlyList<XyuiMetricFinding> Analyze(string source, string filePath = "")
    {
        var findings = new List<XyuiMetricFinding>();
        var lines = source.Split('\n');
        for (var index = 0; index < lines.Length; index++)
        {
            foreach (Match match in Assignment.Matches(lines[index]))
            {
                var value = match.Groups["value"].Success
                    ? match.Groups["value"].Value : match.Groups["value2"].Value.Trim();
                var classification = Classify(lines[index], value, filePath);
                findings.Add(new(match.Groups["metric"].Value, value, classification, index + 1));
            }
        }
        return findings;
    }

    static XyuiMetricClassification Classify(string line, string value, string path)
    {
        if (value.Contains("XY.", StringComparison.Ordinal) ||
            value.Contains("Xyui", StringComparison.Ordinal) ||
            value.Contains("DynamicResource", StringComparison.Ordinal))
            return XyuiMetricClassification.Tokenized;
        if (line.Contains("xyui:allowed-exception", StringComparison.OrdinalIgnoreCase) ||
            line.Contains("canonical:", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("Geometry", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("Canvas", StringComparison.OrdinalIgnoreCase) ||
            path.Contains("Animation", StringComparison.OrdinalIgnoreCase))
            return XyuiMetricClassification.AllowedException;
        return XyuiMetricClassification.UnjustifiedMagicNumber;
    }
}
