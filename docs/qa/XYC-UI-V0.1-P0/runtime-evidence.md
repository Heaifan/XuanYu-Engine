# XYC-UI-V0.1-P0 Runtime Evidence

Date: 2026-10-18 (Asia/Shanghai)

## Production Editor session

- Evidence directory: [`evidence/36e279fdeda64487869f1c56d225167f/`](evidence/36e279fdeda64487869f1c56d225167f/), containing byte-for-byte copies of the four PNGs, their original JSON sidecars, and the original runtime manifest. The diagnostic originals remain under `artifacts/diagnostics/xyc-ui-v0.1-p0/`.
- Status: `RUNTIME_CAPTURED`; PID `37916`; owned HWND `0x1F216E`; visible `true`; foreground observed `false` for all four captures.
- Session closed successfully; CLI exited with code `0`. Window size baseline was 1360×820 DIP; DPI 96 / scale 1.0. Widths 1360 and 1920 DIP were applied with the same 820 DIP height and returned as client dimensions.
- Build identity for all four images: `Editor.App.exe=EFDAE1E21A58D089BC6FD5F8F554F3A12D0F6B86A03A74810F63F4F56809968A;XuanYu.Editor.UI.dll=618A2AB8B9AEFFCDE2D90CEC5A518D0D7C1C5A902765626D896331AFCF0B537C`.
- Capture method: actual DWM desktop composition via `GDI.BitBlt`, crop derived from production TOP bounds. The target HWND was temporarily raised without activation and restored after capture. These are real runtime pixels and are not synthetic or offscreen-rendered.

| Capture | Actual theme | PNG dimensions | SHA-256 | Sidecar |
|---|---|---:|---|---|
| [Light-1360.png](evidence/36e279fdeda64487869f1c56d225167f/Light-1360.png) | Light | 1332×70 | `2687E528079F7ABBB01CA1FC660B4E55FB7D6923B18B8727306BE7598531ACBF` | `Light-1360.json` |
| [Dark-1360.png](evidence/36e279fdeda64487869f1c56d225167f/Dark-1360.png) | Dark | 1332×70 | `A2DAE8C9F2ED8390BDC112CE5C3A6983932FFD95E813420B751A7C659570140D` | `Dark-1360.json` |
| [Light-1920.png](evidence/36e279fdeda64487869f1c56d225167f/Light-1920.png) | Light | 1892×70 | `9BF0DE6BDD7269ECB35F83D0233C06FC3C9F50A8FCD87327A5F46AB929022FDF` | `Light-1920.json` |
| [Dark-1920.png](evidence/36e279fdeda64487869f1c56d225167f/Dark-1920.png) | Dark | 1892×70 | `A372A1B4FED0BB9C72A6712AC1E5733A5BB9700042B3ACE26299A9053463792E` | `Dark-1920.json` |

Manifest verifies both theme state and sampled pixel differences: Light TOP background sample `237,240,242`; Dark `56,62,67`. At 96 DPI, physical pixels equal DIP. The client width includes 28 DIP of window chrome; production TOP bounds are x=14 DIP, y=0, height=70 DIP. Actual TOP PNG width therefore equals client width minus the side margins.

## Silent Host regression

`artifacts/diagnostics/xyc-ui-v0.1-p0/silent-runtime-evidence.json` records PASS: `silent-hidden-hwnd`, `Visible=false`, no `WS_VISIBLE`, foreground not taken, client 776×650, Vulkan surface/swapchain attached, first Present succeeded, `session.close` accepted, child process exited code 0, HWND destroyed and no residual Editor PID. Readback was not requested by this smoke run; this confirms the core hidden-HWND Present lifecycle, not a new readback test.

## Scoped verification

- Editor App Debug build: PASS, 0 warnings / 0 errors.
- Editor Control CLI Debug build: PASS, 0 warnings / 0 errors (App built first so the copied UI assembly is current).
- XYC-filtered World tests: PASS, 25/25, 0 skipped.
- `git diff --check`: PASS (rerun at closeout).

Older failed capture runs remain in the same diagnostic parent directory with their failure manifests. They are retained as diagnostic history and are not counted as successful evidence.
