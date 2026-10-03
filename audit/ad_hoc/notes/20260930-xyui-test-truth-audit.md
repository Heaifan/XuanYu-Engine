# XYUI Test Truth Audit — 2026-09-30

- Audited `E:\MyDoc\project-VSCode\XuanYuEngine\xyui\avalonia\tests\XYUI.Avalonia.Tests`: 147 C# files, 660 xUnit attributes, and a fresh full run of 706 passed / 0 failed / 0 skipped.
- Truth layers must remain separate: style/property contracts, headless measurement, headless control behavior, synthetic input, real desktop interaction, and visual appearance are distinct evidence classes.
- No screenshot, RenderTargetBitmap, pixel, or equivalent visual assertion exists in this test scope; passing headless/property tests cannot establish visual acceptance.
- Tests named VisualState or Real_pointer may still be only Avalonia Headless MouseDown/MouseMove or RaiseEvent evidence; classify by execution path, not test name.
- Popup/dropdown/menu/toolbar lifecycle coverage is meaningful headless or synthetic evidence, but real interactive behavior and desktop popup-root behavior remain ESCALATE until a real app/device gate is executed.
