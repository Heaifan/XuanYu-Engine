import argparse
import csv
import json
import re
from collections import Counter
from pathlib import Path

from PIL import Image, ImageDraw


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--png", type=Path, required=True)
    ap.add_argument("--terrain-log", type=Path, required=True)
    ap.add_argument("--captures-json", type=Path, required=True)
    ap.add_argument("--hgt", type=Path, required=True)
    ap.add_argument("--out", type=Path, required=True)
    ap.add_argument("--camera-revision", type=int, default=3)
    args = ap.parse_args()
    args.out.mkdir(parents=True, exist_ok=False)
    im = Image.open(args.png).convert("RGB")
    clear = (89, 140, 204)
    px = list(im.get_flattened_data())
    counts = Counter(px)
    clear_n = counts[clear]
    terrain_n = len(px) - clear_n
    coverage = Image.new("L", im.size)
    coverage.putdata([255 if p != clear else 0 for p in px])
    depth = Image.new("L", im.size)
    depth.putdata([0 if p == clear else 255 for p in px])
    coverage.save(args.out / "terrain-coverage-mask.png")
    depth.save(args.out / "depth-occupancy-proxy.png")

    rows = []
    for line in args.terrain_log.read_text(encoding="utf-8").splitlines():
        if "[TERRAIN-CHUNK] " not in line:
            continue
        rev = re.search(r"CameraRevision=(\d+)", line)
        chunk = re.search(r"Chunk=(\d+,\d+);", line)
        result = re.search(r"Result=([^;]+)", line)
        lod = re.search(r";LOD=([^;]+);", line)
        bounds = re.search(r"ProjectedBounds=([^;]+)", line)
        if not (rev and chunk and result and int(rev.group(1)) == args.camera_revision):
            continue
        rows.append((chunk.group(1), result.group(1), lod.group(1) if lod else "?",
                     bounds.group(1) if bounds else "?"))
    with (args.out / "chunk-visibility.csv").open("w", newline="", encoding="utf-8") as f:
        w = csv.writer(f)
        w.writerow(["chunk_xy", "result", "lod", "projected_bounds_probe_space"])
        w.writerows(rows)
    grid = Image.new("RGB", (9 * 36, 9 * 36), (35, 35, 35))
    draw = ImageDraw.Draw(grid)
    for chunk, result, _, _ in rows:
        x, y = map(int, chunk.split(","))
        color = (45, 180, 90) if result == "VISIBLE" else (190, 65, 65)
        draw.rectangle((x * 36, y * 36, x * 36 + 34, y * 36 + 34), fill=color)
        draw.text((x * 36 + 5, y * 36 + 9), chunk, fill="white")
    grid.save(args.out / "chunk-visibility-grid.png")
    captures = json.loads(args.captures_json.read_text(encoding="utf-8"))
    camera = next(c["camera"] for c in captures["captures"] if c["id"] == "near-oblique")
    snap = re.search(r"\[TERRAIN-SNAPSHOT\].*?Size=(\d+)x(\d+);CellSizeXY=([0-9.]+),([0-9.]+)",
                     args.terrain_log.read_text(encoding="utf-8"))
    if not snap:
        raise ValueError("Terrain snapshot dimensions/cell size were not found in probe log")
    fw, fh, csx, csy = int(snap[1]), int(snap[2]), float(snap[3]), float(snap[4])
    raw = args.hgt.read_bytes()
    side = int((len(raw) // 2) ** 0.5)
    if side * side * 2 != len(raw):
        raise ValueError("HGT is not a square int16 raster")
    rx, ry = camera["position"]["x"] / csx, camera["position"]["y"] / csy
    ix, iy = int(rx), int(ry)
    fx, fy = rx - ix, ry - iy
    def sample(x, y):
        col = round(x * (side - 1) / (fw - 1))
        row = side - 1 - round(y * (side - 1) / (fh - 1))
        return int.from_bytes(raw[2 * (row * side + col):2 * (row * side + col) + 2], "big", signed=True)
    h00, h01, h10, h11 = sample(ix, iy), sample(ix + 1, iy), sample(ix, iy + 1), sample(ix + 1, iy + 1)
    if fx + fy <= 1:
        surface = h00 * (1 - fx - fy) + h01 * fx + h10 * fy
    else:
        surface = h01 * (1 - fy) + h10 * (1 - fx) + h11 * (fx + fy - 1)
    camera_ground = {"method": "2049-grid source-index mapping + LOD0 mesh triangle interpolation",
                     "terrain_grid": [fw, fh], "grid_xy": [rx, ry],
                     "source_hgt_quad_m": [[h00, h01], [h10, h11]],
                     "triangle_surface_z_m": surface, "camera_z_m": camera["position"]["z"],
                     "camera_below_surface_m": surface - camera["position"]["z"],
                     "classification": "INFERENCE; camera pose/sample interpolation, not GPU depth"}
    (args.out / "camera-ground-estimate.json").write_text(json.dumps(camera_ground, indent=2), encoding="utf-8")
    summary = {"source_png": str(args.png), "size": list(im.size), "clear_rgb": list(clear),
               "clear_pixels": clear_n, "terrain_pixels_by_non_clear_complement": terrain_n,
               "clear_percent": 100 * clear_n / len(px), "terrain_percent": 100 * terrain_n / len(px),
               "depth_proxy": "clear RGB => cleared depth 0; non-clear terrain RGB => terrain wrote depth > 0 (binary only)",
               "camera_revision": args.camera_revision,
               "chunk_counts": dict(Counter(r[1] for r in rows)), "chunks": len(rows),
               "camera_ground_estimate": camera_ground}
    (args.out / "coverage-summary.json").write_text(json.dumps(summary, indent=2), encoding="utf-8")
    print(json.dumps(summary, indent=2))


if __name__ == "__main__":
    main()
