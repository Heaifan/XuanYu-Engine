# 玄域引擎事故库

> 整理时间：2026-08-10 17:56（UTC+08:00）
> 事故库记录“发生了什么、为什么、最终如何收口”；可复用结论已提升到对应 `K-*` 条目。
> 本文件只放具有明确工程复用价值的代表性事故，不追求把每个 Fix 都登记成事故。

## INC-2026-08-12-001 删除图层确认被 Native HWND 覆盖并存在业务路由漏分支

**发生日期**：2026-08-12（UTC+08:00）
**来源 Milestone**：MAP-DATA-A-R2-F2-F2 / F2-F2-F1
**最终功能收口 Commit**：`3d53de05c49c958cd1821303f8c6e302c2abe2ef`
**影响**：点击图层“删除”后主 Avalonia UI 失去输入，而 Vulkan 视口仍可操作；确认界面不可见。

### 已确认事实

1. 主窗口 `DialogCard` 覆盖 `VulkanNativeHost : NativeControlHost` 的 Native HWND 时，Dialog 逻辑 Active、Esc 可取消，但 Native HWND airspace 使卡片不可见。
2. 普通删除首先改为 Editor Owned Avalonia Window；随后真机仍失败。
3. P1 Runtime Probe 的决定性证据是 `REQUEST_RECEIVED name=解除注册数据集`：Dataset-backed 图层并不进入普通“删除图层”分支，而是走旧 `ShowDanger()` Overlay/DialogCard。

### 最终修复

- 普通删除与 Dataset-backed 解除注册共用独立 Owned Confirmation Window；
- 领域语义保持不同：普通 Layer 删除，Dataset 从当前地图解除注册且磁盘文件保留；
- 两条路径在确认前捕获稳定 `LayerId`，确认后按 ID 重解析目标；
- 一次性 Runtime Probe 在得到路由证据后删除，未进入正式提交。

**经验提升**：K-NATIVE-001、K-VAL-002、K-DATA-003、L-VAL-001。

---

## INC-2026-06-24-001 Editor Composition 初始化顺序导致启动崩溃

**发生时间**：2026-06-24 11:45（changelog）
**版本**：`v0.1.7.1-fix`
**Commit**：`359e3cee71f08b9a683753f089d53f01b4c5e7b2`
**影响**：Editor 无法正常启动。
**现象**：退出码 `-1073741819 / 0xC0000005`，表象类似 native AccessViolation。
**根因**：`ProjectBootstrapRoute` 构造时引用 `ctx.HierarchyRoute`，但后者尚未初始化，实际是 NullReferenceException。
**最终修复**：调整 Composition Root 构造顺序，让 `HierarchyRoute` 在依赖它的 Route 前完成。
**经验提升**：K-ARCH-001。

---

## INC-2026-06-25-001 Gizmo Preview 高频路径被 UI/Diagnostics 重工作拖慢

**发生窗口**：2026-06-25 00:18 ～ 22:56（UTC+08:00）
**版本链**：`v0.1.8.7-fix` → `v0.1.8.8-fix` → `v0.1.8.9-fix`
**关键 Commit**：`26f2006`（首轮修复；完整 SHA 待本地 Git 补证）
**影响**：Move Gizmo 拖动帧负载过高，诊断代码本身存在卡顿风险。
**根因**：TransformPreview 曾刷新 Inspector/Diagnostics/PickSnapshot；首轮优化后，Frame Complete 仍残留 Diagnostics refresh 路径。
**最终修复**：Preview 只保留轻量渲染链，Commit 才写 World/UI；Probe 验证 Preview 中相关重操作为 0。
**经验提升**：K-PERF-001。

---

## INC-2026-06-26-001 Native Viewport Mouse Capture 未真实释放

**发生日期**：2026-06-26（changelog 未记录时分）
**关键 Commit 时间**：2026-06-26 09:42:31（UTC+08:00）
**版本**：`v0.1.8.10-fix`
**Commit**：`8d6e7fd9ef6f430c0888f83e3dd8b1901501d741`
**影响**：Native Viewport 可能继续吞鼠标；UI 点击无反应、Gizmo hover 有反馈但拖不动、窗口关闭卡顿。
**根因**：`WM_MBUTTONUP` 只清内部状态，没有可靠 ReleaseCapture；Release 又过度相信内部 `_captured`，缺少 WM_CANCELMODE / Destroy 等兜底。
**最终修复**：Capture API 集中管理，以 GetCapture() 为真实依据，覆盖 ButtonUp/CancelMode/KillFocus/Destroy/Dispose；CaptureChanged 仅同步。
**经验提升**：K-INP-002。

---

## INC-2026-08-01-001 背景全屏三角写 Depth 遮挡静态模型

**发生时间**：2026-08-01 16:56:53（UTC+08:00）
**版本**：`v0.2.21.21-fix`
**Commit**：`e0a994ae11b7d7a2c383d3e4a6e4100385c46ecf`
**影响**：模型不能完整显示，继续缩放后才出现。
**根因**：主管线启用 DepthTest/DepthWrite 后，背景三角仍写 `z=0.98`，先占深度。
**修复演进**：先把背景 depth 改为 1.0；`v0.2.22.0-rz` 再建立天空专用 `DepthTest=Off / DepthWrite=Off` Pipeline。
**经验提升**：K-REN-003。

---

## INC-2026-08-02-001 Real GLB BaseVertex 被重复应用并触发 GPU 失败/日志风暴

**发生时间**：2026-08-02 12:45:00（UTC+08:00）
**版本**：`v0.2.21.23-fix`
**Commit**：`a9c1ec6c302dce5efec2215931eafb58eb9b4f75`
**真实资产**：`german_ss_soldier_mp40.glb`；211,517 Vertices / 926,148 Indices。
**现象**：导入前半段成功，GPU 创建失败：`non-zero BaseVertex not supported`；同一错误后续反复刷屏。
**根因 A**：索引已做 `localIndex + baseVertex` 全局化，但 Primitive 元数据仍留非零 BaseVertex。
**根因 B**：失败没有 Key+Revision 记录，每次投影更新都重试。
**最终修复**：Normalize 后 `BaseVertex=0`；新增 `VulkanStaticModelFailureTracker` 负缓存。
**经验提升**：K-ASSET-001、K-ASSET-002。

---

## INC-2026-08-02-002 托管资产覆盖保存需要可回滚事务

**确认时间**：2026-08-02 14:10:00（UTC+08:00）
**版本**：`v0.2.21.24-rz`
**Commit**：`e0893253a4d7bf27dbcdb5a8f3d308aef9be583d`
**风险**：若覆盖正式 `.xyassets` 时中途失败，可能破坏旧资产根。
**解决**：建立 Prepare/Activate/Complete/Rollback 状态机，staging 与 backup 保证旧数据优先恢复；路径策略防逃逸。
**经验提升**：K-DATA-001。

---

## INC-2026-08-02-003 Scene Load 若直接修改当前状态会导致失败污染

**确认时间**：2026-08-02 15:30:00（UTC+08:00）
**版本**：`v0.2.21.25-rz`
**Commit**：`cafe400fff6a1dde179d011ec14ddc9dfb3a5724`
**风险**：结构错误在加载后半段才发现时，当前场景可能已经被部分替换；单资源缺失又不应让整个场景报废。
**解决**：Candidate World/Catalog/Resources 构建完成后一次 Commit；结构失败整场拒绝且旧状态不变；资源 Missing/Failed 使用占位并保留实体语义。
**经验提升**：K-DATA-002。

---

## INC-2026-08-09-001 LayerPanel 冷启动错位与拖拽热区过小

**根因确认**：2026-08-09 16:18:16（UTC+08:00）
**版本链**：`v0.2.24.49-fix` → `v0.2.24.50-fix`
**最终 Commit**：`60fd339`
**现象**：冷启动布局不稳定，某些操作后恢复；拖拽难以命中。
**根因**：ScrollViewer 横向无限测量使 `*` 列失去合理宽度；拖拽事件直接绑定 14 DIP Path。
**最终修复**：限制横向滚动/保持 Stretch、Auto/Auto/* Grid；24×28 透明 Border 作为热区；新增 Avalonia.Headless Runtime UI Gate。
**经验提升**：K-UI-001、K-VAL-002。

---

## INC-2026-08-10-001 Region Tool 与 Navigation Gizmo 争夺同一 Pointer 手势

**确认时间**：2026-08-10 11:48:28（UTC+08:00）
**版本**：`v0.2.25.9-fix`
**Commit**：`d621755`
**现象**：Region 激活后点击/拖动 Gizmo 可能误加 Region 点或 Move 路径被 Region Preview 抢走。
**根因**：多个输入链各自消费 Down/Move，没有完整手势唯一 Owner。
**最终修复**：恢复并统一输入仲裁与会话清理；`v0.2.25.15-stab` 再统一 Gizmo 命中/所有权。
**经验提升**：K-INP-001。

---

## INC-2026-08-10-002 Region Overlay 世界 Z 偏移与多套 Depth Workaround 叠加

**事故收口窗口**：2026-08-10 13:37:23 ～ 2026-08-10（`v0.2.25.17-stab` 原文未记时分）
**版本链**：`v0.2.25.13-rz` → `.14-fix` → `.15-stab` → `.17-stab`
**关键 Commits**：`ef12f4b`、`8c8dfdd`、`751da52`、`c307c66`
**问题**：Stroke 曾用 `BaseHeight + 0.03` 世界偏移制造层级；随后又尝试 Clip-Z Bias。
**最终修复**：世界锚点统一；建立独立无 Depth Overlay Pass + Fill→Stroke→Marker；删除过期 Clip-Z Bias。
**经验提升**：K-REN-001、K-REN-002。

---

## INC-2026-08-10-003 大尺度 Picking 单精度产生 W=0 / >1 DIP 往返误差

**确认时间**：2026-08-10 12:20:03（UTC+08:00）
**版本**：`v0.2.25.12-rz`
**Commit**：`0594c4c`
**影响范围**：10,000～10,000,000m 场景、多 DPI、斜视相机。
**根因**：Screen→Pick→World→Screen CPU 路径存在单精度计算。
**最终修复**：CameraState/ViewportState 双精度投影与射线，建立 108 项跨尺度/角度往返门禁。
**经验提升**：K-SPA-001。

---

## INC-2026-08-10-004 比例尺 Native Overlay 假验证：窗口层级问题叠加 App 输出副本未同步

**收口时间**：2026-08-10 16:51:42（UTC+08:00）
**版本链**：`v0.2.25.17-stab` → `v0.2.25.18-stab`
**最终 Commit**：`06b26e9`
**现象**：自动验证与源码显示已修，但用户真机仍看不到/看到旧行为。
**根因组合**：Native Overlay 真实 HWND 层级需要从 sibling 模型继续调整；同时 App 输出副本未同步，造成验证对象与用户运行对象不同。
**最终修复**：比例尺变为主窗口 owned `WS_POPUP`；新增 HWND/Visible/Rect/Text/WM_PAINT 探针；修复 App 输出同步；重启 Editor 后真机看到 `100 m`。
**经验提升**：K-VAL-001、K-NATIVE-001、K-VAL-002。

---

## INC-2026-08-10-005 历史版本号重复导致追溯歧义

**审计标识**：`SHR-2026-08-R2`
**审计时间**：2026-08（原始注记未登记具体日与时分）
**涉及版本**：`v0.2.16.2-rz`、`v0.2.17.8-rz`、`v0.2.20.19-fix` 均出现重复分配；另有 18 处版本号/日期非单调。
**处理原则**：历史原文保留，不重排；追溯以 Commit Hash 为准。
**经验提升**：K-GOV-001。

---

## INC-2026-08-10-006 World Reference Grid 与 MapGround 错误耦合导致持续闪烁排障

**发生窗口**：2026-08-10 22:34:00 ～ 23:50:35（UTC+08:00）
**版本链**：`v0.2.25.26-fix` → `v0.2.25.29-fix`
**关键 Commit**：`c1451df`、`2c57893`、`6154078`
**影响**：World Grid 在缩放、低角度观察时出现闪、抖、断与密度不稳定；Region 周边视觉也可能被误判为自身不稳定。

### 已确认事实

- 旧 Grid 为世界空间 LineList，使用 `DepthTest=On`、`DepthWrite=Off`、`LessOrEqual` 与负 Depth Bias；
- Grid 的旧平面语义依赖 Map BaseHeight，与 Ground 存在深度承载耦合；
- Ground 隔离实验改变了旧 Grid 表现，World Axis 在相同条件下连续缩放稳定；
- RW-2A 采用独立 Fullscreen Triangle、世界射线与 World XY（Z=0）求交、DepthTest/DepthWrite 关闭后，Ground ON/OFF 均可独立显示；
- RW-2B 采用 CPU 全帧唯一 Step、1/2/5 序列与 24~80 DIP 回滞，真机通过；Region 同时观察到不再闪烁。

### 高置信机制解释（尚未直接 GPU 捕获证明）

共面 Depth 竞争与世界空间 1px LineList 亚像素覆盖变化共同造成主要闪烁。该解释没有 GPU Capture 的直接证明，故不作为已确认根因事实。

### 最终收口

World Reference Grid 被重新定义为独立 Editor Environment Layer：不属于 Map Surface，不读取 Map BaseHeight，不依赖 Ground Depth；LOD/Step 只由 CPU 全帧统一决定，Fragment 的 `fwidth` 只用于 AA。

**经验提升**：L-REN-001、K-REN-004（并关联 K-REN-001、K-REN-002）。

---

## INC-2026-09-28-001 World Reference Grid 在 Reverse-Z 迁移中违反既有 Environment Layer 合同

**事故等级**：T0+
**事故对象**：World Reference Grid
**全局系统**：Reverse-Z Migration
**最终修复 Commit**：`b95b2e7925b117463b4b301251b1a0516118a7fb`

### 已确认事实

- K-REN-004 已将 World Reference Grid 定义为 Editor Environment，而非 Map Surface，并要求 `DepthTest=Off`、`DepthWrite=Off`、不依赖 MapGround Depth、Ground Bias 或 Terrain Depth。
- Reverse-Z 迁移中，Grid 被改为 `DepthTest=true`、`DepthWrite=false`、显式 `gl_FragDepth`，进入 Scene Reverse-Z Depth Contract。
- 真机表现为空场景 Grid 不可见，导入 DEM 后行为变化；后续多轮局部修复仍未解决。

### 根因

Reverse-Z migration 错误改变了 World Reference Grid 的 Layer Identity / Depth Contract。Reverse-Z 全局契约本身不是根因。

### 错误修复路径

先后尝试 fixed-function Depth Bias、shader-side `gl_FragDepth` bias 与 Grid MinSpacing / Scale 调整，均未恢复正确的独立环境层语义。

### 历史回放实验

保持全局 Reverse-Z：`Near=1`、`Far=0`、`ClearDepth=0`、`Compare=GreaterOrEqual`；仅恢复 `DepthTest=false`、`DepthWrite=false`、`FragDepth=ABSENT`。结果为 `VISUAL RESULT = GRID APPEARS`，远处闪烁同时消失。Shader Source / Embedded SPIR-V 已确认同步。这里不把未经 GPU Capture 证明的机制解释写成 GPU 已证事实。

### 最终恢复合同

Grid 保持 World XY / Z=0 的独立 Editor Environment Layer；不进入 Scene Geometry Depth Contract，不依赖 Ground 或 Terrain Depth，不使用 Ground Bias；CPU 全帧统一 Step，`fwidth` 仅用于 AA。

### 治理根因

仓库已经存在 K-REN-004 与 L-REN-001，但 Reverse-Z 迁移规划与执行没有真正消费 ACTIVE Knowledge，导致已解决事故回潮；连续错误调参也没有及时触发历史知识停止线。

### 防复发措施

新增 `ERR-20260928-001` 与 `EXP-GOVERNANCE-002`，将 Knowledge Preflight 升级为 Planning + Execution 双门禁；任何与 ACTIVE P0 Knowledge 冲突的计划或实现必须 STOP，除非完成显式 Governance Supersede。

### Machine Gate Candidate

`WorldReferenceGridEnvironmentContract` 登记为下一步机器 Gate Candidate：应断言 `ReferenceGrid.DepthTest=false`、`ReferenceGrid.DepthWrite=false`、World Reference Grid shader 的 `FragDepth` absent，并静态或运行时拒绝 Ground Depth dependency 与 Ground Bias workaround。本治理任务不修改产品 Test 项目，故不宣称该 Gate 已实现。

**关联**：INC-2026-08-10-006、K-REN-002、K-REN-004、L-REN-001、ERR-20260928-001、EXP-GOVERNANCE-002。

---

## INC-2026-09-25-001 Diagnostic + Viewport 长周期开发复盘 R1

**确认时间**：2026-09-25（UTC+08:00）
**范围**：Diagnostic Mode、Native Viewport、Input Router、Vulkan-Avalonia Composition、Region Snap Revalidation。
**证据基线**：A1.5 PASS `f67210debb3a4f201662933b0d549a3939d8850d`；WAVE-2.5 E0/E5 审计与收口材料；Region Snap R1 复盘材料。

### 总结

本轮问题不是单一 Popup、坐标或 Alt 修补，而是 UI Ownership、Airspace、Input Ownership、Coordinate Space 与 Runtime Route 长期混在一起，并叠加“自动测试不等于生产链路、源码正确不等于用户运行正确、Helper PASS 不等于 Runtime PASS”。

### 已确认的长期结论

- Avalonia 是唯一 Editor UI / Window Owner；NativeControlHost/HWND 只能作为迁移期遗留路径。
- A1.5 GPU Composition 技术可行性已由 `f67210de` 证明，但生产迁移仍未完成。
- Diagnostic 只能观察，不能抢 Pointer、Capture 或建立第二套输入 Owner。
- Native 与 Avalonia 坐标必须通过 Screen Space 等显式空间转换。
- Input Router 只有接入真实生产 Source 才算完成；一次 Gesture 只能有一个 Owner，Cancel 是生命周期终态。
- 平台差异必须止步于 Adapter；测试不能用错误平台前提制造假阳性。
- 同类真机问题连续两次局部修复无效后，必须审查共同依赖和承载架构。
- 仓库当前入口和 Resolver 高于 Agent 历史环境记忆。

### 本轮已落库

`K-NATIVE-002`、`K-INP-003`、`K-INP-004`、`K-DIAG-001`、`K-GOV-003`、`L-NATIVE-001`、`L-TEST-001` 已加入知识库与 Preflight 索引；架构文档已把 A1.5 从 pending 修正为 PASS，同时保留“生产迁移未完成”的事实边界。

### 待 ChatGPT 正式治理入库的候选

受 `AGENTS.md` 权限规则约束，以下内容由 Codex 登记为候选，不冒充正式 ERR/EXP：

ERR 候选：`ERR-20260925-001` 至 `ERR-20260925-005`，分别对应生产 Router bypass、Diagnostic 阻塞输入、`0x0020` 错判 Alt、Avalonia Alt 测试伪造 Win32 值、绕过仓库 Resolver 判断 SDK。

EXP 候选：`EXP-ARCH-001` 连续局部修复停止线、`EXP-TEST-001` 生产链测试原则、`EXP-UI-002` Diagnostic Observer Rule、`EXP-GOVERNANCE-001` Repository Authority First。

### 后续机器 Gate 候选

`ProductionInputNoBypass`、`DiagnosticInputTransparency`、`NativeCoordinateRoundTrip`、`PlatformKeyNormalization`、`ProductionPathTestRule`、`ViewportLegacyAllowlist`、`CanonicalRunResolver`。本记录不把尚未执行的 Gate 宣布为已通过。


---

## INC-2026-09-29-001 · XYT 中央入口并行 Ownership 冲突

**日期**：2026-09-29
**XYT 事故等级**：T1
**状态**：CLOSED
**影响范围**：XYT-A / XYT-F Report Integration
**相关提交**：A `abd5416c9691512f75087fb2413f8c10bed25bbc`；F2 `6066b6c3193be94a2597b96b1226c47e3811f1dc`

### 事件

XYT-A 与 XYT-F 在同一并行 Wave 中同时拥有 `xyt.ps1`。A 先提交统一入口后，F 检测到 HEAD 变化与文件冲突，按规则停止 Commit/Push，没有覆盖 ForeignDirty。

### 影响

造成 F Lane 明确返工和延迟，但未污染 XYE 产品代码，也未拖慢全部主线，因此定级 T1 而非 T0。

### 根因

并行任务在下发时只划分了功能范围，没有提前识别 `xyt.ps1` 这种中央 Integration Hotspot 的未来写入竞争。

### 处置

F2 保留 Report 能力，将实现移动为独立 Report Module；禁止 F2 修改中央入口。入口后续由唯一 Integration Owner 接线。

### 预防

`Central Entry = Single Owner`；
`Subsystem = Module Contract`。

**关联 Lesson**：L-XYT-001


---

## INC-2026-09-29-002 · 旧测试证据层级失真与 P3 真空

**日期**：2026-09-29
**XYT 事故等级**：T0
**状态**：CLOSED
**影响范围**：Legacy Test Evidence、Runtime Acceptance Evidence、旧自动化 PASS 的证据解释
**来源**：XYT-G Legacy Test Clean-Room Audit
**Commit**：待补证

### 事件

XYT-G 对旧测试体系进行 Clean-Room Audit：扫描 696 个测试源文件、2552 个测试方法，得到 P0=281、P1=284、P2=131、P3=0；37 项需 DOWNGRADE，95 项需 RENAME，P4 明确缺失。

### T0 定性原因

这不是单个测试写错，而是证据体系层面的系统性缺口。旧体系存在以 Runtime/Churn/Performance 等名称表达高层能力、实际验证边界却停留在 P0~P2 的风险。它会让自动测试 PASS 被误解为真实运行证明，造成错误收口、重复返工与开发效率损失。

### 当前隔离

本事故不停止 XYE 主线开发。

T0-LOCK 范围仅包括：
- 禁止把尚未重新登记的旧测试当作 P3/P4 正式证据；
- 禁止基于旧 Runtime/Churn/Performance 名称直接宣称真实运行能力已证明；
- 正式 Runtime 结论必须由新增 P3 能力或人工 P4 提供。

P0~P2 测试仍可按其真实等级继续使用；与此证据链无关的并行开发继续。

### 处置方向

1. 完成 G 审计文件正式提交；
2. 运行 G-INTEGRATION，把审计后的 P-Level 写入 Registry；
3. 优先整改 DOWNGRADE / RENAME 高风险项；
4. 启动 P3 Runtime Harness 建设；
5. P4 继续保持用户最终 IPO 验收；
6. 历史正式结论按影响范围标记为需重验，不无差别推翻全部历史 PASS。

### 解锁条件

完成首批高风险证据纠偏，并建立可用 P3 路径后，对受影响能力执行一轮最小充分测试；通过后可按用户最终裁决解除对应 T0-LOCK。

### 2026-09-29 H3 P3 路径建立

H3 已在正式远端提交 `ccd3dbef94864466ec2a7fc73c4ee6074e5809b2` 建立 Canonical P3 Runtime Harness，并通过真实 App / HWND / Vulkan Device / Swapchain / Present 取得 P3-01 REAL PASS。P3-02 独立保持 TIMEOUT，不覆盖 P3-01。

因此本 Incident 的“P3=0 真空”已经解除；Legacy HIGH 风险测试也已完成首轮 Capability Resolution（129 RESOLVED / 3 REVIEW_REQUIRED）。当前 T0 状态升级为 `READY FOR FINAL CLOSURE AUDIT`，仍等待 L2 与 XYT-INTEGRATION-R1 证明统一入口能够正确消费 Registry / Planner / Executor / Runtime / Evidence / IPO，再由 ChatGPT + 用户最终裁决是否 CLOSED。

### 2026-09-29 Final Closure

XYT-INTEGRATION-R1 已在正式提交 `475ab10dfdde94898725acf4607ee8d09ae1e9dc` 完成中央入口接线与 Dogfood。

关闭证据：

- P0 / P1 / P2：PASS；
- P3-01：REAL PASS，Evidence Identity VALID，可被统一入口消费；
- P3-02：TIMEOUT / independent，未覆盖 P3-01；
- P4：保持 `P4 PENDING`，未被自动升级；
- HIGH 132 条 Legacy Capability 已完成首轮治理：129 RESOLVED，3 REVIEW_REQUIRED；
- Registry 其余 567 条 `REVIEW_REQUIRED` 保持显式未决状态，没有伪装成 PASS，也不参与正式 Required Test 选择；
- `xyt fast / module / global` Dogfood 全部 PASS；
- PRODUCT REGRESSION = NONE OBSERVED；
- UNRESOLVED UNKNOWN = NONE；
- CANDIDATE TREE MATCH = PASS；
- COMMIT ELIGIBILITY = PASS；
- HEAD == Remote，Ahead/Behind = 0/0，Commit Mutex 已释放。

因此本事故的两个系统性根因均已被关闭：

1. 旧测试证据层级失真已通过 P0~P4 Canonical、Registry、Capability Resolution 与 Required Test Selection 约束；
2. P3=0 真空已由真实 Runtime Harness 与中央入口消费链正式解除。

未完成的 567 条 Capability Resolution 属于显式、可追踪的后续治理库存，不再构成 T0，也不阻塞正常开发。

最终裁决：`INC-2026-09-29-002 = CLOSED`。

**关联 Knowledge**：K-VAL-002、K-XYT-AUDIT-001、K-XYT-P3-001、K-XYT-P3-002


### 2026-09-29 G2 统计完整性补充

G2 在正式 Registry Migration 前发现 G 审计摘要的 Risk 统计与逐行明细不一致。重新按 696 个唯一 `XYT-G-*` 明细计数确认：HIGH=132、MEDIUM=237、LOW=327；Actions=696，P-Level=696。问题定性为报告/Oracle T2，处于本 T0 Incident 的处置范围内；迁移在摘要修正前保持 BLOCKED。该问题不改变 P3=0、DOWNGRADE=37、RENAME=95 的核心审计结论。


### 2026-09-29 G3 False PASS 补充

G3 Legacy Registry Migration 曾报告 PASS，但 ChatGPT 审计发现 696 条记录把 `REVIEW_REQUIRED` 写入 `targetCapability`。现行 `test-registry-policy.md` 明确要求 `targetCapability` 中每个键必须存在于 `test-capability-map.json`，而 Capability Map 中不存在该键。

因此 G3 PASS 被撤销，状态改为 `FALSE PASS / BLOCKED`。根因属于迁移表示层 / Oracle Contract 错误：把“能力解析状态”混入“能力身份命名空间”。

处置原则：`REVIEW_REQUIRED` 必须作为独立解析状态保存；未决记录保持 BLOCKED，且不得进入 Capability Query / Required Test Selection。该发现属于当前 `INC-2026-09-29-002` T0 的处置范围，不新建重复 Incident ID。


### 2026-09-29 P3 Harness 首轮阻塞补充

XYT-H 建立 P3 Runtime Harness 后，首轮 P3-01 返回 `BLOCKED_BY`，直接原因是当前路径下未找到 `XuanYu.Editor.App.exe`。Repository Audit 随后确认正式 `run.bat` 会先通过 `scripts/resolve-dotnet.ps1` 获取 SDK，再 Rebuild `XuanYu.Editor.App`，并明确运行产物路径。

因此此次阻塞的已确认根因属于 Harness / Prerequisite 链未闭合，而不是 Product Failure，也不能据此宣布 Environment 缺失。真实 P3=PASS 仍未获得；`INC-2026-09-29-002` 保持 ACTIVE / CONTAINED。


---

## INC-2026-09-29-003 · XYT 聚合 Selftest 文本 PASS 与进程 Exit Code 不一致

**日期**：2026-09-29
**XYT 事故等级**：T1
**状态**：CLOSED
**影响范围**：XYT Aggregate Selftest / Machine Gate

### 事件

`XYT/tests/xyt.selftest.ps1` 在验证 unknown mode 必须失败时，子 `pwsh.exe` 正确返回非零，并把父进程 `$LASTEXITCODE` 留为 2。脚本随后成功完成所有断言并打印 `XYT SELFTEST PASS`，但没有显式 `exit 0`，导致调用方观察到“文本 PASS、进程返回 2”。

### 影响

不会造成 Product Failure，但会让人工报告与自动化 Gate 对同一次 Selftest 得出相反结论，导致正常治理任务被错误阻塞，也可能使只读取文本的 Agent 误报 PASS。

### 根因

父级 Selftest 没有在“预期的子进程失败测试”之后归一化自己的最终机器退出码。Expected Child Failure 与 Parent Test Verdict 没有隔离。

### 修复

Selftest 在全部断言完成并输出 PASS 后显式 `exit 0`。后续必须同时验证 PASS 文本与进程 Exit Code=0。

### 解锁条件

重新执行 Aggregate Selftest，确认：
- 输出包含 `XYT SELFTEST PASS`
- 进程 Exit Code = 0

验证完成后可关闭本 T1。

### 关闭证据

- `xyt.selftest.ps1` 输出 `XYT SELFTEST PASS`，父进程 Exit Code = 0；
- unknown mode 仍正确返回 `Status: INVALID_MODE`，Exit Code = 2，证明预期失败路径未被吞掉；
- Aggregate Selftest 输出 `XYT AGGREGATE SELFTEST PASS`，Exit Code = 0；
- 首次 Aggregate 调用出现一次子进程文件占用竞争，未修改代码，独立等待重跑与最终直接重跑均通过；当前不升级为新 Incident，后续若重复发生再按 FLAKY/Harness 规则处理。

因此该 T1 满足关闭条件。

**关联 Knowledge**：K-XYT-HARNESS-001


---

## INC-2026-09-29-004 · Handoff Commit Mutex 缺少 Stale Recovery 导致 Convergence 阻塞

**日期**：2026-09-29
**XYT / Governance 事故等级**：T1
**状态**：CLOSED
**影响范围**：XYT-L2 Integration Contract Convergence、并行收口串行提交口

### 事件

本地 Commit Mutex 长期保持：

- owner = `XYT-C-CONVERGENCE`
- scope = `GOVERNANCE`
- acquiredAt = `2026-09-29T06:23:51.6042186Z`

期间 Remote HEAD 已由多个合法治理提交持续推进，但 L2 仍被旧 mutex 阻塞。仓库审计确认当前 Handoff Mutex 只有原子创建、Owner 校验和正常 advance/unlock，没有 lease、heartbeat、TTL 或 stale recovery。

### 定性

这是治理控制面的 T1：没有污染产品代码，也不要求 XYE 主线停线，但已经阻塞一个已验证 PASS 的正式 Convergence，并可能继续制造并行任务等待。

### 当前隔离

禁止普通 Agent 手工删除、覆盖或冒用旧 owner。先建立受控 recovery / user override 路径，再恢复 L2 与 H3 收口。

### 关闭条件

- stale mutex 被可审计地恢复；
- ForeignDirty 保持不变；
- L2 / H3 可重新取得正常 commit mutex；
- Handoff 增加防止永久 stale lock 的 recovery contract / selftest。

### 2026-09-29 关闭证据

- H3 通过正式 `advance` 将 Baseline 推进至 Canonical H3 Commit，并正常释放旧 Commit Mutex；
- 随后 L2 成功重新获取 Commit Mutex、完成 Commit / Push / Advance 并正常释放；
- L2 正式远端 Commit：`18842310247abac42b125816645afeccb6947dcb`；
- 最终 HEAD == Remote、Ahead/Behind = 0/0、Staged = 0、ForeignDirty 保留。

因此本次 T1 的实际阻塞已经解除并满足关闭条件。Stale-lock Recovery 的长期增强需求由 K-HANDOFF-001 保留，不再阻塞 XYT R1 收尾。

**关联 Knowledge**：K-HANDOFF-001
