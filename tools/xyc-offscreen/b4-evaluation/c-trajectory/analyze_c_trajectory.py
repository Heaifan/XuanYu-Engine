#!/usr/bin/env python3
"""Analyze per-frame B4 continuous camera trajectories and matched LOD captures."""
import hashlib
import json
import sys
import math
import re
import struct
from functools import lru_cache
from collections import Counter
from pathlib import Path
from statistics import mean

from PIL import Image, ImageChops
import numpy as np

SKY = (89, 140, 204)


def load(root, variant):
    path = root / variant / "frames.json"
    doc = json.loads(path.read_text(encoding="utf-8"))
    if doc.get("schema") != "XYC-B4-C-TRAJECTORY/1":
        raise ValueError(f"invalid evidence schema: {path}")
    return {f["id"]: f for f in doc["frames"]}, doc


def image(path, expected):
    if hashlib.sha256(path.read_bytes()).hexdigest().upper() != expected.upper():
        raise ValueError(f"PNG hash mismatch: {path}")
    return Image.open(path).convert("RGB")


@lru_cache(maxsize=2)
def load_source(path_text):
    path = Path(path_text)
    match = re.fullmatch(r"[ns](\d{2})[ew]\d{3}", path.stem, re.IGNORECASE)
    if not match:
        return None
    south = int(match.group(1))
    side = math.isqrt(path.stat().st_size // 2)
    if side * side * 2 != path.stat().st_size:
        return None
    x_span = 111320 * math.cos(math.radians(south + 0.5))
    return path.read_bytes(), side, x_span


def source_height(path, x, y):
    source = load_source(str(path))
    if source is None:
        return None
    data, side, x_span = source
    gx = x / x_span * (side - 1)
    gy = (side - 1) - y / 111320 * (side - 1)
    c, r = min(side - 2, int(gx)), min(side - 2, int(gy))
    fx, fy = gx - c, gy - r
    sample = lambda row, col: struct.unpack_from(">h", data, 2 * (row * side + col))[0]
    values = [sample(r, c), sample(r, c + 1), sample(r + 1, c), sample(r + 1, c + 1)]
    if -32768 in values:
        return None
    return values[0] * (1 - fx) * (1 - fy) + values[1] * fx * (1 - fy) + values[2] * (1 - fx) * fy + values[3] * fx * fy


def compare(left, right, output):
    if left.size != right.size:
        raise ValueError("image dimensions differ")
    a = np.asarray(left, dtype=np.int16)
    b = np.asarray(right, dtype=np.int16)
    delta = np.abs(a - b)
    changed = int(np.count_nonzero(delta.max(axis=2) > 8))
    mask_a = np.all(a == SKY, axis=2)
    mask_b = np.all(b == SKY, axis=2)
    mask = int(np.count_nonzero(mask_a != mask_b))
    diff = Image.fromarray(delta.astype(np.uint8))
    diff.save(output)
    total = left.width * left.height
    return {"rgbMae": float(delta.mean()),
            "changedPixelPct": changed * 100 / total, "coverageMaskXorPct": mask * 100 / total}


def main():
    root = Path(sys.argv[1]).resolve()
    source = Path(sys.argv[sys.argv.index("--source") + 1]).resolve() if "--source" in sys.argv else None
    baseline, base_doc = load(root, "baseline")
    candidate, cand_doc = load(root, "sse-4")
    if baseline.keys() != candidate.keys():
        raise ValueError("variant frame identifiers differ")
    if base_doc["sourceSha256"] != cand_doc["sourceSha256"]:
        raise ValueError("variant source hashes differ")
    rows, temporal, report = [], [], ["# C-TRAJECTORY-REPORT", "", f"Run: `{root}`", "",
                        "Runtime is offscreen Vulkan readback without window, swapchain, or Present. Status does not represent user visual acceptance.", ""]
    tracks = sorted({f["track"] for f in baseline.values()})
    overall = "PASS"
    report += [f"Device: {base_doc['device']}; Auto LOD identity: `{base_doc['head']}` dirty={base_doc['dirty']}; "
               f"SSE 4 px identity: `{cand_doc['head']}` dirty={cand_doc['dirty']}.", ""]
    for track in tracks:
        ids = [key for key, frame in baseline.items() if frame["track"] == track]
        issues, max_jump, lod_changes, sse_lod_changes = [], 0.0, 0, 0
        max_mask_xor, max_pair_mae, triangle_delta = 0.0, 0.0, []
        auto_lods, sse_lods = Counter(), Counter()
        previous_images, first_images, prior_ids = {}, {}, {}
        max_temporal_mask = 0.0
        peak_transition = None
        for key in ids:
            a, b = baseline[key], candidate[key]
            if a["camera"] != b["camera"] or a["observationCenter"] != b["observationCenter"]:
                issues.append("camera mismatch")
            for frame in (a, b):
                if frame["coveragePixels"] <= 0 or frame["visibleChunks"] <= 0 or frame["terrainTriangles"] <= 0:
                    issues.append(f"empty geometry/coverage at {key}")
                if frame["gpuTimestampMs"] is None:
                    issues.append(f"GPU timestamp unavailable at {key}")
                if len(frame["chunks"]) < frame["visibleChunks"] + frame["culledChunks"]:
                    issues.append(f"incomplete chunk/LOD trace at {key}")
            ia = image(root / "baseline" / a["png"], a["pngSha256"])
            ib = image(root / "sse-4" / b["png"], b["pngSha256"])
            row = compare(ia, ib, root / "sse-4" / f"diff-{key}.png")
            row.update({"id": key, "autoTriangles": a["terrainTriangles"],
                        "sse4Triangles": b["terrainTriangles"], "autoCoverage": a["coveragePixels"],
                        "sse4Coverage": b["coveragePixels"], "autoGpuMs": a["gpuTimestampMs"],
                        "sse4GpuMs": b["gpuTimestampMs"]})
            rows.append(row)
            if source is not None:
                camera = a["camera"]["position"]
                height = source_height(source, camera["x"], camera["y"])
                row["sourceHgtBilinearHeightMeters"] = height
                row["cameraHeightAboveSourceHgtMeters"] = None if height is None else camera["z"] - height
            max_mask_xor = max(max_mask_xor, row["coverageMaskXorPct"])
            max_pair_mae = max(max_pair_mae, row["rgbMae"])
            triangle_delta.append((a["terrainTriangles"] - b["terrainTriangles"]) * 100 /
                                  max(1, a["terrainTriangles"]))
            auto_lods.update(c["lod"] for c in a["chunks"] if c["lod"] >= 0)
            sse_lods.update(c["lod"] for c in b["chunks"] if c["lod"] >= 0)
            for variant, current in (("baseline", ia), ("sse-4", ib)):
                if variant in previous_images:
                    motion = compare(previous_images[variant], current,
                        root / variant / f"motion-diff-{key}.png")
                    max_temporal_mask = max(max_temporal_mask, motion["coverageMaskXorPct"])
                    temporal.append({"track": track, "variant": variant,
                        "from": prior_ids[variant], "to": key, **motion})
                elif track == "return-A-B-A":
                    first_images[variant] = current.copy()
                previous_images[variant] = current
                prior_ids[variant] = key
        for left_id, right_id in zip(ids, ids[1:]):
            left, right = baseline[left_id], baseline[right_id]
            denom = max(1, left["coveragePixels"])
            jump = abs(right["coveragePixels"] - left["coveragePixels"]) * 100 / denom
            max_jump = max(max_jump, jump)
            if jump >= max_jump:
                peak_transition = {"from": left_id, "to": right_id, "autoCoverage": [left["coveragePixels"], right["coveragePixels"]],
                    "sse4Coverage": [candidate[left_id]["coveragePixels"], candidate[right_id]["coveragePixels"]],
                    "cameraFrom": left["camera"]["position"], "cameraTo": right["camera"]["position"]}
            before = {(c["x"], c["y"]): c["lod"] for c in left["chunks"] if c["lod"] >= 0}
            after = {(c["x"], c["y"]): c["lod"] for c in right["chunks"] if c["lod"] >= 0}
            lod_changes += sum(before[k] != after[k] for k in before.keys() & after.keys())
            left_sse = {(c["x"], c["y"]): c["lod"] for c in candidate[left_id]["chunks"] if c["lod"] >= 0}
            right_sse = {(c["x"], c["y"]): c["lod"] for c in candidate[right_id]["chunks"] if c["lod"] >= 0}
            sse_lod_changes += sum(left_sse[k] != right_sse[k] for k in left_sse.keys() & right_sse.keys())
        status = "FAIL" if any(x.startswith("empty geometry") for x in issues) else "PASS"
        if issues and status != "FAIL":
            status = "INCONCLUSIVE"
        if max_jump > 35 and status == "PASS":
            status = "INCONCLUSIVE"
            issues.append("adjacent coverage changed over 35%; inspect PNG sequence")
        if status == "FAIL":
            overall = "FAIL"
        elif status == "INCONCLUSIVE" and overall == "PASS":
            overall = "INCONCLUSIVE"
        track_header = len(report)
        report += [f"## {track}: {status}", "",
                   f"Frames: {len(ids)}; max adjacent coverage change: {max_jump:.4f}%; "
                   f"visible chunks Auto/SSE4: {min(baseline[k]['visibleChunks'] for k in ids)}–{max(baseline[k]['visibleChunks'] for k in ids)}/"
                   f"{min(candidate[k]['visibleChunks'] for k in ids)}–{max(candidate[k]['visibleChunks'] for k in ids)}; "
                   f"LOD histogram Auto/SSE4: {dict(auto_lods)}/{dict(sse_lods)}; "
                   f"chunk LOD transitions Auto/SSE4: {lod_changes}/{sse_lod_changes}; "
                   f"SSE4 triangle reduction min/mean/max: {min(triangle_delta):.2f}/{mean(triangle_delta):.2f}/{max(triangle_delta):.2f}%; "
                   f"max paired coverage-mask XOR: {max_mask_xor:.5f}%; "
                   f"max paired RGB MAE: {max_pair_mae:.5f}; max adjacent temporal mask XOR: {max_temporal_mask:.5f}%.",
                   "Issues: " + ("; ".join(sorted(set(issues))) if issues else "none"), ""]
        if max_jump > 35 and peak_transition:
            report += [f"Peak coverage transition: `{peak_transition['from']}` → `{peak_transition['to']}`; "
                       f"Auto {peak_transition['autoCoverage']}; SSE4 {peak_transition['sse4Coverage']}; "
                       f"camera {peak_transition['cameraFrom']} → {peak_transition['cameraTo']}."]
            if source is not None:
                for key in (peak_transition["from"], peak_transition["to"]):
                    f = baseline[key]; p = f["camera"]["position"]
                    h = source_height(source, p["x"], p["y"])
                    report.append(f"{key} source-HGT bilinear height: {h}; camera Z: {p['z']}; "
                                  f"camera above source estimate: {None if h is None else p['z'] - h} m.")
            report.append("")
        if track == "return-A-B-A":
            for variant, docs in (("baseline", baseline), ("sse-4", candidate)):
                first, last = docs[ids[0]], docs[ids[-1]]
                returned = previous_images[variant]
                start = first_images[variant]
                exact = first["camera"] | {"revision": 0} == last["camera"] | {"revision": 0}
                exact = exact and first["observationCenter"] == last["observationCenter"]
                same_pixels = ImageChops.difference(start, returned).getbbox() is None
                report += [f"{variant} A→B→A camera exact (excluding revision): {exact}; PNG exact: {same_pixels}."]
                if not exact or not same_pixels:
                    overall = "FAIL"
                    report[track_header] = f"## {track}: FAIL"
            report.append("")
    report += ["## Interpretation and coverage gaps", "", "A high temporal coverage change shared by both LOD variants is not classified as an LOD defect. Source-HGT bilinear height estimates, where listed, are not the resampled render mesh or the final visible surface; confirm against a production camera/runtime path before assigning cause.", "", "`n23e121.hgt` exists locally, but this runner is pinned to B3 `n24e121` coordinates and hash; the existing B3 harness imports a single tile. Stitched terrain is not supported by that baseline harness.", ""]
    (root / "frame-comparisons.json").write_text(json.dumps(rows, indent=2), encoding="utf-8")
    (root / "temporal-comparisons.json").write_text(json.dumps(temporal, indent=2), encoding="utf-8")
    (root / "C-TRAJECTORY-REPORT.md").write_text("\n".join(report), encoding="utf-8")
    print(json.dumps({"status": overall, "tracks": tracks, "report": str(root / "C-TRAJECTORY-REPORT.md")}, indent=2))
    return 1 if overall == "FAIL" else 0


if __name__ == "__main__":
    raise SystemExit(main())
