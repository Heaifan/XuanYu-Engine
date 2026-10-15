using System.Globalization;
using System.Text;

internal static class BenchmarkSvgCharts
{
    public static void WriteAll(string root, IReadOnlyList<BenchmarkSummaryRow> rows)
    {
        var timed = rows.Any(x => x.GpuMsP95.HasValue);
        Bars(Path.Combine(root, "chart-render-time.svg"), rows,
            timed ? "GPU RenderPass 耗时 P50 / P95（ms）" : "Fence + Readback 墙钟耗时 P50 / P95（ms）",
            timed ? x => x.GpuMsP50 : x => x.WallMsP50, timed ? x => x.GpuMsP95 : x => x.WallMsP95);
        Bars(Path.Combine(root, "chart-triangles.svg"), rows, "Terrain 三角形数量（跨进程均值）", x => x.Triangles);
        Bars(Path.Combine(root, "chart-gpu-memory.svg"), rows, "Terrain GPU 驻留缓冲估算（MiB）", x => x.EstimatedGpuBytes / 1048576);
        Stacked(Path.Combine(root, "chart-lod.svg"), rows);
        Bars(Path.Combine(root, "chart-coverage.svg"), rows, "真实 GPU Terrain 像素覆盖率（%）", x => x.TerrainCoverage);
    }

    static void Bars(string path, IReadOnlyList<BenchmarkSummaryRow> rows, string title,
        Func<BenchmarkSummaryRow, double?> first, Func<BenchmarkSummaryRow, double?>? second = null)
    {
        var max = rows.SelectMany(x => new[] { first(x) ?? 0, second?.Invoke(x) ?? 0 }).DefaultIfEmpty(1).Max();
        var b = Header(title, rows.Count); var y = 56;
        foreach (var row in rows)
        {
            Label(b, 8, y + 15, $"{row.Samples}/{row.Pose}");
            Rect(b, 165, y, Value(first(row), max), 12, "#2575b8");
            if (second is not null) Rect(b, 165, y + 15, Value(second(row), max), 8, "#e68934");
            y += second is null ? 27 : 34;
        }
        b.Append("</svg>"); File.WriteAllText(path, b.ToString());
    }

    static void Stacked(string path, IReadOnlyList<BenchmarkSummaryRow> rows)
    {
        var max = rows.Max(x => x.LodMeans.Sum()); if (max <= 0) max = 1;
        var b = Header("Terrain Chunk LOD 分布（跨进程均值）", rows.Count); var y = 56;
        var colors = new[] { "#2f855a", "#68a357", "#d6a72b", "#dd7434", "#a53d42" };
        foreach (var row in rows)
        {
            Label(b, 8, y + 14, $"{row.Samples}/{row.Pose}"); var x = 165d;
            for (var i = 0; i < 5; i++) { var width = Value(row.LodMeans[i], max); Rect(b, x, y, width, 16, colors[i]); x += width; }
            y += 28;
        }
        b.Append("</svg>"); File.WriteAllText(path, b.ToString());
    }

    static StringBuilder Header(string title, int rows) => new($"<svg xmlns='http://www.w3.org/2000/svg' width='1400' height='{Math.Max(220, 70 + rows * 35)}'><rect width='100%' height='100%' fill='white'/><text x='8' y='28' font-size='20'>{title}</text>");
    static void Label(StringBuilder b, int x, int y, string value) => b.Append($"<text x='{x}' y='{y}' font-size='12'>{value}</text>");
    static void Rect(StringBuilder b, double x, double y, double width, int height, string color) => b.Append($"<rect x='{N(x)}' y='{N(y)}' width='{N(Math.Max(1, width))}' height='{height}' fill='{color}'/>");
    static double Value(double? value, double max) => Value(value ?? 0, max);
    static double Value(double value, double max) => Math.Clamp(value / max * 1050, 1, 1050);
    static string N(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
