# XYC-WP2-P4-R8 三档 DEM 静默 Vulkan 对照

日期：2026-10-17（Asia/Shanghai）
分类：真实 Windows Vulkan Swapchain Present；诊断实验，不是用户 P4

## 执行身份

- Branch：`recovery/editor-center-navigation-r1`
- Source HEAD：`71fc8c365fcdd83166b9d57fdd123fe81316d618`
- BuildIdentity：`HEAD=71fc8c365fcdd83166b9d57fdd123fe81316d618;DIRTY=YES;PROBE=INSTRUMENT-VIEWPORT-ZOOM-GROUND-DEM-R1;BUILD=2026-10-17T20:33:33.0362820+08:00`
- GPU：NVIDIA GeForce RTX 3060；视口 776×650；Release Editor / XYC CLI
- DEM：`n24e121.hgt` SHA-256 `BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1`
- 每个样本独立静默进程；监测先于子进程启动。3 个采样档均为 `HIDDEN-HWND-SWAPCHAIN-PRESENT`，Visible=false、WS_VISIBLE=false、OwnProcessForeground=false，Monitor 无违规，进程退出码 0。
- 9 张图均通过 PresentedFrame / CameraRevision / Readback 文件名 / Readback SHA / 落盘 PNG SHA 逐帧对账。完整记录及 PNG 位于 `artifacts/diagnostics/wp2-p4-r8/native-ab/`；过程日志位于对应 `XycSilentProbe` 临时目录及 `native-ab-v2/runner-console.log`。

## 采样选择与净空

Replay-only 采样选择器支持 513、1025、2049；非 replay 与未指定 override 时继续使用生产默认 513。净空查询复用渲染 Heightfield 同一三角面插值和垂直夸张，报告相机 XY 处的垂直 Z 差，不代表视线距离或最短欧氏距离。诊断记录全场分区 Chunk 数与完整规则网格的理论三角形数；每 Chunk 实际 LOD、可见 Chunk 数和实际提交三角形尚未从渲染器暴露，画面差异只能解释为采样密度与网格/LOD 联合影响。

## 实验姿态

- 固定姿态 2 km：沿用已记录 R7 Camera Pose；513、1025 的本次 ScaleBar 显示 `2 km`。2049 同样世界相机姿态下地表投影查询返回无效，ScaleBar 未显示；该帧只用于固定世界姿态图像观察，不把其标记为 2 km ScaleBar PASS。
- 500 m 姿态：从 2 km 相机位置通过生产 `camera.dolly` 连续正向增量 10 次到 `ScaleIndicatorText == 500 m`，约束标记均为 false；固定其相机世界位置、方向、FOV、Near/Far 和 ObservationCenter供三档复放。
- 固定世界姿态时的渲染面/净空：

| Heightfield | 分区 Chunk | 全规则网格理论三角形 | 相机处渲染面 Z | 相机垂直净空 |
|---:|---:|---:|---:|---:|
| 513×513 | 9 | 524,288 | 1866.61 m | 2447.76 m |
| 1025×1025 | 25 | 2,097,152 | 1868.65 m | 2445.72 m |
| 2049×2049 | 81 | 8,388,608 | 1869.56 m | 2444.81 m |

- 匹配净空组将相机和 ObservationCenter 一起沿 Z 平移，保持相同相对姿态；净空统一为 2447.76 m。1025、2049 分别需约 +2.04 m、+2.95 m Z 平移。匹配净空组用于体验观察，不与固定姿态组做逐像素差分。
- 上表三角形数是未降 LOD 的单一完整规则网格理论数，非 GPU 实际提交量；几何上界相对 513 为 4× / 16×。Chunk 分区数为所有 Chunk，不是可见 Chunk 计数。没有采集帧时、内存峰值或 GPU 成本，不据此宣称运行时成本。

## 结果

同姿态 GPU 图像可见地显示 513 的山脊/沟谷较平，1025 恢复明显轮廓，2049 细节最多。匹配净空组也观察到相同方向的改善。可重复的 RGB 差异只代表图像变化量，不是感知画质评分。固定 500 m 姿态 ROI `(160,100)-(760,600)`：513→1025 RGB MAE 2.644、任一通道差值 >20 的像素 0.208%；513→2049 MAE 4.212、比例 2.325%；1025→2049 MAE 2.289、比例 0.221%。Auto LOD 在本轮三档始终启用；采样变化同时改变分区拓扑，故未隔离出采样密度的唯一因果，也未证明 Auto LOD 是主因。既有 R4 的一组 Auto 与 LOD0 同姿态结果相同，但只适用于该历史姿态。

本次样本未出现明显大片地形缺口或屏幕边界异常截断；这不能否定其他历史视角下的截断问题。2049 的 2 km ScaleBar 无效是独立观测，须保留为比例尺地表投影边界，不能归类为地形缺失。

## 九帧身份

| 姿态 | 档位 | ScaleBar | Present Frame / CameraRevision | PNG SHA-256 |
|---|---:|---|---|---|
| 固定 2 km | 513 | 2 km | 2308 / 4 | `E92E17F83F1D99D48BD85ECF75550E4B651B4DBA429883F1D7B73D6902FF5B18` |
| 固定 2 km | 1025 | 2 km | 1666 / 4 | `5769E3A8A290CBE5D16C4A4E1D9CEA29EBD41E787D9F6D54E57E5C029B173DF6` |
| 固定姿态 | 2049 | 无效/隐藏 | 2275 / 4 | `CFA8D5C42A549ADDE73ED4EEF970776C3D56894550728E49A68AFBD283289714` |
| 固定 500 m | 513 | 500 m | 2336 / 14 | `CCCABFD5997DF570422F3D4559496553D792AB9A11194CDDF48D3FB69B9BB73E` |
| 固定 500 m | 1025 | 500 m | 1669 / 5 | `04E7C467A610DC65A5F74AD883F8DB92FD9A5FDD8F05FB9E32CA153A1CC346E3` |
| 固定 500 m | 2049 | 500 m | 2277 / 5 | `D028AB5903995EA2497AEB543FB613DDC0ADA5CD8737167C39E82635B16509CD` |
| 匹配净空 500 m | 513 | 500 m | 2346 / 15 | `CCCABFD5997DF570422F3D4559496553D792AB9A11194CDDF48D3FB69B9BB73E` |
| 匹配净空 500 m | 1025 | 500 m | 1672 / 6 | `4FD46824C1C82BAEA1928AB7A7005D541ACB8E24AB8EF1E12963D857131D86F64` |
| 匹配净空 500 m | 2049 | 500 m | 2279 / 6 | `2E1E5B1E1B0ADDED2108AB7A7005D541ACB8E24AB8EF1E12963D857131D86F64` |

## 验证与边界

- `TerrainDiagnosticSamplesTests`：5/5 PASS；R8 Release Editor App build：0 warnings / 0 errors；R8 Release XYC CLI build：0 warnings / 0 errors。
- `git diff --check`：PASS（仅输出既有 XYUI 文件 CRLF 提示）。登记 Candidate 文件均在 100 行以内。
- `scripts/arch-a-guard.ps1`：未通过；报告 `XuanYu.Render.Abstractions.csproj` 引用 `XuanYu.Render.Vulkan`。该项目文件未修改，属于当前基线架构问题，不由 R8 引入。
- 工作区含 XYUI Dirty 与先前 WP2 诊断 Dirty，保持原样。BuildIdentity 为 DIRTY=YES，故不是干净树构建声明。
- 不修改生产 513 默认、Auto LOD、产品地形策略或版本；不把本实验视为用户 P4。

## 决策

R8 提供了真实 GPU 画质证据，支持“高度场密度/生成网格共同影响近地轮廓”的判断；仍不能单独认定 513 是唯一根因。1025 可作为成本与体验之间的下一候选评估，但尚无帧时和显存测量，不能切换生产默认。建议下一步先补实际每 Chunk LOD/可见拓扑及同 pose 的提交几何量；若需生产决策，再补 1025 与 2049 的帧时/显存成本。WP2-P4 继续 PARTIAL，等待用户 P4 视觉验收。
