#!/usr/bin/env python3
"""Normalize Parent-owned XYC-B3 Vulkan process outputs for offline analysis."""
import argparse
import csv
import hashlib
import json
from pathlib import Path

from PIL import Image, ImageChops, ImageEnhance, ImageFilter, ImageStat

SKY = (89, 140, 204)
CONFIGS = [("2049-auto", "baseline"), ("2049-lod-bias-1", "lod-bias-1"),
           ("SSE-0.5px", "sse-0.5"), ("SSE-1px", "sse-1"),
           ("SSE-2px", "sse-2"), ("SSE-4px", "sse-4")]
ERROR_FILES = {"baseline": "selected-error-baseline.json", "lod-bias-1": "selected-error-lodplus1.json",
               "sse-0.5": "selected-error-sse05.json", "sse-1": "selected-error-sse1.json",
               "sse-2": "selected-error-sse2.json", "sse-4": "selected-error-sse4.json"}


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest().upper()


def load_process(root, name, rep):
    path = root / name / f"replicate-{rep}" / f"run-{rep}" / "run-manifest.json"
    doc = json.loads(path.read_text(encoding="utf-8-sig"))
    run = doc["Configurations"][0]
    png = Path(run["Png"])
    if sha(png) != run["PngSha256"]:
        raise ValueError(f"PNG hash mismatch: {png}")
    return path, doc, run, png


def compare(base_path, candidate_path, diff_path):
    a = Image.open(base_path).convert("RGB")
    b = Image.open(candidate_path).convert("RGB")
    if a.size != b.size:
        raise ValueError("GPU image dimensions differ")
    diff = ImageChops.difference(a, b)
    stats = ImageStat.Stat(diff)
    changed = 0
    mask_changed = 0
    edge_diff = ImageChops.difference(a.convert("L").filter(ImageFilter.FIND_EDGES),
                                      b.convert("L").filter(ImageFilter.FIND_EDGES))
    pixels_a, pixels_b = list(a.getdata()), list(b.getdata())
    for p, (x, y) in enumerate(zip(pixels_a, pixels_b)):
        if max(abs(x[i] - y[i]) for i in range(3)) > 8:
            changed += 1
        if (x == SKY) != (y == SKY):
            mask_changed += 1
    ImageEnhance.Contrast(diff).enhance(3).save(diff_path)
    count = a.width * a.height
    return {"rgbMae": sum(stats.mean) / 3, "changedPixelPct": changed * 100 / count,
            "maskXorPct": mask_changed * 100 / count,
            "edgeMae": ImageStat.Stat(edge_diff).mean[0], "diffPng": str(diff_path)}


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("run", type=Path)
    ap.add_argument("error_table", type=Path)
    ap.add_argument("output", type=Path)
    args = ap.parse_args()
    if args.output.exists():
        raise SystemExit(f"Refusing to overwrite: {args.output}")
    order = {}
    with (args.run / "run-order.csv").open(encoding="utf-8-sig", newline="") as stream:
        for row in csv.DictReader(stream):
            order[(row["config"], int(row["replicate"]))] = int(row["order"])
    table_doc = json.loads(args.error_table.read_text(encoding="utf-8-sig"))
    errors = {(e["ChunkX"], e["ChunkY"], e["Lod"]): e for e in table_doc["errors"]}
    configs = []
    actual = {}
    for label, directory in CONFIGS:
        mesh = json.loads((args.run / ERROR_FILES[directory]).read_text(encoding="utf-8-sig"))
        runs, pngs, mesh_chunks, selected_lods = [], {}, [], []
        for rep in range(1, 6):
            manifest, doc, run, png = load_process(args.run, directory, rep)
            runs.append({"replicate": rep, "order": order[(directory, rep)],
                "gpuFrameP50Ms": run["GpuMsP50"], "gpuFrameP95Ms": run["GpuMsP95"],
                "validFrames": run["ValidGpuSamples"], "triangles": run["Triangles"],
                "visibleChunks": run["VisibleChunks"], "drawCalls": run["DrawCalls"],
                "estimatedGpuBytes": run["EstimatedGpuBytes"], "pngSha256": run["PngSha256"],
                "manifestSha256": sha(manifest), "gpuBefore": (args.run / directory / f"replicate-{rep}" / "gpu-before.csv").read_text().strip(),
                "gpuAfter": (args.run / directory / f"replicate-{rep}" / "gpu-after.csv").read_text().strip()})
            pngs[rep] = png
            if rep == 1:
                for probe in run["ChunkProbes"]:
                    if probe["Result"] != "DRAW_RECORDED":
                        continue
                    x, y = map(int, probe["Chunk"].split(",")); lod = int(probe["Lod"])
                    row = errors.get((x, y, lod))
                    if row:
                        mesh_chunks.append(row)
                        selected_lods.append({"chunkX": x, "chunkY": y, "lod": lod})
        actual[directory] = (runs, pngs, mesh_chunks)
        family = "SSE" if directory.startswith("sse-") else ("AUTO" if directory == "baseline" else "LOD_BIAS")
        row = {"name": label, "family": family, "status": "MEASURED",
            "evidenceClass": "MEASURED_GPU_DERIVED_ERROR_SUMMARIES", "processRuns": runs,
            "meshError": {"sourceToHeightfieldP95M": mesh["sourceToRender"]["P95VisualMeters"], "heightfieldToFinalMeshP95M": None,
                "finalMeshWorldP95M": mesh["sourceToFinalMesh"]["P95VisualMeters"],
                "finalMeshWorldMaxM": mesh["sourceToFinalMesh"]["MaxVisualMeters"],
                "visibleFootprintSamples": mesh["sourceToFinalMesh"]["ValidCount"],
                "visibleChunkP95MetersMax": max((x["P95VisualErrorMeters"] for x in mesh_chunks), default=None),
                "visibleChunkMaxErrorMeters": max((x["MaxVisualErrorMeters"] for x in mesh_chunks), default=None),
                "visibleChunksWithError": len(mesh_chunks),
                "selectedChunkLods": selected_lods,
                "meaning": "pooled source-sample error over union of visible chunk footprints at measured selected LOD; excludes invisible terrain"},
            "screenError": {"p95Px": None, "validity": "projected per-chunk estimates are diagnostics, not guaranteed bound"},
            "pixels": {}, "sourceArtifact": str(args.run / directory)}
        if family == "SSE":
            row["thresholdPx"] = float(directory.removeprefix("sse-"))
        configs.append(row)
    base_png = actual["baseline"][1][1]
    quality_dir = args.output.parent / (args.output.stem + "-quality")
    quality_dir.mkdir(parents=True)
    for config, (_, directory) in zip(configs, CONFIGS):
        metrics = [compare(base_png, actual[directory][1][rep], quality_dir / f"{directory}-rep{rep}-diff.png")
                   for rep in range(1, 6)]
        config["pixels"] = {key: sum(m[key] for m in metrics) / len(metrics)
                             for key in ("rgbMae", "changedPixelPct", "maskXorPct", "edgeMae")}
        config["pixels"]["representativeDiffPng"] = metrics[0]["diffPng"]
    doc = {"schemaVersion": "xyc-b3-evaluation/v1", "dataset": {
        "sourceSha256": "BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1",
        "device": "NVIDIA GeForce RTX 3060", "buildIdentity": "HEAD=9429be62288cd58c56ff7a9b6608857afb4c6175;B3=1;DIRTY=YES"},
        "pairing": {"baseline": "2049-auto", "allowCrossFamily": True,
            "note": "same replicate index; execution order interleaved and hardware snapshots retained"},
        "errorTable": str(args.error_table), "configurations": configs}
    args.output.write_text(json.dumps(doc, indent=2), encoding="utf-8")
    print(f"Wrote {args.output}; quality images: {quality_dir}")


if __name__ == "__main__":
    main()
