# XYC-B4-D-NATIVE-CONTROL-AUDIT

- 任务：`XYC-B4-D-NATIVE-CONTROL-AUDIT`
- 审计基线：B3 HEAD `10af215415a33cb26c4fccf81cb7452ef800d70a`
- 实际审计 checkout：branch `recovery/editor-center-navigation-r1`，HEAD 与基线一致；Bootstrap 状态 `READY`，SDK `10.0.400`。
- Dirty：`XuanYu.Editor.UI/Diagnostic/TerrainCameraReplayRunner.cs`、`tools/terrain/Invoke-WP2-P4-VulkanReplay.ps1`。仅只读检查，未覆盖/改动。
- 范围：代码、既有测试和仓库记录审计。未启动编辑器/Vulkan 交互实例，未占用 GPU；未运行测试。
- 写入：本任务目录内仅本报告。

## 结论摘要

XYC 当前已经具备**可通过 CLI 启动隔离的真实 XYE Editor 子进程，并无鼠标键盘注入地控制相机、加载固定 NASADEM、等待关联呈现帧及获取 Vulkan Swapchain color PNG**的实现。它不是仅有规划；但其 `offscreen` 明确标记 `NOT-IMPLEMENTED`，已有 surface-free offscreen 工具是独立离屏 Terrain Renderer，不是后台控制真实 Editor。

因此，最小原生回放流程已基本可执行；**控制和 color-frame 部分可进入受控的真实编辑器 P4 replay 准备/执行阶段**，但本轮没有运行，不能报告 P4 PASS。现有接口没有真实 Editor depth readback，也没有与被捕获呈现帧绑定的 Chunk/LOD 逐帧状态查询。若本次 P4 定义要求这些证据，则须先补充诊断接口；不得以离屏基准的 chunk/LOD 或 GPU depth attachment 存在，替代 Editor 本帧证据。

## 能力矩阵

| 能力 | 状态 | 依据与限制 |
|---|---|---|
| 独立进程/后台控制协议 | **已实现** | `XuanYu.Editor.Control.Cli` 启动随附 `XuanYu.Editor.App.exe`，参数 `--xyc-stdio` / `--xyc-session`；stdin/stdout 为 JSON-lines，协议 `XYC-API/1`。它是独立 Editor 进程，不是 headless/offscreen Editor；主窗口仍会创建。见 `XuanYu.Editor.Control.Cli/Program.cs`、`XuanYu.Editor.UI/Control/XycStdioHost.cs`、`XycAppSession.cs`。 |
| Runtime 自动化 API | **已实现，范围受控** | `runtime.info`、`scene.inspect`、`camera.inspect`、`camera.set-test-pose`、`camera.orbit`、`capture.arm`、`capture.release`、`session.close`。无通用 UI 自动化、脚本执行、任意命令或任意文件导入 API。见 `XuanYu.Editor.UI/Control/XycCommandProcessor.cs`。 |
| 相机 Pose 注入 | **已实现** | `camera.set-test-pose` 校验有限值/姿态，经 `UiVmXycRuntime` 的 UI Dispatcher 调 `vm.WriteXycCamera`，返回 before/after 与 revision；不是直接改影子 Camera。需要 CLI 显式 `--diagnostic-write`。见 `UiVmXycRuntime.cs`、`XycCameraValidator.cs`、`XycCommandProcessor.cs`。 |
| 无鼠标 Zoom / Orbit | **部分已实现** | Orbit 可用 `camera.orbit`，复用 Editor Camera Navigation begin/preview/end 合同，不发真实 Pointer/鼠标事件。可用连续 `camera.set-test-pose` 实现确定性 zoom/pose 变化；协议没有语义明确的 `camera.zoom` 命令或原生滚轮事件。见 `UiVmXycRuntime.cs`、`XycOrbitCommandTests.cs`。 |
| NASADEM 导入 | **已实现，测试策略固定** | CLI `--test-dem` 需要 `--diagnostic-write`，路径必须是仓库登记的 `artifacts/diagnostics/wp2-p4-r3/n24e121.hgt`，且 SHA-256 必须为 `BEF653...1340E1`；随后调用产品 `ImportTerrainSourceAsync`。不能导入任意 DEM。见 `Program.cs`、`XycTestDemPolicy.cs`、`XycAppSession.cs`、`XycTestDemPolicyTests.cs`。 |
| 真实 Editor PNG | **已实现，证据链有两种来源** | `ViewportProbe` 可对目标 Vulkan Swapchain frame GPU→host readback，输出 PNG + sidecar（FrameId、CameraRevision、ImageIndex、设备、格式、hash）；capture arm 将捕获关联到相机 revision。`Invoke-XYC-WP2-NoMouse.ps1` 还用 Windows `PrintWindow` 抓窗口，但其自身标为 `PARTIAL / WINDOWED`，不应把窗口抓图当 Vulkan 像素来源。关联方式和具体采集不是完全由 CLI 完成：脚本读取探针输出。见 `ViewportProbe.GpuCapture.cs`、`ViewportProbe.Present.cs`、`tools/terrain/Invoke-XYC-WP2-NoMouse.ps1`。 |
| Depth 图 | **未实现为 XYC 捕获能力** | Vulkan Editor 有 depth attachment/管线，但审计到的 XYC 命令和 GPU capture 输出只有 Swapchain color PNG，没有 depth image readback/导出命令或 depth 元数据。存在 depth buffer 不等于可采集 depth 证据。 |
| Chunk/LOD 状态 | **渲染侧有诊断探针；XYC API/回放 manifest 未集成** | `VulkanClearFrameOwner.Terrain.cs` / `.Terrain.Replay.cs` 计算可见/裁剪 Chunk、所选 LOD 与三角形统计并调用 `TerrainProbeLogger`；`XYE_TERRAIN_REPLAY=1` 可启用。这是 Editor 渲染侧真值，但 `scene.inspect` 不返回这些记录，候选脚本也未校验探针日志与 `presentedFrameId` / `cameraRevision` 同帧关联。因此“有探针”已实现，“稳定可消费的同帧采集合同”未完成。Offscreen benchmark 的 probe 另算，不能代替 Editor。 |
| 无干扰启动/关闭 | **应用进程层已实现；桌面隔离不成立** | CLI 启动独立进程，协议 `session.close` 让 Editor shutdown；测试 `XycProcessE2ETests` 验证关闭。实现不调用鼠标、键盘注入或前台激活，但窗口化 Editor 仍可能显示/覆盖桌面；不保证桌面内容完全不受视觉干扰，也没有真正 headless Editor 模式。异常时 CLI 有 kill child tree 的兜底。 |
| 可验证的进程级测试 | **已有测试，未在本轮执行** | `XycProcessE2ETests` 覆盖真实 CLI 子进程、runtime/scene/camera、Pose revision、Orbit、session close；其他合同测试覆盖权限、协议、Orbit、capture 权限和 DEM 路径策略。测试源码/历史 changelog 不能替代当前机器新鲜运行证据。 |

## 最小可执行原生回放

现有接口足以做**窗口化真实 Editor、无鼠标输入**的受控回放：

1. 通过 `scripts/xye-dotnet.ps1` 构建 CLI 与 Editor App；验证目标 exe/dll 是同一 build identity。不要直接运行离屏 `XuanYu.OffscreenTerrain` 来代替此步骤。
2. 检查 NASADEM 路径及固定 SHA-256；由 `XuanYu.Editor.Control.Cli --app <EditorApp.exe> --session-id <unique-id> --diagnostic-write --test-dem <fixed HGT> --timeout-seconds <bounded>` 启动独立进程。控制端只读写 CLI stdin/stdout，不发 OS 输入事件。
3. 发送唯一 requestId 的 `XYC-API/1` JSON-lines：`runtime.info`、`scene.inspect`、`camera.inspect`；检查 build、PID、terrain id/revision、首个 presented frame。
4. 对每个姿态发送 `camera.set-test-pose`，使用返回/回读 camera revision；`capture.arm(cameraRevision)` 后轮询 `scene.inspect`，仅当 `presentedFrameId` 前进且 `presentedCameraRevision` 匹配时记为有效呈现帧。
5. 保存关联 GPU PNG 和 sidecar；对 Orbit 使用 `camera.orbit` 后重复 arm/wait/capture。结束调用 `capture.release`、`session.close`，并验证子进程退出；异常时用 CLI 超时/进程兜底并收集 stderr。
6. 建议序列：固定初始 pose → 20 km pose → 2 km pose → `camera.orbit(45,25)` → 返回原 pose。每步记录 request/ack 时间、pose/revision、scene/terrain revision、FrameId、ImageIndex、GPU device、PNG hash。避免单靠 `PrintWindow` 判定渲染结果。

已有候选脚本 `tools/terrain/Invoke-XYC-WP2-NoMouse.ps1` 已实现固定 DEM、Pose、Orbit、frame/revision 轮询、GPU PNG 校验和关闭流程，但在本 checkout 它**不是本任务 write scope**，且工作树显示该路径邻近的另一个 WP2 replay 脚本为 dirty。此处仅作为静态参考，不建议审计任务直接运行它。

## 最小后续接口方案（仅在 P4 证据要求 depth/chunk 时）

避免扩大成通用自动化平台。先只增加只读诊断采集：

- 在 `XuanYu.Core/Diagnostics/ViewportProbe` 及 Vulkan present/readback owner 加一个默认关闭、按 capture token/frame 请求的 depth readback；sidecar 写明 depth 格式、clear/reverse-Z 语义、FrameId、CameraRevision 和 hash。仅把 attachment 对象 dump 出来不足以证明目标帧。
- 增加 `XycRenderSnapshot` 只读响应，在 `scene.inspect` 可选字段或专用 `render.inspect` 命令中返回**与同一 presented FrameId 绑定**的可见 Chunk/LOD/triangle/draw counts；复用现有 `TerrainProbeLogger`/渲染侧数据，完成稳定采集和 FrameId/CameraRevision 关联，不从 Offscreen benchmark 重算。
- 在 Editor Window/Swapchain 创建阶段添加显式 `--xyc-window-mode` 诊断策略：若当前 Avalonia/Vulkan 后端不能安全无激活/隐藏启动，明确归类为 windowed P4，并停止宣称桌面视觉隔离；不要通过伪造 HWND 或只隐藏窗口推导真实 present 成功。
- 为新响应、授权/默认关闭、capture frame 关联增加隔离合同测试；在用户批准实际 RTX Vulkan 回放窗口前，继续保持不启动 GPU 交互实例。

不建议为当前目标改生产输入系统；也不需要任意 DEM、真实滚轮模拟或一般 UI automation。当前 `camera.orbit`/pose 注入足以完成算法可重复的导航回放。

## 成本与风险估算

| 工作项 | 估算 | 风险/边界 |
|---|---:|---|
| 复用现有 API 写/执行 bounded native replay，补 manifest 证据 | 0.5–1 人日 | 中：要核验进程退出、探针 frame/revision 关联和真实 GPU PNG；目前未运行，无运行结果。 |
| XYC 同帧可见 Chunk/LOD 只读快照 + 合同测试 | 1–2 人日 | 中：需确认 Vulkan draw/chunk owner 在呈现时可稳定取得一致快照。 |
| Editor depth GPU readback + sidecar + 测试 | 2–4 人日 | 中高：barrier/layout、反向 Z、帧同步、host copy 与资源生命周期，不能用离屏 depth 冒充 Editor depth。 |
| 后台/隐藏窗口可控启动及可见性验证（如硬性要求） | 2–5 人日调查/实现 | 高且不确定：Avalonia/Vulkan swapchain 可能依赖可见/可呈现 HWND；隐藏窗口是否继续呈现必须实机验证，不能承诺 headless。 |

估算不包含大规模自动化平台、生产输入系统修改、任意 DEM 导入或产品 UI 变更。

## P4 判定

- **可以进入**：以当前已实现接口开始一次受控的真实 Editor、无鼠标/键盘注入、窗口化 Vulkan color-frame replay；前提是执行者先确认 GPU 窗口可见性影响可接受，并使用独立进程/唯一 session。
- **不能判定已通过**：本轮没有启动/运行实例，没有新鲜 PNG、frame manifest 或用户目视验收证据。既有 changelog 记载 Native Windows/Vulkan P3 与 WP2 用户 P4 仍未执行/待验收，需以当前原始证据重新确认。
- **若 P4 要求完整 depth + chunk/LOD 同帧证据**：当前不能满足，先完成上文最小只读诊断接入，再安排受控 GPU run。无论自动化证据如何，最终产品视觉验收仍归用户。

## 证据索引

- `XuanYu.Editor.Control.Cli/Program.cs`：目标 Editor 校验、test DEM guard、stdio 子进程和超时关闭。
- `XuanYu.Editor.UI/Control/XycAppSession.cs`、`XycStdioHost.cs`：会话启动、输入协议、session close。
- `XuanYu.Editor.UI/Control/XycCommandProcessor.cs`、`IXycRuntime.cs`、`UiVmXycRuntime.cs`：命令集、权限、UI dispatcher、真实相机写入/Orbit 和回读。
- `XuanYu.Editor.UI/Control/XycTestDemPolicy.cs`：固定 NASADEM 路径和 SHA 策略。
- `XuanYu.Core/Diagnostics/ViewportProbe.GpuCapture.cs`、`ViewportProbe.Present.cs`、`XuanYu.Render.Vulkan/Render/Present/VulkanPresentLoop.TerrainProbe.cs`：Swapchain color GPU readback 与 frame/revision 关联；`XuanYu.Render.Vulkan/Render/Terrain/VulkanClearFrameOwner.Terrain.cs`、`.Terrain.Replay.cs`：Editor 可见 Chunk/LOD 诊断探针；`VulkanDepthAttachment.cs`：depth attachment 存在但非 XYC 导出。
- `XuanYu.World.Tests/Control/XycProcessE2ETests.cs`、`XycCommandProcessorTests.cs`、`XycOrbitCommandTests.cs`、`XycCaptureCommandTests.cs`、`XycTestDemPolicyTests.cs`：既有合同与进程级测试源码。
- `tools/terrain/Invoke-XYC-WP2-NoMouse.ps1`：既有原生回放候选脚本（未在本轮运行）。
- `tools/xyc-offscreen/Program.cs`、`VulkanOffscreenTerrainRenderer.cs`、`BenchmarkProbeReader.cs`、`TerrainBenchmarkRunner.cs`：真正 offscreen Terrain PNG 与 benchmark Chunk/LOD，和 Editor runtime 控制分开。
- `changelog.md` 的 v0.3.0.18–.21：记录 XYC 控制、color capture、无鼠标脚本以及各阶段 P3/P4 的历史状态；历史记录不等于本轮运行证据。
- `docs/knowledge/knowledge-index.md`：K-XYC-001（相机权威与执行后回读）、K-XYC-002（回读状态与截图必须关联呈现帧）。

## 变更与验证

- Changed：仅新增本审计报告。
- Verified：Bootstrap、HEAD 与指定 B3 SHA 一致、dirty 路径检查、静态源码/测试/历史记录审计。
- Not run：构建、测试、编辑器、Vulkan、GPU 回放；没有声称运行通过。
- 风险：`Invoke-XYC-WP2-NoMouse.ps1` 和部分 probe/output 路径需要在正式回放前核对当前 build wiring；本报告未验证可直接在当前 dirty checkout 一键执行。
- 5+100：仅一份 Markdown 分析报告；无生产代码/测试脚本变更。Foreign dirty 两个文件保全。Candidate Tree 只含授权目录内报告。

XYK CANDIDATE:
NONE — 本任务形成能力状态审计，未发现需要提出的新长期知识条目；既有 K-XYC-001/002 已覆盖相关核心规则。

