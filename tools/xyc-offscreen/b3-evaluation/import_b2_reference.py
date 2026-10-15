#!/usr/bin/env python3
"""Convert an existing ignored B2 run directory into the B3 normalized input schema."""
import argparse
import csv
import hashlib
import json
from pathlib import Path


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("b2_root", type=Path, help="B2 run directory containing root run-manifest.json")
    ap.add_argument("output", type=Path, help="New normalized JSON path")
    args = ap.parse_args()
    root = args.b2_root.resolve()
    manifest_path = root / "run-manifest.json"
    if not manifest_path.is_file():
        raise SystemExit(f"Missing B2 manifest: {manifest_path}")
    meta = json.loads(manifest_path.read_text(encoding="utf-8-sig"))
    order_path = root / "run-order.csv"
    order_map = {}
    if order_path.is_file():
        with order_path.open(encoding="utf-8-sig", newline="") as f:
            for row in csv.DictReader(f):
                order_map[(row.get("config"), int(row.get("replicate", 0)))] = int(row["order"])
    configs = []
    for aggregate in meta.get("configurations", []):
        name = aggregate["config"]
        runs = []
        for manifest in sorted(root.glob(f"{name}/replicate-*/run-1/run-manifest.json")):
            run = json.loads(manifest.read_text(encoding="utf-8-sig"))
            c = run["Configurations"][0]
            replicate = run.get("Replicate")
            runs.append({"replicate": replicate, "order": order_map.get((name, replicate)),
                         "gpuFrameP50Ms": c.get("GpuMsP50"), "gpuFrameP95Ms": c.get("GpuMsP95"),
                         "validFrames": c.get("ValidGpuSamples"), "triangles": c.get("Triangles"),
                         "visibleChunks": c.get("VisibleChunks"), "drawCalls": c.get("DrawCalls"),
                         "estimatedGpuBytes": c.get("EstimatedGpuBytes"),
                         "pngSha256": c.get("PngSha256"), "manifestSha256": sha(manifest)})
        q = next((x for x in meta.get("quality", []) if x.get("config") == name), {})
        configs.append({"name": name, "family": "B2_REFERENCE", "status": "REFERENCE_ONLY",
                        "evidenceClass": "MEASURED_IN_B2_NOT_B3", "processRuns": runs,
                        "meshError": {"sourceToHeightfieldP95M": aggregate.get("height_error_world_p95_m"),
                                      "heightfieldToFinalMeshP95M": None, "finalMeshWorldP95M": None,
                                      "sourceToHeightfieldMeaning": "B2 sampled HGT-to-heightfield reconstruction field; not final mesh error"},
                        "screenError": {"p95Px": None, "validity": "NOT PROVIDED BY B2"},
                        "pixels": {"maskXorPct": q.get("maskXorPct"), "rgbMae": q.get("rgbMaeCommonTerrain"),
                                   "edgeMae": q.get("edgeMaeCommonTerrain")},
                        "sourceArtifact": str(manifest_path)})
    for threshold in (0.5, 1, 2, 4):
        configs.append({"name": f"SSE-{threshold:g}px", "family": "SSE", "thresholdPx": threshold,
                        "status": "NOT RUN", "evidenceClass": "NOT RUN", "processRuns": [],
                        "meshError": {"sourceToHeightfieldP95M": None, "heightfieldToFinalMeshP95M": None,
                                      "finalMeshWorldP95M": None}, "screenError": {"p95Px": None},
                        "pixels": {}, "sourceArtifact": None})
    doc = {"schemaVersion": "xyc-b3-evaluation/v1", "dataset": {"sourceSha256": meta.get("sourceSha256"),
            "device": meta.get("device"), "driverVersion": meta.get("driverVersion"),
            "buildIdentity": meta.get("buildIdentity"), "sourceSetFingerprintSha256": meta.get("sourceSetFingerprintSha256")},
           "b2Artifact": {"root": str(root), "rootManifest": str(manifest_path), "rootManifestSha256": sha(manifest_path)},
           "pairing": {"baseline": "2049-auto", "note": "pairing indices must be validated against run-order.csv; B2 and SSE families are not paired"},
           "configurations": configs}
    if args.output.exists():
        raise SystemExit(f"Refusing to overwrite existing input: {args.output}")
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(doc, indent=2), encoding="utf-8")


if __name__ == "__main__":
    main()
