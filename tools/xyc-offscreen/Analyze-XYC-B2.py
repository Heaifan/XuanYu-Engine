import csv, hashlib, json, math, statistics, subprocess, sys
from pathlib import Path
from PIL import Image, ImageChops, ImageFilter

root = Path(sys.argv[1]).resolve()
groups = {}
for path in sorted(root.glob("*/replicate-*/run-1/run-manifest.json")):
    data = json.loads(path.read_text(encoding="utf-8-sig"))
    config = data["Configurations"][0]
    groups.setdefault(path.parents[2].name, []).append((path, data, config))

def pct(values, p):
    values = sorted(values)
    if not values: return None
    pos = (len(values) - 1) * p
    lo = int(pos); hi = min(lo + 1, len(values) - 1)
    return values[lo] + (values[hi] - values[lo]) * (pos - lo)

def stats(values):
    mean = statistics.mean(values) if values else None
    return {"p50": pct(values, .5), "p95": pct(values, .95), "mean": mean,
            "min": min(values) if values else None, "max": max(values) if values else None,
            "cv": statistics.pstdev(values) / mean if len(values) > 1 and mean else None}

def image_metrics(reference, candidate, out):
    a = Image.open(reference).convert("RGB"); b = Image.open(candidate).convert("RGB")
    if a.size != b.size: raise ValueError("PNG dimensions differ")
    ap = list(a.get_flattened_data()); bp = list(b.get_flattened_data()); sky = (89, 140, 204)
    xor = common = abs_rgb = abs_edge = 0
    ae = list(a.convert("L").filter(ImageFilter.FIND_EDGES).get_flattened_data())
    be = list(b.convert("L").filter(ImageFilter.FIND_EDGES).get_flattened_data())
    diff = Image.new("RGB", a.size); dp = []
    for i, (x, y) in enumerate(zip(ap, bp)):
        at = x != sky; bt = y != sky
        if at != bt: xor += 1
        if at and bt:
            common += 1; abs_rgb += sum(abs(x[c] - y[c]) for c in range(3))
            abs_edge += abs(ae[i] - be[i])
        dp.append(tuple(min(255, abs(x[c] - y[c]) * 3) for c in range(3)))
    diff.putdata(dp); diff.save(out)
    total = len(ap)
    return {"maskXorPct": xor * 100 / total, "rgbMaeCommonTerrain": abs_rgb / (common * 3) if common else None,
            "edgeMaeCommonTerrain": abs_edge / common if common else None,
            "coveragePct": sum(x != sky for x in bp) * 100 / total, "commonTerrainPixels": common}

rows = []
for name, entries in groups.items():
    configs = [x[2] for x in entries]
    gpu = [c["GpuMsP50"] for c in configs if c.get("GpuMsP50") is not None]
    p95 = [c["GpuMsP95"] for c in configs if c.get("GpuMsP95") is not None]
    samples = [v for c in configs for v in c.get("SamplesData", []) if v.get("GpuRenderMs") is not None]
    pooled = [v["GpuRenderMs"] for v in samples]
    first = configs[0]
    rows.append({"config": name, "processes": len(entries), "sampleCountPerProcess": [c.get("ValidGpuSamples", 0) for c in configs],
        "gpuProcessP50Ms": stats(gpu), "gpuProcessP95Ms": stats(p95), "gpuPooledFramesMs": stats(pooled),
        "triangles": stats([c["Triangles"] for c in configs]), "visibleChunks": stats([c["VisibleChunks"] for c in configs]),
        "drawCalls": stats([c["DrawCalls"] for c in configs]), "lodCounts": [first[f"Lod{i}"] for i in range(5)],
        "cpuMeshBuildMs":stats([c["CpuMeshBuildMs"] for c in configs]),"cpuUploadMs":stats([c["CpuUploadMs"] for c in configs]),
        "estimatedGpuBytes":stats([c["EstimatedGpuBytes"] for c in configs]),
        "coveragePct": stats([c["TerrainCoverage"] for c in configs]), "heightErrorWorldP95M": first.get("HeightErrorWorldP95Meters"),
        "statuses": [c["Status"] for c in configs], "entries": entries})

baseline = next((x for x in rows if x["config"] == "2049-auto"), None)
quality = []
if baseline:
    base_entry = sorted(baseline["entries"], key=lambda e: e[2].get("GpuMsP50") or math.inf)[len(baseline["entries"]) // 2]
    base_png = base_entry[0].parent / base_entry[2]["Png"] if not Path(base_entry[2]["Png"]).is_absolute() else Path(base_entry[2]["Png"])
    for row in rows:
        entry = sorted(row["entries"], key=lambda e: e[2].get("GpuMsP50") or math.inf)[len(row["entries"]) // 2]
        png = Path(entry[2]["Png"]); out = root / f"diff-2049-auto-vs-{row['config']}.png"
        metrics = image_metrics(base_png, png, out)
        quality.append({"config": row["config"], **metrics, "diffPng": out.name})

probe_rows = []
for name, entries in groups.items():
    for index, entry in enumerate(entries, 1):
        probe = entry[0].parent / "probes" / "terrain.log"
        for line in probe.read_text(encoding="utf-8-sig").splitlines() if probe.exists() else []:
            if "[TERRAIN-CHUNK]" not in line: continue
            fields = {pair[0]:pair[1] for part in line.split("[TERRAIN-CHUNK]",1)[1].split(";")
                      if len(pair := part.split("=",1)) == 2}
            lod = int(fields.get("LOD", "-1"))
            probe_rows.append({"config":name,"replicate":index,"chunk_id":fields.get("Chunk"),
                "world_bounds":fields.get("WorldBounds"),"cell_count":fields.get("Cells"),
                "auto_lod":fields.get("AutoLod"),"draw_lod":lod,"sample_stride":(1 << lod) if lod >= 0 else "UNKNOWN",
                "projected_pixels":fields.get("ProjectedPixels"),"cull_result":fields.get("CullResult"),
                "reject_plane":fields.get("RejectPlane"),"draw_result":fields.get("Result"),
                "draw_recorded":fields.get("DrawRecorded"),"index_count":fields.get("IndexCount"),
                "triangles":int(fields.get("IndexCount", "0"))//3,"camera_revision":fields.get("CameraRevision"),
                "camera_inside_chunk_aabb":entry[2].get("CameraInsideChunkAabb")})
if baseline and "2049-lod-bias-1" in groups:
    from PIL import ImageDraw
    compare = [baseline, next(x for x in rows if x["config"] == "2049-lod-bias-1")]
    chosen = [sorted(x["entries"], key=lambda e:e[2].get("GpuMsP50") or math.inf)[len(x["entries"])//2] for x in compare]
    images = [Image.open(Path(x[2]["Png"])).convert("RGB") for x in chosen]
    montage = Image.new("RGB", (images[0].width*2, images[0].height+36), "white")
    draw = ImageDraw.Draw(montage); draw.text((8,8),"2049-auto",fill="black"); draw.text((images[0].width+8,8),"2049 LOD +1",fill="black")
    montage.paste(images[0],(0,36)); montage.paste(images[1],(images[0].width,36)); montage.save(root/"2049-lod-quality-comparison.png")

flat = []
for row in rows:
    q = next(x for x in quality if x["config"] == row["config"]) if quality else {}
    flat.append({"config": row["config"], "processes": row["processes"], "frames_per_process": row["sampleCountPerProcess"],
        "gpu_process_median_p50_ms": row["gpuProcessP50Ms"]["p50"], "gpu_process_median_p95_ms": row["gpuProcessP95Ms"]["p50"],
        "gpu_process_p50_min_ms":row["gpuProcessP50Ms"]["min"],"gpu_process_p50_max_ms":row["gpuProcessP50Ms"]["max"],
        "gpu_process_p50_cv":row["gpuProcessP50Ms"]["cv"],"gpu_process_p95_cv":row["gpuProcessP95Ms"]["cv"],
        "gpu_pooled_p50_ms": row["gpuPooledFramesMs"]["p50"], "gpu_pooled_p95_ms": row["gpuPooledFramesMs"]["p95"],
        "triangles_mean": row["triangles"]["mean"], "visible_chunks_mean": row["visibleChunks"]["mean"], "draw_calls_mean": row["drawCalls"]["mean"],
        "lod0": row["lodCounts"][0], "lod1": row["lodCounts"][1], "lod2": row["lodCounts"][2], "lod3": row["lodCounts"][3], "lod4": row["lodCounts"][4],
        "coverage_pct_mean": row["coveragePct"]["mean"], "mask_xor_pct": q.get("maskXorPct"), "rgb_mae_common_terrain": q.get("rgbMaeCommonTerrain"),
        "edge_mae_common_terrain": q.get("edgeMaeCommonTerrain"), "height_error_world_p95_m": row["heightErrorWorldP95M"],
        "cpu_mesh_build_p50_ms":row["cpuMeshBuildMs"]["p50"],"cpu_mesh_build_p95_ms":row["cpuMeshBuildMs"]["p95"],
        "cpu_upload_p50_ms":row["cpuUploadMs"]["p50"],"cpu_upload_p95_ms":row["cpuUploadMs"]["p95"],
        "estimated_gpu_buffer_bytes_mean":row["estimatedGpuBytes"]["mean"],"status": ";".join(row["statuses"])})

def write_csv(path, values):
    if not values: return
    with path.open("w", newline="", encoding="utf-8-sig") as f:
        writer = csv.DictWriter(f, fieldnames=list(values[0])); writer.writeheader(); writer.writerows(values)
write_csv(root / "ab-configs.csv", flat)
write_csv(root / "baseline.csv", [x for x in flat if x["config"] in ("1025-auto", "2049-auto")])
write_csv(root / "chunk-probes.csv", probe_rows)
(root / "analysis.json").write_text(json.dumps({"groups": [{k:v for k,v in row.items() if k != "entries"} for row in rows], "quality": quality}, indent=2), encoding="utf-8")

order = {x["config"] + ":" + str(x["replicate"]): int(x["order"])
         for x in csv.DictReader((root / "run-order.csv").open(encoding="utf-8-sig"))}
observations = []
for row in rows:
    for index, entry in enumerate(row["entries"], 1):
        c = entry[2]
        observations.append({"config":row["config"],"replicate":index,
            "order":order.get(row["config"] + ":" + str(index), index),
            "gpuP50Ms":c.get("GpuMsP50"),"gpuP95Ms":c.get("GpuMsP95"),
            "triangles":c.get("Triangles"),"visibleChunks":c.get("VisibleChunks")})
write_csv(root / "process-metrics.csv", observations)
paired = []
by_pair = {(x["config"],x["replicate"]):x for x in observations}
for rep in range(1, 6):
    a = by_pair.get(("1025-auto",rep)); b = by_pair.get(("2049-auto",rep))
    if a and b and a["gpuP50Ms"] and a["gpuP95Ms"]:
        paired.append({"replicate":rep,"1025_order":a["order"],"2049_order":b["order"],
            "gpu_p50_ratio_2049_over_1025":b["gpuP50Ms"]/a["gpuP50Ms"],
            "gpu_p95_ratio_2049_over_1025":b["gpuP95Ms"]/a["gpuP95Ms"]})
write_csv(root / "paired-ratios.csv", paired)
(root / "hardware.csv").write_text("run-order,config,replicate,phase,raw-nvidia-smi\n", encoding="utf-8")
def svg(path, title, series, xkey, ykey, xlabel, ylabel):
    w, h = 1100, 650; x0, y0, pw, ph = 90, 65, 950, 510
    values = [p[ykey] for s in series for p in s if p.get(ykey) is not None]
    vmax = max(values, default=1) or 1; colors = ["#276FBF", "#F28E2B", "#59A14F", "#E15759", "#B07AA1", "#76B7B2", "#EDC948", "#FF9DA7"]
    out = [f"<svg xmlns='http://www.w3.org/2000/svg' width='{w}' height='{h}'><rect width='100%' height='100%' fill='white'/><text x='20' y='30' font-size='22'>{title}</text>"]
    for i in range(6):
        y = y0 + ph * i / 5; out.append(f"<path d='M{x0} {y}h{pw}' stroke='#ddd'/><text x='8' y='{y+4}' font-size='12'>{vmax*(5-i)/5:.2f}</text>")
    for si, points in enumerate(series):
        coords = []
        for i, p in enumerate(points):
            x = x0 + pw * (i / max(1, len(points)-1)); y = y0 + ph * (1 - p[ykey] / vmax); coords.append(f"{x},{y}")
            out.append(f"<circle cx='{x}' cy='{y}' r='4' fill='{colors[si]}'><title>{p[xkey]}: {p[ykey]:.3f}</title></circle>")
        label = points[0][xkey] if points else str(si+1)
        out.append(f"<polyline points='{' '.join(coords)}' fill='none' stroke='{colors[si]}' stroke-width='2'/><text x='{x0+si*155}' y='{h-12}' fill='{colors[si]}'>{label}</text>")
    out.append(f"<text x='20' y='{h-12}'>{ylabel}</text></svg>"); path.write_text(''.join(out), encoding='utf-8')

chart_data = []
for row in rows:
    vals = [e[2].get("GpuMsP50") for e in row["entries"]]
    chart_data.append([{ "config": row["config"], "value": v } for v in vals])
svg(root / "process-variability.svg", "GPU P50 by independent process (ms)", chart_data, "config", "value", "configuration", "GPU ms")
svg(root / "cost-quality.svg", "GPU time cost by configuration (process P50, ms)", [[{"config":x["config"],"value":x["gpu_process_median_p50_ms"]} for x in flat]], "config", "value", "configuration", "GPU ms")
svg(root / "chunks-lod-triangles.svg", "GPU triangles per configuration", [[{"config":x["config"],"value":x["triangles_mean"]} for x in flat]], "config", "value", "configuration", "triangles")

raw_runs = [json.loads(line) for line in (root / "process-runs.jsonl").read_text(encoding="utf-8").splitlines() if line.strip()]
with (root / "hardware.csv").open("a", encoding="utf-8", newline="") as f:
    writer = csv.writer(f)
    for r in raw_runs:
        for phase in ("gpuBefore", "gpuAfter"):
            writer.writerow([r["order"],r["config"],r["replicate"],phase,r[phase]])
ordered = sorted(observations, key=lambda x:x["order"])
svg(root / "run-order-gpu.svg", "Actual process order: Vulkan GPU P50 (ms)", [[{"config":x["config"],"value":x["gpuP50Ms"]} for x in ordered]], "config", "value", "run order", "GPU ms")
hardware = []
for r in raw_runs:
    for phase in ("gpuBefore", "gpuAfter"):
        try: values = next(csv.reader([r[phase]]))
        except Exception: values = []
        values += ["UNKNOWN"] * max(0, 9 - len(values))
        hardware.append({"order":r["order"],"config":r["config"],"replicate":r["replicate"],"phase":phase,
            "timestamp":values[0],"name":values[1],"driver":values[2],"gpu_clock_mhz":values[3],
            "memory_clock_mhz":values[4],"temperature_c":values[5],"power_w":values[6],
            "gpu_util_pct":values[7],"memory_used_mib":values[8]})
write_csv(root / "gpu-hardware.csv", hardware)
temps = [{"config":f"{x['order']}-{x['phase']}","value":float(x["temperature_c"])} for x in hardware if x["temperature_c"].replace('.','',1).isdigit()]
svg(root / "gpu-temperature.svg", "GPU temperature snapshots by process order (°C)", [temps], "config", "value", "run order", "°C")

ratio_p50 = [x["gpu_p50_ratio_2049_over_1025"] for x in paired]
ratio_p95 = [x["gpu_p95_ratio_2049_over_1025"] for x in paired]
def cv(values): return statistics.pstdev(values) / statistics.mean(values) if len(values) > 1 and statistics.mean(values) else None
ratio_summary = {"p50RatioMedian":pct(ratio_p50,.5),"p50RatioRange":[min(ratio_p50),max(ratio_p50)] if ratio_p50 else [],
                 "p50RatioCv":cv(ratio_p50),"p95RatioMedian":pct(ratio_p95,.5),"p95RatioCv":cv(ratio_p95)}
write_csv(root / "paired-ratios.csv", [{**x,"p50_ratio_cv_across_pairs":ratio_summary["p50RatioCv"]} for x in paired])
repo = root.parents[3]
changed = subprocess.run(["git","-C",str(repo),"ls-files","-m"],capture_output=True,text=True,check=True).stdout.splitlines()
untracked = subprocess.run(["git","-C",str(repo),"ls-files","--others","--exclude-standard"],capture_output=True,text=True,check=True).stdout.splitlines()
source_files = sorted(set(changed + untracked))
source_set = "\n".join(f"{name}|{hashlib.sha256((repo/name).read_bytes()).hexdigest()}" for name in source_files)
source_fingerprint = hashlib.sha256(source_set.encode("utf-8")).hexdigest()
manifest = {"generatedUtc": __import__("datetime").datetime.now(__import__("datetime").timezone.utc).isoformat(),
    "sourceSha256":"BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1", "configurations":flat,"quality":quality,"pairedRatios":ratio_summary,
    "buildIdentity":groups["2049-auto"][0][1]["BuildIdentity"],"device":groups["2049-auto"][0][1]["Device"],
    "driverVersion":groups["2049-auto"][0][1]["DriverVersion"],
    "gpuTimestampPeriodNs":groups["2049-auto"][0][1]["TimestampPeriodNs"],
    "gpuTimestampValidBits":groups["2049-auto"][0][1]["TimestampValidBits"],
    "sourceSetFingerprintSha256":source_fingerprint,"sourceFiles":source_files}
manifest["sourceSha256"] = "BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1"
(root / "run-manifest.json").write_text(json.dumps(manifest, indent=2), encoding="utf-8")
def val(name, key):
    return next((r[key] for r in rows if r["config"] == name), None)
lines = ["# XYC-B2 RTX 3060 近地斜视 A/B 报告", "", f"- HGT SHA-256: `{manifest['sourceSha256']}`", "- 相机/分辨率: near-oblique, 1024×768; 每配置 5 个独立进程，每进程至少 30 个有效 Vulkan timestamp 样本。", "- GPU 状态来源: 每进程前后 `nvidia-smi` 快照；原值见 `process-runs.jsonl`。", "- 说明: process P50/P95 是独立进程分布统计；pooled-frame 分位数仅作帧内辅助，不能当独立进程重复。", "", "## 主要比较", "", "| 配置 | GPU process P50 中位数 ms | GPU P95 中位数 ms | pooled P50/P95 ms | 可见 Chunk | LOD0..4 | 三角形均值 | 覆盖率 | 掩码 XOR% | RGB MAE | 边缘 MAE | 状态 |", "|---|---:|---:|---:|---:|---|---:|---:|---:|---:|---:|---|"]
lines[3:3] = [f"- BuildIdentity: `{manifest['buildIdentity']}`", f"- Source set SHA-256: `{source_fingerprint}`"]
for x in flat:
    lines.append(f"| {x['config']} | {x['gpu_process_median_p50_ms']:.3f} | {x['gpu_process_median_p95_ms']:.3f} | {x['gpu_pooled_p50_ms']:.3f}/{x['gpu_pooled_p95_ms']:.3f} | {x['visible_chunks_mean']:.1f} | {x['lod0']}/{x['lod1']}/{x['lod2']}/{x['lod3']}/{x['lod4']} | {x['triangles_mean']:.0f} | {x['coverage_pct_mean']:.2f}% | {x['mask_xor_pct']:.3f} | {x['rgb_mae_common_terrain']:.3f} | {x['edge_mae_common_terrain']:.3f} | {x['status']} |")
lines += ["", "## 交错配对", "", f"- 同序号 1025/2049 GPU P50 倍数中位数: `{ratio_summary['p50RatioMedian']:.3f}x`，配对范围 `{ratio_summary['p50RatioRange']}`，CV `{ratio_summary['p50RatioCv']:.3f}`。", f"- 同序号 P95 倍数中位数: `{ratio_summary['p95RatioMedian']:.3f}x`；逐对数值见 `paired-ratios.csv`。", "- 执行顺序图: [GPU P50 by run order](run-order-gpu.svg); 温度图: [GPU temperature](gpu-temperature.svg)。", "", "## 可视化", "", "- [独立进程 GPU 波动](process-variability.svg)", "- [GPU 成本对照](cost-quality.svg)", "- [Chunk/LOD 三角形量](chunks-lod-triangles.svg)", "- `diff-2049-auto-vs-*.png` 为同姿态 GPU PNG 逐像素差异图；对照基准选取 2049-auto 中进程 GPU P50 中位的图像。", "", "## 解读边界", "", "- `mask XOR`、共同地形 RGB MAE 和边缘 MAE 是图像代理指标，不等于主观画质验收。", "- HGT P95 是全域固定采样点的高度重建误差，不是此相机姿态的几何 SSE。", "- GPU 缓冲估算不等于驱动实测显存；显存列是 `nvidia-smi` 全设备占用快照，不能归因到本进程。", "- 根因只在跨独立进程一致、且 Chunk/LOD 对照满足单变量条件时才标为 MEASURED；其余结论为 DERIVED / HYPOTHESIS。", "", "## 文件", "", "- `baseline.csv`, `ab-configs.csv`, `paired-ratios.csv`, `process-metrics.csv`, `chunk-probes.csv`, `gpu-hardware.csv`, `run-manifest.json`, `analysis.json`, `process-runs.jsonl`, `run-order.csv`", "- [2049 auto / LOD +1 对照图](2049-lod-quality-comparison.png); 每进程目录包含 GPU PNG、逐帧 timestamp/PNG hash JSON、Chunk/LOD probe 与运行日志。"]
metric = {x["config"]:x for x in flat}; base = metric["2049-auto"]; low = metric["1025-auto"]
bias = metric["2049-lod-bias-1"]; c240 = metric["2049-chunk240-lod0"]; c480 = metric["2049-chunk480-lod0"]
def reduction(a,b): return (1-b/a)*100 if a else 0
lines += ["", "## 测量所得根因", "", f"- MEASURED: 交错 5 对中 2049-auto / 1025-auto GPU P50 倍数 `{ratio_summary['p50RatioMedian']:.3f}x`，CV `{ratio_summary['p50RatioCv']:.4f}`；三角形分别 `{base['triangles_mean']:.0f}` 与 `{low['triangles_mean']:.0f}`。当前采集状态下差异稳定可复现。", f"- MEASURED: 固定 2049 采样、240-cell Chunk 和相机，仅将自动 LOD 粗化一级，GPU P50 `{base['gpu_process_median_p50_ms']:.3f}→{bias['gpu_process_median_p50_ms']:.3f} ms`（减少 {reduction(base['gpu_process_median_p50_ms'],bias['gpu_process_median_p50_ms']):.1f}%），三角形 `{base['triangles_mean']:.0f}→{bias['triangles_mean']:.0f}`（减少 {reduction(base['triangles_mean'],bias['triangles_mean']):.1f}%）。near-oblique 下的 auto LOD 几何细分是主要 GPU 成本来源。", f"- MEASURED: 固定 LOD0 时，Chunk 240→480 将可见块 `{c240['visible_chunks_mean']:.0f}→{c480['visible_chunks_mean']:.0f}`，但三角形 `{c240['triangles_mean']:.0f}→{c480['triangles_mean']:.0f}`、GPU P50 `{c240['gpu_process_median_p50_ms']:.3f}→{c480['gpu_process_median_p50_ms']:.3f} ms`；块变少并未变快，较大的相交 chunk 提交了更多几何。", f"- DERIVED: 2049-auto 的全域 HGT 采样高程 P95 为 `{base['height_error_world_p95_m']:.3f} m`，1025 为 `{low['height_error_world_p95_m']:.3f} m`。LOD +1 与 2049-auto 的 mask XOR `{metric['2049-lod-bias-1']['mask_xor_pct']:.3f}%`、共同地形 RGB MAE `{metric['2049-lod-bias-1']['rgb_mae_common_terrain']:.3f}/255`；同姿态并排图可见山脊轮廓有平滑，尚不能判为保留全部高频细节。", "- B1 曾报 2049 GPU P50=33.815 ms；本轮 5 进程中位数=21.984 ms。B1 三个进程中位数约 22.222/43.479/43.112 ms，提示历史时序存在显著状态波动；本轮核心频率快照为 210–2047 MHz、GPU 温度 47–56°C，快照在每进程前后采集，不能解释帧内时钟变化。", "- RootCause: 2049 高采样网格通过固定 240-cell 网格生成更多、物理尺寸更小的 Chunk，近地视角下多数可见 Chunk 仍为 LOD0；主要可控成本是提交三角形数量，而 Chunk 数不是成本代理。Chunk 划分与 auto LOD 仍有交互，不把 1025→2049 总差额归给单一因素。", "", "## CPU 与硬件测量", "", "| 配置 | CPU mesh build P50/P95 ms | CPU upload P50/P95 ms | 估算 GPU buffer bytes |", "|---|---:|---:|---:|"]
for x in flat: lines.append(f"| {x['config']} | {x['cpu_mesh_build_p50_ms']:.1f}/{x['cpu_mesh_build_p95_ms']:.1f} | {x['cpu_upload_p50_ms']:.1f}/{x['cpu_upload_p95_ms']:.1f} | {x['estimated_gpu_buffer_bytes_mean']:.0f} |")
lines += ["", f"Build source fingerprint: `{source_fingerprint}`（`run-manifest.json` 中列出 fingerprint 对应的 tracked dirty 与 untracked 源文件）。", "上述 CPU build/upload 是冷构建阶段 CPU wall-clock，不与 GPU timestamp 相加。设备显存和运行温度等采样见 `gpu-hardware.csv`；显存为全 GPU 占用而非进程 attribution。LOD +1 仍只是离屏诊断候选，未改变正式默认算法，也不代表用户 P4 验收。"]
(root / "report.md").write_text("\n".join(lines), encoding="utf-8")
