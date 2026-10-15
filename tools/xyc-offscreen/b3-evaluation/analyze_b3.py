#!/usr/bin/env python3
"""Offline XYC-B3 evidence normalizer and Pareto report generator (stdlib only)."""
import argparse
import csv
import json
import math
import statistics
from pathlib import Path
from xml.sax.saxutils import escape


def percentile(values, q):
    xs = sorted(values)
    if not xs:
        return None
    pos = (len(xs) - 1) * q
    lo = int(pos)
    hi = min(lo + 1, len(xs) - 1)
    return xs[lo] + (xs[hi] - xs[lo]) * (pos - lo)


def summarize(values):
    xs = [float(x) for x in values if x is not None]
    return {"n": len(xs), "p50": percentile(xs, .5), "p95": percentile(xs, .95),
            "min": min(xs) if xs else None, "max": max(xs) if xs else None,
            "mean": statistics.mean(xs) if xs else None}


def write_csv(path, rows):
    keys = list(dict.fromkeys(k for row in rows for k in row))
    with path.open("w", newline="", encoding="utf-8-sig") as f:
        if not keys:
            f.write("\n")
            return
        w = csv.DictWriter(f, fieldnames=keys, extrasaction="ignore")
        w.writeheader()
        w.writerows(rows)


def svg(path, title, x_label, y_label, points, x_key, y_key, color="#276FBF"):
    valid = [p for p in points if isinstance(p.get(x_key), (int, float)) and isinstance(p.get(y_key), (int, float))]
    width, height, left, top, pw, ph = 900, 560, 90, 65, 760, 410
    xmax = max((p[x_key] for p in valid), default=1) or 1
    ymax = max((p[y_key] for p in valid), default=1) or 1
    out = [f"<svg xmlns='http://www.w3.org/2000/svg' width='{width}' height='{height}'>",
           "<rect width='100%' height='100%' fill='white'/>",
           f"<text x='20' y='32' font-size='21'>{escape(title)}</text>"]
    for i in range(6):
        y = top + ph * i / 5
        out.append(f"<path d='M{left} {y:.1f}h{pw}' stroke='#ddd'/>")
    out += [f"<path d='M{left} {top}v{ph}h{pw}' fill='none' stroke='#333'/>",
            f"<text x='{left + pw/2}' y='{height-16}' text-anchor='middle'>{escape(x_label)}</text>",
            f"<text x='18' y='{top + ph/2}' transform='rotate(-90 18 {top + ph/2})' text-anchor='middle'>{escape(y_label)}</text>"]
    for p in valid:
        x = left + pw * p[x_key] / xmax
        y = top + ph * (1 - p[y_key] / ymax)
        label = escape(str(p.get("label", p.get("configuration", ""))))
        out.append(f"<circle cx='{x:.2f}' cy='{y:.2f}' r='5' fill='{color}'><title>{label}: {p[x_key]:.4g}, {p[y_key]:.4g}</title></circle>")
        out.append(f"<text x='{x+7:.2f}' y='{y-7:.2f}' font-size='11'>{label}</text>")
    out.append("</svg>")
    path.write_text("\n".join(out), encoding="utf-8")


def main():
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("input", type=Path, help="Normalized JSON evidence input")
    ap.add_argument("output", type=Path, help="New output directory; must not exist")
    args = ap.parse_args()
    if args.output.exists():
        raise SystemExit(f"Refusing to overwrite existing output directory: {args.output}")
    doc = json.loads(args.input.read_text(encoding="utf-8-sig"))
    if doc.get("schemaVersion") != "xyc-b3-evaluation/v1":
        raise SystemExit("Unsupported schemaVersion")
    configs = doc.get("configurations", [])
    args.output.mkdir(parents=True)
    summaries, processes, pairs = [], [], []
    for c in configs:
        runs = c.get("processRuns", [])
        p50 = [r.get("gpuFrameP50Ms") for r in runs]
        p95 = [r.get("gpuFrameP95Ms") for r in runs]
        row = {"configuration": c["name"], "family": c.get("family"), "status": c.get("status"),
               "evidenceClass": c.get("evidenceClass"), "processCount": len(runs),
               "independentProcessP50Ms": summarize(p50), "independentProcessP95Ms": summarize(p95),
               "triangles": summarize([r.get("triangles") for r in runs]),
               "visibleChunks": summarize([r.get("visibleChunks") for r in runs]),
               "drawCalls": summarize([r.get("drawCalls") for r in runs]),
               "estimatedGpuBytes": summarize([r.get("estimatedGpuBytes") for r in runs]),
               "meshError": c.get("meshError", {}), "screenError": c.get("screenError", {}),
               "pixels": c.get("pixels", {}), "sourceArtifact": c.get("sourceArtifact")}
        summaries.append(row)
        for i, run in enumerate(runs, 1):
            processes.append({"configuration": c["name"], "family": c.get("family"),
                              "replicate": run.get("replicate", i), "order": run.get("order"),
                              "gpuFrameP50Ms": run.get("gpuFrameP50Ms"), "gpuFrameP95Ms": run.get("gpuFrameP95Ms"),
                              "validFrames": run.get("validFrames"), "triangles": run.get("triangles"),
                              "visibleChunks": run.get("visibleChunks"), "drawCalls": run.get("drawCalls"),
                              "estimatedGpuBytes": run.get("estimatedGpuBytes")})
    by_name = {c["name"]: c for c in configs}
    baseline_name = doc.get("pairing", {}).get("baseline")
    for c in configs:
        if c["name"] == baseline_name:
            continue
        left, right = by_name.get(baseline_name), c
        if not left or (left.get("family") != right.get("family") and
                        not doc.get("pairing", {}).get("allowCrossFamily", False)):
            continue
        left_runs = {r.get("replicate"): r for r in left.get("processRuns", [])}
        for r in right.get("processRuns", []):
            b = left_runs.get(r.get("replicate"))
            if b and b.get("gpuFrameP50Ms") and r.get("gpuFrameP50Ms"):
                pairs.append({"baseline": baseline_name, "configuration": c["name"],
                              "replicate": r.get("replicate"), "pairingBasis": "same replicate index; validate run order/environment",
                              "gpuP50Ratio": r["gpuFrameP50Ms"] / b["gpuFrameP50Ms"],
                              "baselineOrder": b.get("order"), "candidateOrder": r.get("order")})
    write_csv(args.output / "processes.csv", processes)
    write_csv(args.output / "summaries.csv", [{**r, "independentProcessP50Ms": r["independentProcessP50Ms"]["p50"],
              "independentProcessP95Ms": r["independentProcessP95Ms"]["p50"],
              "trianglesMean": r["triangles"]["mean"], "meshErrorP95M": r["meshError"].get("finalMeshWorldP95M"),
              "screenErrorP95Px": r["screenError"].get("p95Px"), "pixelEdgeMae": r["pixels"].get("edgeMae"),
              "visibleChunkP95MetersMax": r["meshError"].get("visibleChunkP95MetersMax"),
              "visibleChunkMaxErrorMeters": r["meshError"].get("visibleChunkMaxErrorMeters"),
              "pixelMaskXorPct": r["pixels"].get("maskXorPct"), "pixelRgbMae": r["pixels"].get("rgbMae")}
             for r in summaries])
    write_csv(args.output / "paired-ratios.csv", pairs)
    (args.output / "analysis.json").write_text(json.dumps({"input": str(args.input), "configurations": summaries,
           "pairedRatios": pairs, "pareto": "only rows with measured cost and valid comparable error metrics are eligible"}, indent=2), encoding="utf-8")
    gpu_points = [{"label": r["configuration"], "x": r["independentProcessP50Ms"]["p50"], "y": r["triangles"]["mean"]} for r in summaries]
    svg(args.output / "gpu-cost.svg", "GPU timestamp cost by configuration", "GPU process P50 median (ms)", "Mean triangles", gpu_points, "x", "y")
    pareto_points = []
    for r in summaries:
        err = r["meshError"].get("finalMeshWorldP95M")
        cost = r["independentProcessP50Ms"]["p50"]
        if r.get("family") == "SSE" and r.get("status") == "MEASURED" and err is not None and cost is not None:
            pareto_points.append({"label": r["configuration"], "x": cost, "y": err})
    svg(args.output / "pareto-final-mesh.svg", "Eligible measured GPU cost vs final mesh error", "GPU process P50 median (ms)", "Final mesh world error P95 (m)", pareto_points, "x", "y", "#E15759")
    proxy_points = [{"label": r["configuration"], "x": r["independentProcessP50Ms"]["p50"],
                     "y": r["meshError"].get("visibleChunkP95MetersMax")}
                    for r in summaries if r["meshError"].get("visibleChunkP95MetersMax") is not None]
    svg(args.output / "gpu-vs-visible-chunk-error.svg", "GPU cost vs largest visible-chunk P95 (diagnostic proxy)",
        "GPU process P50 median (ms)", "Largest per-chunk final-mesh P95 (m)", proxy_points, "x", "y", "#F28E2B")
    error_points = []
    for r in summaries:
        me = r["meshError"]
        for key, label in (("sourceToHeightfieldP95M", "source→heightfield"), ("heightfieldToFinalMeshP95M", "heightfield→mesh"), ("finalMeshWorldP95M", "source→final mesh")):
            if me.get(key) is not None:
                error_points.append({"label": f"{r['configuration']} {label}", "x": len(error_points)+1, "y": me[key]})
    svg(args.output / "error-layers.svg", "Error layers (only supplied values)", "Evidence item index", "World error P95 (m)", error_points, "x", "y", "#59A14F")
    names = [c["name"] for c in configs if c.get("family") == "SSE"]
    lines = ["# XYC-B3 offline evaluation", "", f"Input: `{args.input}`", "",
             "Only rows marked **NOT RUN** lack B3 measurements. B2 numbers remain separate historical references.", "",
             "## Configuration status", "", "| Configuration | Family | Status | Evidence | Independent processes | GPU P50 median ms | Visible-footprint final mesh P95 m | Mask XOR % |", "|---|---|---|---|---:|---:|---:|---:|"]
    for r in summaries:
        lines.append(f"| {r['configuration']} | {r.get('family')} | {r.get('status')} | {r.get('evidenceClass')} | {r['processCount']} | {r['independentProcessP50Ms']['p50']} | {r['meshError'].get('finalMeshWorldP95M')} | {r['pixels'].get('maskXorPct')} |")
    lines += ["", "## Figures", "", "- [GPU cost and triangles](gpu-cost.svg)", "- [Eligible final-mesh Pareto](pareto-final-mesh.svg)", "- [Visible-chunk error proxy](gpu-vs-visible-chunk-error.svg)", "- [Separated error layers](error-layers.svg)", "", "## Interpretation limits", "",
              "- GPU frame samples are nested within a process. Independent-process summaries use one process statistic per process; pooled frame samples are not independent process replications.",
              "- Source HGT→heightfield reconstruction error is not final rendered mesh error. The three error fields are separate and must share a documented world-XY sample protocol before comparison.",
              "- `estimatedGpuBytes` is an estimate, not measured resident VRAM. Device-wide `nvidia-smi` memory is not process attribution.",
              "- Pareto eligibility requires measured SSE status and final mesh error; derived estimates and B2 reference rows are excluded.",
              "- `visibleChunkP95MetersMax` is the largest per-chunk P95 among visible chunks. It is not a pooled whole-view P95 and is plotted separately from the final-mesh Pareto.",
              "- `finalMeshWorldP95M` for the B3 matrix is pooled exactly over NASADEM source samples in the union of visible chunk footprints using that process's measured selected LODs. It excludes invisible terrain and is not a whole-map statistic.",
              "- SSE configurations listed: " + (", ".join(names) if names else "none") + "."]
    (args.output / "report.md").write_text("\n".join(lines) + "\n", encoding="utf-8")


if __name__ == "__main__":
    main()
