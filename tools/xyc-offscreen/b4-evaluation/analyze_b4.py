#!/usr/bin/env python3
"""Independent offline analysis of the captured XYC-B3 SSE evidence."""
import csv, hashlib, json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
SRC = ROOT / "tools/xyc-offscreen/b3-evaluation/results/20261015-202400"
OUT = ROOT / "artifacts/diagnostics/xyc-b4/b-quality"
REPORT = Path(__file__).with_name("B-SSE-QUALITY-REPORT.md")

def pct(values, q):
    xs = sorted(values)
    if not xs: return None
    i = (len(xs)-1)*q; lo=int(i); hi=min(lo+1,len(xs)-1)
    return xs[lo]+(xs[hi]-xs[lo])*(i-lo)

def stats(values):
    return {"n":len(values),"p50":pct(values,.5),"p95":pct(values,.95),
            "p99":pct(values,.99),"max":max(values) if values else None}

def main():
    OUT.mkdir(parents=True,exist_ok=True)
    analysis=json.loads((SRC/"analysis.json").read_text(encoding="utf-8"))
    table=json.loads((SRC/"error-table.json").read_text(encoding="utf-8"))
    by_chunk={(e["ChunkX"],e["ChunkY"],e["Lod"]):e for e in table["errors"]}
    names={"2049-auto":"Auto","2049-lod-bias-1":"LOD+1","SSE-0.5px":"SSE 0.5 px",
           "SSE-1px":"SSE 1 px","SSE-2px":"SSE 2 px","SSE-4px":"SSE 4 px"}
    rows=[]; hot=[]
    for c in analysis["configurations"]:
        vis=c.get("meshError",{}).get("selectedChunkLods",[])
        chunks=[by_chunk[(v["chunkX"],v["chunkY"],v["lod"])]|v for v in vis]
        for e in chunks:
            hot.append({"configuration":names.get(c["configuration"],c["configuration"]),
                "chunkX":e["ChunkX"],"chunkY":e["ChunkY"],"lod":e["Lod"],
                "cellX0":e["ChunkX"]*240,"cellX1":min((e["ChunkX"]+1)*240,2048),
                "cellY0":e["ChunkY"]*240,"cellY1":min((e["ChunkY"]+1)*240,2048),
                "validSamples":e["ValidCount"],"p95M":e["P95VisualErrorMeters"],
                "maxM":e["MaxVisualErrorMeters"]})
        if c["configuration"] not in names: continue
        m=c["meshError"]; gpu=c["independentProcessP50Ms"]["p50"]
        rows.append({"name":names[c["configuration"]],"gpuP50Ms":gpu,
            "gpuP95Ms":c["independentProcessP95Ms"]["p50"],"triangles":c["triangles"]["p50"],
            "sourceToFinalP95M":m.get("finalMeshWorldP95M"),"sourceToFinalMaxM":m.get("finalMeshWorldMaxM"),
            "visibleFootprintSamples":m.get("visibleFootprintSamples"),
            "sourceToRenderP95M":m.get("sourceToHeightfieldP95M"),
            "renderToLodP95M":m.get("heightfieldToFinalMeshP95M"),
            "screenRgbMae":c["pixels"].get("rgbMae"),"screenChangedPct":c["pixels"].get("changedPixelPct"),
            "screenMaskXorPct":c["pixels"].get("maskXorPct"),"visibleChunks":m.get("selectedChunkLods",[])})
    baseline=next(r for r in rows if r["name"]=="Auto")
    for r in rows:
        r["gpuP50SavingsPct"]=(1-r["gpuP50Ms"]/baseline["gpuP50Ms"])*100
        r["sourceToFinalP95DeltaPct"]=(r["sourceToFinalP95M"]/baseline["sourceToFinalP95M"]-1)*100
        r["sourceToFinalErrorStats"]={"n":r["visibleFootprintSamples"],"p50M":None,
            "p95M":r["sourceToFinalP95M"],"p99M":None,"maxM":r["sourceToFinalMaxM"]}
    pareto=[]
    for a in rows:
        dominated=any(b["gpuP50Ms"]<=a["gpuP50Ms"] and b["sourceToFinalP95M"]<=a["sourceToFinalP95M"] and
            (b["gpuP50Ms"]<a["gpuP50Ms"] or b["sourceToFinalP95M"]<a["sourceToFinalP95M"]) for b in rows if b!=a)
        pareto.append({"name":a["name"],"gpuP50Ms":a["gpuP50Ms"],"sourceToFinalP95M":a["sourceToFinalP95M"],"dominated":dominated})
    stats_out={}
    for label in sorted({x["configuration"] for x in hot}):
        group=[x for x in hot if x["configuration"]==label]
        stats_out[label]={"chunkRecordCount":len(group),"chunkP95RecordDistributionM":stats([x["p95M"] for x in group]),
            "chunkMaxRecordDistributionM":stats([x["maxM"] for x in group]),
            "warning":"Quantiles summarize per-chunk summary records, not pooled source samples."}
    traj={"SSE 0.5 px":"trajectory-differences-sse05.json","SSE 1 px":"trajectory-differences-sse1.json",
          "SSE 2 px":"trajectory-differences-sse2.json","SSE 4 px":"trajectory-differences.json"}
    differences={k:json.loads((SRC/v).read_text(encoding="utf-8")) for k,v in traj.items()}
    result={"schema":"XYC-B4-SSE-QUALITY/1","sourceHeadClaim":"10af215415a33cb26c4fccf81cb7452ef800d70a",
        "sourceRunIdentityObserved":"HEAD=9429be62288cd58c56ff7a9b6608857afb4c6175; B3=VIS; DIRTY=YES",
        "coverageStatus":"UNCONFIRMED","sourceToRender":{"p50M":None,"p95M":10.370311111110823,"p99M":None,"maxM":None,
        "meaning":"HGT source to 2049 render heightfield; only P95 supplied; raw residuals unavailable"},
        "renderToLod":{"p50M":None,"p95M":None,"p99M":None,"maxM":None,
        "reason":"No separately sampled heightfield-to-selected-LOD residuals; source-to-final is not decomposable from stored summaries."},
        "configurations":rows,"pareto":pareto,"visibleChunkRecordStatistics":stats_out,
        "visibleChunkHotspots":sorted(hot,key=lambda x:(x["maxM"],x["p95M"]),reverse=True),
        "trajectoryPixelDifferences":differences,
        "quantilePolicy":"No P50/P99/Max are inferred where source samples or corresponding summaries are absent."}
    (OUT/"error-statistics.json").write_text(json.dumps(result,indent=2),encoding="utf-8")
    (OUT/"pareto-analysis.json").write_text(json.dumps({"objective":"minimize GPU P50 and pooled Source-to-final-mesh P95",
        "scope":"one near-oblique pose; visible footprint only","points":pareto,"coverageStatus":"UNCONFIRMED"},indent=2),encoding="utf-8")
    inputs=[SRC/"analysis.json",SRC/"error-table.json"]+[SRC/v for v in traj.values()]
    evidence={"task":"XYC-B4-B-SSE-QUALITY-ANALYSIS","execution":"offline Python analysis; no renderer/Vulkan/GPU started",
        "command":"python tools/xyc-offscreen/b4-evaluation/analyze_b4.py",
        "sourceHeadClaim":result["sourceHeadClaim"],"sourceRunIdentityObserved":result["sourceRunIdentityObserved"],
        "coverageStatus":"UNCONFIRMED","inputs":[{"path":str(x.relative_to(ROOT)),
        "sha256":hashlib.sha256(x.read_bytes()).hexdigest()} for x in inputs]}
    (OUT/"analysis-run.json").write_text(json.dumps(evidence,indent=2),encoding="utf-8")
    with (OUT/"visible-chunk-hotspots.csv").open("w",newline="",encoding="utf-8-sig") as f:
        w=csv.DictWriter(f,fieldnames=list(hot[0]));w.writeheader();w.writerows(sorted(hot,key=lambda x:x["maxM"],reverse=True))
    vals={r["name"]:r for r in rows}
    lines=["# XYC-B4-B-SSE-QUALITY-ANALYSIS", "", "## Scope and evidence identity", "",
      "Offline analysis only; no renderer, Vulkan, or RTX 3060 work was started. Input batch: `20261015-202400`; terrain: `n24e121.hgt`; source SHA-256: `BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1`.",
      "", "The task identifies B3 HEAD `10af215…`, but captured manifests identify `HEAD=9429be…; B3=VIS; DIRTY=YES`. Results below are conditional on the supplied B3 artifacts; attribution to the claimed HEAD is **UNCONFIRMED** until Parent reconciles this provenance.",
      "", "All image-level conclusions have `Coverage = UNCONFIRMED`; blue areas are not presumed valid.", "", "## Configuration comparison", "",
      "GPU numbers are medians across five independent process-level P50/P95 summaries. Error P95/Max is pooled over 3,113,501 visible-footprint source samples in the near-oblique pose; it is Source→final selected mesh, not a Render→LOD decomposition.",
      "", "| Configuration | GPU P50 ms | GPU P95 ms | Triangle count | GPU P50 saved vs Auto | Source→final P95 m | Delta vs Auto | Source→final Max m | Near-oblique PNG |", "|---|---:|---:|---:|---:|---:|---:|---:|---|"]
    for r in rows:
        lines.append(f"| {r['name']} | {r['gpuP50Ms']:.3f} | {r['gpuP95Ms']:.3f} | {r['triangles']:,.0f} | {r['gpuP50SavingsPct']:.1f}% | {r['sourceToFinalP95M']:.2f} | {r['sourceToFinalP95DeltaPct']:+.1f}% | {r['sourceToFinalMaxM']:.2f} | RGB MAE {r['screenRgbMae']}; XOR {r['screenMaskXorPct']}% |")
    lines += ["", "## Error layers and quantile availability", "", "- Source→Render (HGT→2049 render heightfield): P95 = 10.3703 m. Source sample array and sample count are absent from this package, so P50, P99, and Max cannot be computed.",
      "- Render→LOD: P50/P95/P99/Max cannot be computed. The normalized input stores this layer as null; Source→final mesh summaries cannot be algebraically split into the two layers.",
      "- Source→final selected mesh: P95 and Max are available per configuration from pooled visible-footprint source samples (n=3,113,501). P50/P99 are absent; no percentile interpolation is attempted.",
      "- Chunk-record distributions in `error-statistics.json` are quantiles of 21 per-chunk P95/Max records, not source-sample error quantiles.", "",
      "## Visible chunk hotspots", "", "For SSE 4 px, the largest per-chunk P95 is chunk (6,8), selected LOD 2, 32.43 m; its max is 93.85 m. The largest visible max remains chunk (6,4), LOD 0, 134.80 m (chunk P95 12.31 m). These identify terrain grid regions only: x/y cell intervals are in `visible-chunk-hotspots.csv`; geographic lon/lat mapping is unverified.",
      "", "Per-chunk summary distributions (21 visible chunk records; these are not pooled source-sample percentiles):", "",
      "| Configuration | Chunk P95 records P50/P95/P99/Max m | Chunk Max records P50/P95/P99/Max m |", "|---|---:|---:|"]
    for label,s in stats_out.items():
        q=s["chunkP95RecordDistributionM"]; x=s["chunkMaxRecordDistributionM"]
        fmt=lambda d:f"{d['p50']:.2f}/{d['p95']:.2f}/{d['p99']:.2f}/{d['max']:.2f}"
        lines.append(f"| {label} | {fmt(q)} | {fmt(x)} |")
    lines += [
      "", "## Screen pixels versus geometry", "", "For the representative near-oblique PNG, Auto/SSE 0.5/1/2/4 report RGB MAE=0 and mask XOR=0, while their selected chunk LODs and pooled geometric errors differ. In the fixed trajectory, SSE 4 differs only at sequence-B in the stored metrics: changed pixels 0.063705%, RGB MAE 0.019146, mask XOR 0.003052%; seven other captures are exactly equal. This is pixel agreement for these captures, not proof of full terrain coverage or perceived quality.",
      "", "## Performance–quality Pareto", "", "Using GPU P50 and visible-footprint Source→final P95, the non-dominated points are SSE 1 px (20.349 ms, 7.84 m), SSE 2 px (17.960 ms, 8.29 m), SSE 4 px (12.907 ms, 11.96 m), and LOD+1 (5.568 ms, 14.38 m). Auto is dominated by SSE 1 in these supplied summary metrics. LOD+1 is the fastest, with a larger error and measurable image differences; it is not an SSE tier.",
      "", "## Candidate tiers and next evidence", "", "- **SSE 1 px — conservative candidate:** about 9.8% lower GPU P50 than Auto with no representative near-oblique pixel difference and a slightly lower observed pooled P95. Validate on additional views and provenance before any default discussion.",
      "- **SSE 2 px — balanced candidate:** about 20.4% lower GPU P50; observed P95 is 2.6% above Auto, with only sequence-B pixel differences in the fixed trajectory. Suitable for a quality/performance trade-off study, not yet a product default.",
      "- **SSE 4 px — aggressive candidate:** about 42.8% lower GPU P50 and 42.8% fewer triangles; pooled P95 rises 48.0% while Max is unchanged in this view. The sequence-B difference is small but nonzero. Retain as an opt-in test tier pending coverage and multi-terrain evidence.",
      "- Next: reconcile run HEAD/dirty identity; retain paired raw per-sample Source→Render and Render→LOD residuals to compute P50/P95/P99/Max; verify geographic chunk mapping; rerun representative views/terrains with same-pose PNG metrics and validated terrain coverage. User visual acceptance remains separate.",
      "", "## Changed / Verified / Residual Risk", "", "**Changed:** added this offline analyzer, report, and machine-readable B4 outputs only. No product LOD, shared renderer, or product code changed.",
      "**Verified:** analyzer executed; three JSON outputs parsed; source error-table SHA-256 matches the raw B3 artifact; `git diff --check` returned no diagnostics. No tests or GPU/runtime commands were run.",
      "**Residual Risk:** B3 run identity does not match the task HEAD claim; quantile gaps, unconfirmed terrain coverage, and geographic chunk transform remain unresolved.",
      "", "## XYK CANDIDATE", "", "Candidate only; do not deposit directly: preserve pooled source-sample quantiles separately by error layer, and label per-chunk summary distributions so they cannot be mistaken for pooled terrain percentiles. Keep GPU image agreement distinct from geometric error and coverage evidence.",
      "", "## Files", "", "Machine-readable tables and Pareto data: `artifacts/diagnostics/xyc-b4/b-quality/`; source analyzer: `tools/xyc-offscreen/b4-evaluation/analyze_b4.py`. No product code or rendering pipeline was changed.", ""]
    REPORT.write_text("\n".join(lines),encoding="utf-8")
    print(f"Wrote {REPORT} and analysis outputs under {OUT}")

if __name__=="__main__": main()
