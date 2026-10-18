# Capture comparison — XYC-UI-V0.1-P1-B-R1

## Approved per-case thresholds

The user approved these thresholds before final runtime validation. Every profile must independently pass all measures: exact pixel ratio ≥94.5%; RGB MAE ≤0.50; PSNR ≥35 dB; pixels with max-channel delta >16 ≤0.9%; equal dimensions; opaque alpha; correct Light/Dark theme. The fixed P1-A GDI images and comparison method were retained.

## Candidate final runtime

Reference images: `docs/qa/XYC-UI-V0.1-P1-A/evidence/candidate-final/<case>/<case>.png`
Candidate runtime: `evidence/candidate-runtime-final9/<case>/<case>.png`
Candidate binary identity: App SHA-256 `DE13C9CF9C40C876B9F62C49D864F0FD665E77FE73EA5CCC1EE8896E0538B74C`; UI SHA-256 `7C685D237CC9C1E0EF385444B7E6028EC3286D22628E1A5BEDCFDEBCF1C8EE94`.

| Profile | Dimensions | Exact | RGB MAE | PSNR | >16 delta | Theme | Result |
|---|---:|---:|---:|---:|---:|---|---|
| Light-1360 | 1330×98 | 95.2348% | 0.1040 | 48.2278 dB | 0.0583% | Light | PASS |
| Light-1920 | 1890×98 | 96.6467% | 0.0732 | 49.7564 dB | 0.0410% | Light | PASS |
| Dark-1360 | 1330×98 | 94.7783% | 0.4197 | 35.3377 dB | 0.8171% | Dark | PASS |
| Dark-1920 | 1890×98 | 96.3255% | 0.2953 | 36.8639 dB | 0.5750% | Dark | PASS |

All four passed individually. Dark-1360 has the narrowest margin; its result remains above each approved threshold. The Windows desktop ran at 96 DPI, so DIP and physical pixel dimensions matched.

## WGC comparison

| Profile | Exact | RGB MAE | PSNR | >16 delta | Result |
|---|---:|---:|---:|---:|---|
| Light-1360 | 84.7246% | 9.3153 | 17.5275 dB | 11.5068% | FAIL |
| Light-1920 | 89.2506% | 6.5552 | 19.0537 dB | 8.0974% | FAIL |
| Dark-1360 | 84.7146% | 15.5748 | 14.2448 dB | 14.5036% | FAIL |
| Dark-1920 | 89.2436% | 10.9600 | 15.7709 dB | 10.2062% | FAIL |

WGC frame crops do not meet the threshold. The data proves that a real HWND frame can be read while nonforeground, not that its pixels are a valid replacement for the TOP evidence path.
