# XYT-H / P3 Real Runtime Harness R1

本 Harness 只产生 `REAL RUNTIME AUTOMATED EVIDENCE PASS`，不产生 `PRODUCT PASS`，也不解除 `INC-2026-09-29-002`。P3 必须跨越真实 App、Window/Native Host、Vulkan Device、Swapchain 与 Present；Headless、源码读取和纯逻辑测试分别只能登记 P0/P1/P2。

## 使用

```powershell
pwsh -File scripts/governance/xyt-runtime.ps1 -Capability P3-01 -TimeoutSeconds 30
pwsh -File scripts/governance/xyt-runtime.ps1 -Capability P3-02 -TimeoutSeconds 30
pwsh -File scripts/governance/xyt-runtime.ps1 -Capability P3-03 -TimeoutSeconds 30
pwsh -File scripts/governance/xyt-runtime.ps1 -Capability P3-04 -TimeoutSeconds 30
pwsh -File scripts/governance/xyt-runtime.selftest.ps1
```

P3-01 要求 Surface、LogicalDevice、Swapchain 与首帧 Present。P3-02 对真实 XYE 窗口执行一次 Win32 resize，并要求 Swapchain 重建。P3-03/P3-04 要求真实 DEM、Terrain Render Path、Camera 与模式/Pointer Capture/Toolbar/DEM 保持标记；缺少标记时结果为 `BLOCKED_BY`，不是 Headless PASS。

默认真实运行路径每次先执行 `xye-bootstrap.ps1`、`resolve-dotnet.ps1` 与 `xye-dotnet.ps1 restore/build`，确认 `XuanYu.Editor.App\bin\Debug\net10.0\XuanYu.Editor.App.exe` 后才启动 App。Evidence 在 `.xyt/runtime/<RuntimeTestId>.json` 和 `.log` 记录 Version、Commit、Branch、Dirty State、App EXE Path、App Build Timestamp、GPU、Vulkan Adapter、Window Host、Vulkan Device、Swapchain、Present Evidence、Runtime Test ID、Capability、Start Time、Duration、Result、Evidence Path。Harness 启动失败、证据写入失败或自身异常统一为 `UNCLASSIFIED` + `HarnessFailure=true`，不得冒充产品 FAIL。

## 证据边界

`PASS` 只表示该 Capability 的自动化真实运行证据达到 P3；它不证明最终视觉、交互体验、业务正确性或用户验收。根因不明不得自动写入 PRODUCT ROOT CAUSE；允许状态仅为 `PASS / FAIL / TIMEOUT / FLAKY / BLOCKED_BY / UNCLASSIFIED`。

完成 R1 后报告 `P3 CAPABILITY READY`，由后续最小充分验证与 ChatGPT/用户裁决处理 T0 锁定。
