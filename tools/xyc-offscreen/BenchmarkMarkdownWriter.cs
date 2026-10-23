using System.Globalization;

internal static class BenchmarkMarkdownWriter
{
    public static void Write(string path, string root, BenchmarkFinalReport report)
    {
        var run = report.Runs[0]; var lines = new List<string>
        {
            "# XYC-B1 RTX 3060 Terrain GPU 基准报告", "",
            $"- 构建：`{run.BuildIdentity}`", $"- GPU：{run.Device}；Vulkan DriverVersion raw={run.DriverVersion}",
            $"- 环境：{run.OperatingSystem}；{run.Runtime}；CPU={run.CpuIdentifier}",
            $"- NASADEM：`n24e121` SHA-256 `{run.SourceSha256}`", "- 输出：1024×768，真实 Vulkan GPU Image Readback",
            $"- 运行：{report.Runs.Length} 个独立进程；每项 warmup 后目标采样 30 帧；GPU timestamp queue valid bits={run.TimestampValidBits}，period={run.TimestampPeriodNs.ToString("0.###", CultureInfo.InvariantCulture)} ns",
            "- measuredGpuBytes：UNKNOWN；estimatedGpuBytes 是 Terrain GPU vertex/index buffer capacity 合计，不是驱动报告的物理占用。", "",
            "## 统计图", "", "- [RenderPass 时间](chart-render-time.svg)", "- [三角形数量](chart-triangles.svg)",
            "- [GPU 缓冲估算](chart-gpu-memory.svg)", "- [GPU Terrain 覆盖率](chart-coverage.svg)", "- [LOD 分布](chart-lod.svg)", "",
            "## 12 配置汇总", "", "| Samples | Pose | GPU P50/P95/P99 ms | Fence+Readback P50/P95/P99 ms | HGT import P50/P95 ms | CPU sample P50/P95 ms | CPU mesh P50/P95 ms | Upload P50/P95 ms | Total wall avg ms | Triangles | LOD0..4 | Coverage | HGT error P95 m | Hash repeat | Status |",
            "|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---|---:|---:|---|---|"
        };
        foreach (var x in report.Summary)
            lines.Add($"| {x.Samples} | {x.Pose} | {N(x.GpuMsP50)}/{N(x.GpuMsP95)}/{N(x.GpuMsP99)} | {N(x.WallMsP50)}/{N(x.WallMsP95)}/{N(x.WallMsP99)} | {N(x.CpuImportMsP50)}/{N(x.CpuImportMsP95)} | {N(x.TerrainSampleMsP50)}/{N(x.TerrainSampleMsP95)} | {N(x.CpuMeshBuildMsP50)}/{N(x.CpuMeshBuildMsP95)} | {N(x.UploadMsP50)}/{N(x.UploadMsP95)} | {N(x.TotalWallMs)} | {N(x.Triangles)} | {string.Join('/', x.LodMeans.Select(v => N(v)))} | {N(x.TerrainCoverage)}% | {N(x.HeightErrorWorldP95)} | {x.RepeatPngHashStable} | {x.Status} |");
        lines.AddRange(["", "## 结论与视角证据", "", report.Recommendation, "",
            "| Samples | Pose | GPU PNG | Chunk/LOD probe | Camera in Chunk AABB | Height error visual P95 m |", "|---:|---|---|---|---|---:|"]);
        foreach (var x in report.Runs[0].Configurations)
        {
            var image = Path.GetRelativePath(root, x.Png).Replace('\\', '/');
            var diff = x.Samples == 513 ? "无" : $"[view](diff-{(x.Samples == 1025 ? "513-vs-1025" : "1025-vs-2049")}-{x.Pose}.png)";
            lines.Add($"| {x.Samples} | {x.Pose} | [PNG]({image}) | {x.ChunkProbes.Count} chunk records; {diff} | {x.CameraInsideChunkAabb} | {N(x.HeightErrorVisualP95Meters)} |");
        }
        lines.AddRange(["", "## 解释边界", "",
            "GPU P50/P95 来自每项 90 个 Vulkan timestamp query 样本（3 个独立进程×30 帧，TopOfPipe→BottomOfPipe，覆盖本离屏 RenderPass）；Fence+Readback 是 CPU 墙钟时间，不能折算成交互 FPS。CPU terrain sample、mesh build 与 upload 的 P50/P95 来自 3 个独立进程各一次配置测量（n=3），属于冷路径小样本，不应与 90 帧 GPU 分位数等量解读。CPU upload 是 Vulkan buffer 创建/上传 API 的 CPU 墙钟包围时间。PNG encode 单独记录于每帧 JSON 样本。",
            "2049 是高采样参考，不代表原始 3601² 真值。高程误差是在固定 64×64 个源位置上比较 HGT 世界高程与采样 Heightfield 双线性重建；视觉夸张误差单独按当前 VerticalExaggeration 倍乘。PNG 相异不单独判失败。用户 P4 仍需独立视觉验收。",
            "", "## 逐配置错误与缺样本", ""]);
        foreach (var x in report.Runs.SelectMany(x => x.Configurations).Where(x => x.Status != "PASS" || x.FailureReason.Length > 0))
            lines.Add($"- run-{report.Runs.First(r => r.Configurations.Contains(x)).Replicate} / {x.Samples} / {x.Pose}: `{x.Status}` {x.FailureReason}");
        File.WriteAllLines(path, lines);
    }

    static string N(double? x) => x?.ToString("0.###", CultureInfo.InvariantCulture) ?? "UNKNOWN";
}
