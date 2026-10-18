# Capture feasibility — XYC-UI-V0.1-P1-B-R1

Baseline: `d2d011ac98314f172e0e3d729583e67832099f1f`
Runtime: Windows 11 Enterprise 26300, Avalonia 12.0.4, NVIDIA RTX 3060, primary display 1920×1080, DPI 96 / scale 1.0.

## A. Avalonia RenderTargetBitmap

- Real production `Window` and real `Top` Visual Tree rendered; the rendered Window was cropped to the actual TOP DIP bounds. No test window or fake component tree was used.
- Four Light/Dark × 1360/1920 profiles succeeded. The TOP region was 1330×98 or 1890×98 physical pixels, opaque alpha, with actual TOP content and background.
- The Window remained nonforeground. Runtime sidecars show `foregroundUnchanged=true`, `zOrderUnchanged=true`. The backend performs no `SetWindowPos`, desktop sampling, mouse or keyboard injection.
- A bright magenta native overlay was verified on screen (`GetPixel=0x00FF00FF`) over the TOP. The RTB capture hash remained identical to the unoccluded image, demonstrating capture of the underlying production Window content while obscured.
- Capture also succeeded while the Window state was `Minimized`; the API sidecar records `windowState=Minimized`. Restoring returned it to `Normal`.
- Avalonia rendering runs on the UI Dispatcher and is synchronous. A bounded `timeoutMs` token is checked before work and after `Render` before saving; timed-out output is not written. The runtime timeout path returns a structured error, but it cannot interrupt the renderer while its synchronous call is executing. Unit tests cover pre-cancel, cancellation propagation during backend work, and timeout mapping. Runtime timeout is exercised separately in the final capture evidence.
- The resulting bitmap represents Avalonia software re-rendering of the production Window, not a captured Vulkan viewport frame. The native viewport is outside this TOP-only evidence contract.

Decision: selected for the production XYC TOP capture path. It passes the approved per-profile fidelity thresholds and the measured no-focus/no-Z-order/occlusion requirements.

## B. Windows Graphics Capture

- `GraphicsCaptureSession.IsSupported()` returned true. A capture item was created from the real Editor HWND without a picker. A D3D11 FramePool frame was read back to PNG.
- All four standard profiles returned frames while the Editor was nonforeground; foreground HWND and preceding Z-order handle were unchanged around capture. Frame sizes were 1362×852 or 1922×852, including non-client frame pixels.
- TOP crops were compared using the measured crop rectangle and fixed P1-A GDI references. All four cases failed the user-approved fidelity thresholds (exact match 84.7–89.3%, MAE 6.56–15.57, PSNR 14.24–19.05 dB, >16 delta pixels 8.10–14.50%). No post-hoc alignment or threshold changes were applied.
- WGC resource release and standard session shutdown completed. Occlusion/minimized-specific WGC behavior was not selected for further runtime testing after its four fidelity failures; it is not integrated.

Decision: retain as a measured comparison only. The four real-frame PNGs and raw manifests remain under `evidence/rtb-probe/wgc-standard/`.

## C. Existing GDI/DWM compatibility path

- The existing implementation remains available by explicit `backend="gdiCompatibility"`.
- Source inspection confirms it calls `SetWindowPos(HWND_TOP or HWND_TOPMOST, SWP_NOACTIVATE)` and reads desktop pixels through GDI BitBlt, then attempts to restore the prior Z order. It samples the composed desktop, so covering the Editor can replace its pixels with the overlay.
- The four accepted P1-A images remain the fixed visual reference. GDI was not deleted or silently used by the new default.

## Limits

The tested Windows desktop was 96 DPI and 1920×1080. Other DPI scale factors and multi-monitor geometry have not been measured. The selected path requires a live production Avalonia Window and UI Dispatcher; it is not an offscreen Editor, does not capture the presented Vulkan frame, and does not eliminate the visible Editor window itself.
