import hashlib
import json
import math
import sys
from pathlib import Path
from PIL import Image

LIMITS = {"exactPctMin": 94.5, "maeMax": 0.50, "psnrDbMin": 35.0, "over16PctMax": 0.9}
CASES = ("Light-1360", "Light-1920", "Dark-1360", "Dark-1920")


def compare(reference: Path, candidate: Path) -> dict:
    left = Image.open(reference).convert("RGBA")
    right = Image.open(candidate).convert("RGBA")
    if left.size != right.size:
        raise ValueError(f"dimension mismatch: {left.size} != {right.size}")
    exact = over16 = total = squared = 0
    opaque = True
    count = left.width * left.height
    for a, b in zip(left.get_flattened_data(), right.get_flattened_data()):
        delta = [abs(a[i] - b[i]) for i in range(3)]
        peak = max(delta)
        exact += peak == 0
        over16 += peak > 16
        total += sum(delta)
        squared += sum(value * value for value in delta)
        opaque &= b[3] == 255
    mae = total / (3 * count)
    psnr = 10 * math.log10(255 * 255 / (squared / (3 * count))) if squared else math.inf
    result = {"dimensions": list(left.size), "exactPct": 100 * exact / count,
              "mae": mae, "psnrDb": psnr, "over16Pct": 100 * over16 / count,
              "alphaOpaque": opaque}
    result["pass"] = (result["exactPct"] >= LIMITS["exactPctMin"] and
                      mae <= LIMITS["maeMax"] and psnr >= LIMITS["psnrDbMin"] and
                      result["over16Pct"] <= LIMITS["over16PctMax"] and opaque)
    return result


def main() -> None:
    reference_root, run_root, output = map(Path, sys.argv[1:4])
    runtime = json.loads((run_root / "runtime-evidence.json").read_text(encoding="utf-8-sig"))
    cases = {}
    for name in CASES:
        image = run_root / name / f"{name}.png"
        sidecar = json.loads(image.with_suffix(".json").read_text(encoding="utf-8-sig"))
        measured = compare(reference_root / name / f"{name}.png", image)
        cases[name] = {**measured, "sha256": hashlib.sha256(image.read_bytes()).hexdigest(),
                       "pid": sidecar["window"]["processId"], "hwnd": sidecar["window"]["hwnd"],
                       "theme": sidecar["theme"]["actual"], "dpiScale": sidecar["window"]["dpiScale"],
                       "buildIdentity": sidecar["buildIdentity"],
                       "foregroundUnchanged": sidecar["foregroundUnchanged"],
                       "zOrderUnchanged": sidecar["zOrderUnchanged"],
                       "backend": sidecar["captureBackend"]}
        cases[name]["pass"] &= cases[name]["theme"] == name.split("-")[0]
    result = {"status": "PASS" if all(item["pass"] for item in cases.values()) else "FAIL",
              "candidateIdentity": runtime["candidateIdentity"], "appSha256": runtime["appSha256"],
              "uiSha256": runtime["uiSha256"], "limits": LIMITS, "cases": cases}
    output.write_text(json.dumps(result, indent=2), encoding="utf-8")
    print(result["status"])


if __name__ == "__main__":
    main()
