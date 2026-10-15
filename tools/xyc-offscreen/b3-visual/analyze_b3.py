#!/usr/bin/env python3
"""Validate externally captured XYC-B3 evidence and emit deterministic PNG diffs."""
import argparse
import hashlib
import json
import math
from pathlib import Path

from PIL import Image, ImageChops, ImageStat


def close_vec(actual, expected, tol=1e-5):
    return all(math.isclose(float(actual[k]), float(expected[k]), rel_tol=0, abs_tol=tol)
               for k in ("x", "y", "z"))


def sha256(path):
    h = hashlib.sha256()
    with path.open("rb") as f:
        for block in iter(lambda: f.read(1024 * 1024), b""):
            h.update(block)
    return h.hexdigest().upper()


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("bundle", type=Path, help="Parent-produced capture bundle")
    ap.add_argument("--trajectory", type=Path, default=Path(__file__).with_name("trajectory.json"))
    args = ap.parse_args()
    bundle = args.bundle.resolve()
    trajectory = json.loads(args.trajectory.read_text(encoding="utf-8"))
    evidence_path = bundle / "captures.json"
    evidence = json.loads(evidence_path.read_text(encoding="utf-8"))
    if evidence.get("schema") != "XYC-B3-CAPTURES/1":
        raise ValueError("captures.json schema must be XYC-B3-CAPTURES/1")
    if evidence.get("terrainSha256", "").upper() != trajectory["terrain"]["sha256"]:
        raise ValueError("terrain SHA-256 does not match the trajectory")
    captures = {item["id"]: item for item in evidence.get("captures", [])}
    required = [s["capture"] for s in trajectory["steps"]]
    missing = [name for name in required if name not in captures]
    if missing:
        raise ValueError("missing captures: " + ", ".join(missing))
    report = {"schema": "XYC-B3-ANALYSIS/1", "terrainSha256": evidence["terrainSha256"],
              "captureCount": len(required), "poseChecks": [], "sequenceChecks": [], "imageDiffs": []}
    dimensions = None
    for step in trajectory["steps"]:
        capture = captures[step["capture"]]
        expected = trajectory["poses"][step["pose"]]
        camera = capture["camera"]
        ok = close_vec(camera["position"], expected["position"]) and close_vec(camera["forward"], expected["forward"])
        ok = ok and close_vec(camera["up"], expected["up"])
        for key in ("fovDegrees", "nearPlane", "farPlane"):
            ok = ok and math.isclose(float(camera[key]), float(expected[key]), rel_tol=0, abs_tol=1e-8)
        image_path = (bundle / capture["image"]).resolve()
        if bundle not in image_path.parents:
            raise ValueError(f"capture image escapes bundle: {capture['image']}")
        if sha256(image_path) != capture["imageSha256"].upper():
            raise ValueError(f"image SHA-256 mismatch: {step['capture']}")
        with Image.open(image_path) as image:
            size = image.size
            image.load()
        if dimensions is None:
            dimensions = size
        if size != dimensions:
            raise ValueError(f"capture dimensions differ: {step['capture']} {size} != {dimensions}")
        report["poseChecks"].append({"capture": step["capture"], "pass": bool(ok), "pose": step["pose"], "imageSize": size})
        if not ok:
            raise ValueError(f"captured camera does not match fixed pose: {step['capture']}")
    for sequence in trajectory["sequenceChecks"]:
        a, b, returned = [captures[x] for x in sequence["captures"]]
        camera_a, camera_return = a["camera"], returned["camera"]
        same = all(close_vec(camera_a[key], camera_return[key]) for key in ("position", "forward", "up"))
        same = same and all(math.isclose(float(camera_a[key]), float(camera_return[key]), rel_tol=0, abs_tol=1e-8)
                            for key in ("fovDegrees", "nearPlane", "farPlane"))
        report["sequenceChecks"].append({"name": sequence["name"], "cameraReturnExact": same})
        if not same:
            raise ValueError(f"A-to-B-to-A camera pose did not return exactly: {sequence['name']}")
        ia = Image.open((bundle / a["image"]).resolve()).convert("RGB")
        ib = Image.open((bundle / returned["image"]).resolve()).convert("RGB")
        diff_path = bundle / "diff-sequence-A-before-vs-A-return.png"
        ImageChops.difference(ia, ib).save(diff_path)
        report["imageDiffs"].append({"left": sequence["captures"][0], "right": sequence["captures"][2],
            "path": diff_path.name, "meanAbsoluteError": sum(ImageStat.Stat(ImageChops.difference(ia, ib)).mean) / 3})
    (bundle / "analysis.json").write_text(json.dumps(report, indent=2), encoding="utf-8")
    print(json.dumps({"status": "PASS", "captureCount": len(required), "analysis": str(bundle / "analysis.json")}, indent=2))


if __name__ == "__main__":
    main()
