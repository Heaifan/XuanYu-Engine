#!/usr/bin/env python3
"""Compare fixed-pose baseline and SSE Vulkan capture bundles."""
import argparse
import csv
import hashlib
import json
from pathlib import Path

from PIL import Image, ImageChops, ImageEnhance, ImageStat

SKY = (89, 140, 204)


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest().upper()


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("baseline", type=Path)
    ap.add_argument("candidate", type=Path)
    args = ap.parse_args()
    left = json.loads((args.baseline / "captures.json").read_text())
    right = json.loads((args.candidate / "captures.json").read_text())
    a = {x["id"]: x for x in left["captures"]}; b = {x["id"]: x for x in right["captures"]}
    rows = []
    for name in a:
        apath = args.baseline / a[name]["image"]; bpath = args.candidate / b[name]["image"]
        if sha(apath) != a[name]["imageSha256"] or sha(bpath) != b[name]["imageSha256"]:
            raise ValueError(f"PNG hash mismatch: {name}")
        imga = Image.open(apath).convert("RGB"); imgb = Image.open(bpath).convert("RGB")
        if imga.size != imgb.size: raise ValueError(f"dimension mismatch: {name}")
        diff = ImageChops.difference(imga, imgb); stats = ImageStat.Stat(diff)
        pa, pb = list(imga.getdata()), list(imgb.getdata()); changed = mask = 0
        for x, y in zip(pa, pb):
            changed += max(abs(x[i]-y[i]) for i in range(3)) > 8
            mask += (x == SKY) != (y == SKY)
        diff_path = args.candidate / f"diff-{name}.png"
        ImageEnhance.Contrast(diff).enhance(3).save(diff_path)
        count = imga.width * imga.height
        rows.append({"capture": name, "rgbMae": sum(stats.mean)/3,
            "changedPixelPct": changed*100/count, "maskXorPct": mask*100/count,
            "diffPng": diff_path.name})
    with (args.candidate / "trajectory-differences.csv").open("w", newline="", encoding="utf-8-sig") as f:
        writer = csv.DictWriter(f, fieldnames=rows[0].keys()); writer.writeheader(); writer.writerows(rows)
    (args.candidate / "trajectory-differences.json").write_text(json.dumps(rows, indent=2))
    print(json.dumps({"captureCount": len(rows), "results": rows}, indent=2))


if __name__ == "__main__":
    main()
