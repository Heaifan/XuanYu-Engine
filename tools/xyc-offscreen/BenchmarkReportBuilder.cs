using System.Text.Json;

internal static class BenchmarkReportBuilder
{
    public static int Run(string[] args)
    {
        var root = Value(args, "--out"); var runs = Enumerable.Range(1, 3)
            .Select(i => JsonSerializer.Deserialize<BenchmarkRun>(File.ReadAllText(Path.Combine(root,
                $"run-{i}", "run-manifest.json")), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException($"run-{i} manifest JSON 无效" )).ToArray();
        var summary = BenchmarkSummaryCalculator.Summarize(runs);
        var report = new BenchmarkFinalReport(runs, summary, Recommend(summary, runs));
        File.WriteAllText(Path.Combine(root, "benchmark-report.json"), JsonSerializer.Serialize(report,
            new JsonSerializerOptions { WriteIndented = true }));
        BenchmarkCsvWriter.Write(Path.Combine(root, "benchmark-summary.csv"), summary);
        BenchmarkSvgCharts.WriteAll(root, summary);
        BenchmarkMarkdownWriter.Write(Path.Combine(root, "report.md"), root, report);
        Console.WriteLine($"B1 REPORT={Path.Combine(root, "report.md")}");
        return summary.Any(x => x.Status != "PASS") ? 1 : 0;
    }

    static string Value(string[] args, string key)
    {
        var index = Array.IndexOf(args, key);
        return index >= 0 && index + 1 < args.Length ? Path.GetFullPath(args[index + 1])
            : throw new ArgumentException($"缺少参数 {key}");
    }

    static string Recommend(IReadOnlyList<BenchmarkSummaryRow> rows, BenchmarkRun[] runs)
    {
        var chunk = rows.Where(x => x.Pose == "chunk-aabb-entry").ToArray();
        var raw = runs.SelectMany(x => x.Configurations).Where(x => x.Pose == "chunk-aabb-entry").ToArray();
        if (raw.Any(x => x.CameraInsideChunkAabb && (x.TerrainPixels == 0 || x.VisibleChunks == 0)))
            return "优先检查 Chunk/Frustum 裁剪与近裁剪面：相机位于 Chunk AABB 内，但 GPU 可见量为零；需结合对应 chunk probe 定位裁剪面，当前仍是风险证据而非根因定论。";
        var near = rows.Where(x => x.Pose == "near-oblique").OrderBy(x => x.Samples).ToArray();
        if (near.Length == 3 && near[0].HeightErrorWorldP95.HasValue && near[2].HeightErrorWorldP95.HasValue &&
            near[0].HeightErrorWorldP95 > near[2].HeightErrorWorldP95 * 1.5)
            return "近地斜视的 HGT 世界高程重建误差随采样加密明显下降；优先评估采样密度 / 网格细化成本，并以差异图和 P95 数据复核视觉收益。";
        var zeroProjected = raw.SelectMany(x => x.ChunkProbes).Count(x => x.Cull == "VISIBLE" && x.ProjectedPixels <= 0);
        return zeroProjected > 0 ? $"发现 {zeroProjected} 个可见 Chunk 的 ProjectedPixels=0 记录，优先检查 LOD 投影尺寸/选择链；该计数本身不证明错误粗化。" :
            $"当前 12 配置没有足够证据区分 LOD、采样密度与裁剪；先依据 GPU 差异图、覆盖率、chunk probe 和高程误差选择下一组 A/B，不改现有算法。";
    }
}

internal sealed record BenchmarkFinalReport(BenchmarkRun[] Runs,
    List<BenchmarkSummaryRow> Summary, string Recommendation);
