# Architecture 架构知识

## K-ARCH-002 产品模式持续膨胀时先建立 Workspace 边界

**状态**：Active
**优先级**：P1
**证据等级**：E1
**标签**：Editor、Workspace、Scope、Migration、Ownership
**适用范围**：一个编辑器工具同时承担多个独立产品模式、面板上下文或输入/渲染流程时。

**首次确认**：2026-08-11（UTC+08:00）
**版本**：`v0.2.25.33-fix`（MAP-A 战略收口基线）
**Commit**：`6724079`
**最近验证**：`v0.2.26.0-rz` / EDITOR-A-R1 Workspace Contract / `4cabf42`
**来源**：`MAP-A-R3-D2-F1-CLOSEOUT`、`R3-backlog.md`、MAP-A → EDITOR-A 单轮过渡计划。

### 问题

Map Editor 同时承载地图上下文、图层、Region Tool、Pointer、Picking、Draft、Render、Commit 与 History 时，产品模式的边界会被工具栏入口掩盖；失败发生后，局部修补很难判断应修改输入、领域、渲染还是产品归属。

### 根因

功能按“继续向一个工具追加入口”组织，而不是先明确 Workspace 身份、上下文插槽和切换时谁拥有临时工具状态；Region Drawing 的未验收路径因此与 Map Editor 的既有职责长期耦合。

### 工程规则

当一个工具持续吸收独立产品模式时，应先建立最小 Workspace Contract：Workspace Identity、布局上下文身份、唯一 Current Workspace Owner、Enter/Leave/Switch 与上下文保留不变量。稳定的 Domain、Camera、Picking 和 Renderer 合同优先迁移复用；Workspace 不得复制它们的权威状态。

### 禁止做法

- 仅靠继续增加 Toolbar/Panel 开关来表示独立产品模式；
- 为切换 Workspace 新建第二份 World、Camera 或 Selection 权威状态；
- 因产品归属调整而重写已验证的 Renderer、Picking 或 Domain 合同；
- 把旧路径的真机 FAIL 改写为新路径已通过。

### 正确做法

1. 先冻结旧路径的真实验收状态和迁移目标；
2. 建立纯 Editor 层的 Workspace 身份、定义和唯一 Manager；
3. 将临时 Tool 状态在切换边界结束，World/Camera/兼容 Selection 继续由既有 Owner 持有；
4. 先用合同测试证明切换不变量，再单独实施可见 Workspace UI 或 Region 能力。

### 真实历史示例

2026-08-11，`MAP-A-R3-D2-F1` 保留 `FINAL ACCEPTANCE FAILED · 5 ITEMS REMAIN`。用户批准旧 Region Drawing 产品路径以 `SUPERSEDED · NOT ACCEPTED` 终止并迁移到 `REGION-A`，同时要求同一 Transition Round 建立 `EDITOR-A-R1 Workspace Contract`，而非继续在 Map Editor 内修补 Region UI。

### 未来应用示例

当 Road Editor 或 Terrain Editor 需要独立 Toolbar、左右面板和主内容时，先注册独立 Workspace，并复用 World、Camera、Selection 和 Render Snapshot；不要向 Map Editor 添加更多条件分支或复制状态。

### 验证方法

- Workspace Manager 的默认、双向切换和重复切换回归；
- Current Workspace 只有一个 Owner；
- Workspace 层不依赖 Vulkan，且不保存 World/Camera 的第二份可写状态；
- 可见 UI 阶段另行进行真实窗口/输入验收。

### 适用边界

只有单一产品模式内的微小命令或面板显示变化，不应为了形式引入 Workspace。Workspace Identity 不是持久化 Schema，也不替代 Domain 的事实所有权。

**关联 Lesson**：L-ARCH-001
**关联 Knowledge**：K-VAL-002、K-INP-001、K-REN-001、K-REN-002

---

## K-SPA-001 大地图 Screen↔World CPU 链必须使用双精度并做往返验证

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Large World、Double Precision、Picking、Projection、DPI
**适用范围**：地图编辑器 CPU 投影、射线、Picking、屏幕尺度、测距、相机辅助算法。

**首次确认**：2026-08-10 12:20:03（UTC+08:00）
**版本**：`v0.2.25.12-rz`
**Commit**：`0594c4c`
**来源**：`changelog.md` / MAP-A-R3-D2-F1 Metric/Picking 精度门禁

### 问题

10,000～10,000,000m 大尺度地图、斜视相机与高 DPI 组合下，单精度 CPU 投影曾出现 `W=0` 和 Screen→World→Screen 超过 1 DIP 的误差。只验证“Pick 返回非空”无法发现坐标已经漂移。

### 工程规则

玄域地图编辑相关 CPU 空间计算默认使用 `double`。GPU 顶点阶段可以在明确边界转换为 float，但 CPU 的相机状态、投影、逆矩阵、射线构造和关键空间判断不得因为方便复用 GPU float 结构而降精度。

### 验证规则

空间功能必须优先建立往返不变量：

```text
Screen A
→ Ray / Pick
→ World P
→ Project
→ Screen B

|A - B| <= tolerance
```

并跨尺度、DPI、投影视角覆盖。

### 真实历史示例

`v0.2.25.12-rz` 将地图 Screen→Pick→World→Screen CPU 路径改为基于 `CameraState` / `ViewportState` 的双精度投影与射线构造，并加入 100m、10km、10,000km、多 DPI、正交、45°、80°自动回归，共 108 项 Metric/Picking 验证。

### 未来应用示例

新增“地图标尺点击生成测距点”时，测试不能只断言点被创建；必须检查原屏幕点投到地面后再投回屏幕仍在允许 DIP 误差内，并覆盖 100m 到 10,000km 尺度。

### 禁止做法

- CPU 侧为了直接复用 Shader 矩阵全部改用 float。
- 只用近距离俯视测试证明 Picking 正确。
- 把大尺度误差通过放大 Pick 半径掩盖。

### 边界

纯 GPU 视觉效果不自动要求 double；本条约束的是决定用户编辑语义的 CPU 空间链。

**关联 Incident**：INC-2026-08-10-003
**关联 Knowledge**：K-SPA-002、K-REN-001

---

## K-SPA-002 斜视 Metric 具有方向性，计算失败时保持上一合法状态

**状态**：Active
**优先级**：P1
**证据等级**：E1
**标签**：Metric、Camera、Oblique View、Fail-closed、Scale Indicator
**适用范围**：比例尺、Zoom Floor、屏幕世界尺度、斜视地图。

**确认日期**：2026-08-10（原始 changelog 未记录时分）
**版本**：`v0.2.25.17-stab`
**Commit**：`c307c66`
**来源**：`changelog.md` / MAP-A-R3-D2-F1 STAB-4A/4B/4C

### 问题

在斜视相机下，“1 DIP 对应多少米”不一定是单一、与方向无关的标量。把 X/Y 都强行映射成一个 `MetersPerDip`，会让比例尺或 Zoom Floor 在低角度出现不安全的尺度判断。

### 工程规则

当投影关系具有方向性时，模型必须显式保存 X/Y 或其它必要方向值，消费方根据语义选择。数值计算失败时，不应把非法值继续写入相机或把尺度退化成 0；应保持上一合法状态并暴露失败证据。

### 真实历史示例

`v0.2.25.17-stab` 将 Viewport Metric 拆为 X/Y 方向值：比例尺消费 X，Zoom Floor 取更安全的较小方向；Metric 失败时保持上一合法相机，而不是继续推进非法缩放。

### 未来应用示例

未来实现“屏幕上 100 px 的战线长度换算世界距离”，若摄像机 80° 斜视，应明确测量方向。横向 100 px 与纵向 100 px 可能对应不同世界距离，不能无条件共用一个值。

### 验证方法

至少覆盖俯视、45°、80°等角度；对 X/Y 分别断言；注入无法构造合法射线的情况，确认相机/缩放状态不被 NaN、0 或 Infinity 污染。

**关联 Knowledge**：K-SPA-001

---

## K-SPA-003 Cursor-Anchored Zoom Capability 必须保持 Screen→Surface 权威，Pure Dolly 不得修改 Pivot

**原始标题（2026-10-03）**：Cursor-Anchored Zoom 必须保持 Screen→Surface 权威，Pure Dolly 不得修改 Pivot
**状态**：Active
**Editor 全局适用解释**：SUPERSEDED
**Superseded By**：K-SPA-004
**优先级**：P0
**证据等级**：E1
**标签**：Camera、Zoom、Dolly、Terrain、Picking、Screen Anchor、Surface Authority
**适用范围**：Gameplay 或明确要求 Screen-Anchored Navigation 的交互；DEM Terrain、ReferencePlane fallback、Surface Source、Terrain Tile Edge。

**上下文边界**：本条约束 Screen→Surface Anchored Dolly capability，不再定义所有 Editor Wheel Zoom 的产品策略。原先将本条解释为“Editor Wheel 必须使用 Cursor Anchor”的全局规则，已由 K-SPA-004 supersede；Cursor Anchor capability 保留并继续有效。

**首次确认**：2026-10-03
**来源任务**：DEM-ZOOM-ANCHOR-DRIFT-R1
**Branch**：`feat/v0.3-world-authoring-r1`
**代码 Commit**：待补证（修复尚未提交；验证基线 HEAD=`a3d5787ba7c4a52821cb32f0bc895b1334f1daf9`，共享 Workspace Dirty=YES）

### 问题

真实 DEM 导入后，Wheel Zoom 会让鼠标所指地形点发生屏幕漂移。修复前，Wheel 输入本来携带 Screen XY，但 `UiVmD1Handler.HandleWheel` 丢弃坐标并调用无光标参数的 `DollyCamera`；同时 Anchored Dolly 又同步平移 `ObservationCenter`，把纯缩放错误地变成了 Pivot / Focus mutation。Terrain 越界查询还会抛异常，令 SurfaceSource fallback 缺少显式状态。

### 根因

Zoom Anchor 的语义权威没有贯穿完整链路：输入层丢失了 Cursor XY，而 Camera mutation 又混入了 Orbit Pivot 职责。结果是“有 Wheel 路由”和“有 Dolly”都成立，但固定 Screen Anchor、Surface Source 与 Camera 不变量没有形成单一合同。

### Capability 规则

Cursor-Anchored Zoom 的唯一权威链固定为：

```text
Wheel Screen XY
→ GroundPickResult
→ SurfaceSource + World XYZ
→ Pure Dolly
```

- 有效 Terrain Hit 时，必须使用完整 Terrain World XYZ；不得把 Z 归零，也不得再由 ReferencePlane 覆盖。
- Terrain 无效或越界时，必须返回可判定状态，再显式 fallback 到逻辑 ReferencePlane；不得用异常或静默默认值表达 Surface Source。
- Pure Dolly 只允许修改 Camera Position。Forward、Up、FOV、Orbit Pivot、Focus、Target 与 RenderOrigin 必须保持不变。
- Zoom 不是 Orbit，不得通过 `LookAt`、重算 Target、平移 ObservationCenter 或回到 DEM 中心来补偿漂移。
- 输入路由不仅要“接通”，还必须把决定交互语义的 Screen XY / Wheel Delta 等 payload 原样保留到最终 Consumer。

### 禁止做法

- Handler 接收到 Cursor XY 后改调无光标 Dolly；
- Terrain Hit 有效时 fallback 到 ReferencePlane 或强制 Z=0；
- Anchored Dolly 同步平移 ObservationCenter / Pivot / Focus；
- 用 smoothing、magic offset、经验比例或每帧 recenter 掩盖 Anchor 漂移；
- 把 Terrain OutOfBounds 作为异常路径，让调用方猜测是否 fallback。

### 验证方法

固定同一 Screen XY，执行 Zoom In/Out round trip，并同时验证：

```text
Screen XY → Surface P0
Zoom
Screen XY → Surface P1

World XY error
World Z error
Screen reprojection error
Forward angular error
Pivot / Focus invariance
SurfaceSource invariance
```

至少覆盖平地、山坡、高差区域、Tile Edge 与 DEM 外 ReferencePlane 区域；多 Tile、LOD Surface Resolver 或 RenderOrigin 变更后必须重新验证。

### 本轮证据

修复前固定屏幕点误差为 `18.1115 px`，Pivot 从 `(3,2,4)` 变为 `(2.7,2.9,3.55)`。修复后 ReferencePlane 对照序列最大屏幕误差约 `0.000852 px`，In/Out round-trip Camera Position 误差 `8.35e-6`，Forward angular error `≤1e-9`；真实 HGT 集成测试中 Terrain World XY / Z error 均 `≤1e-5`，有效 Terrain Hit 未被 ReferencePlane 覆盖。Core Camera/Render/Map/Vulkan 373/373、DEM/Zoom/Viewport 17/17、真实 HGT Cursor Anchor 1/1 PASS。P4-C 真实用户操作验收仍为 PENDING，因此本条保持 E1，不宣称完整 Product Closure。

**关联 Knowledge**：K-SPA-001、K-INP-003、K-VAL-002
**关联 Lesson**：L-REN-002

**Governance Supersede**：事件 `EDITOR-ZOOM-AUTHORITY-SCOPE-SUPERSEDE-R1`；原 Editor 全局适用解释已由 K-SPA-004 取代。原事故、根因、验证数值与 capability 规则保留。

---

## K-SPA-004 Editor / Gameplay Zoom Authority 必须按产品上下文分离

**状态**：Active
**优先级**：P0
**证据等级**：E1
**标签**：Camera、Navigation Center、ObservationCenter、Editor Zoom、Gameplay Zoom、Orbit、Product Policy
**适用范围**：Editor Navigation；Gameplay 或明确要求 Screen-Anchored Navigation 的交互。

**确认日期**：2026-10-05
**来源任务**：`GOVERNANCE-SUPERSEDE-K-SPA-003-EDITOR-NAVIGATION-R1`
**Effective Baseline**：`4277b242391e273080c4c19b82c9acf53b348843`（用户报告为 LAST USER-VERIFIED GOOD，P4 PASS）
**Supersedes**：K-SPA-003 将 Cursor-Anchored Zoom 提升为全局 Editor Wheel Zoom policy 的解释；不取代 K-SPA-003 的 Screen→Surface capability。

### 问题

K-SPA-003 的 Screen→Surface Zoom 技术 capability 曾被提升为通用 Editor Zoom Authority。技术能力存在，不代表所有产品上下文都必须使用该能力。Editor Navigation 与 Gameplay / 显式 Screen-Anchored Navigation 必须分开定义。

### Editor 产品合同

- Editor Navigation Center 的权威是当前 `ObservationCenter`。
- 默认 Editor / Empty World 的 `ObservationCenter` 为 World Origin `(0,0,0)`。
- Editor Wheel Zoom 与 Orbit 共用当前 `ObservationCenter`；Zoom 不依赖鼠标位置，不执行 GroundPick、Surface Resolve 或 viewport ray repick。Dolly 地形净空可按下方 Scope Clarification，对候选相机路径位置做只读 Surface 查询；不得用它选择导航中心。
- Orbit Begin 冻结当时的 `ObservationCenter` 为该 Session Pivot；Orbit Move 不重新解析 Surface 或刷新 Pivot。
- `ObservationCenter` 只能由已定义的 Center Authority Event 改变：Empty World 初始化设为 World Origin；初次建立内容并执行 Initial Content Establishment Auto Frame 时，设为该 framing 的中心；Pan、Explicit Focus、Explicit Frame / View command 及其它正式授权的 View Reframe 操作可设为各自结果中心。
- 普通 Zoom、Orbit、Selection、Terrain LOD、Terrain refresh 与普通 Render update 不改变 Navigation Center。新内容的初次建立若触发正式 Auto Frame，是明确的 framing authority event，不属于任意数据状态变化；后续普通数据/渲染变化不得隐式夺取中心。
- Editor Wheel 与 Orbit 仍只读取当前 `ObservationCenter`；内容建立后，它们继承 Auto Frame 产生的新中心，不重新使用鼠标、GroundPick 或 Render Surface 选择中心。

### 2026-10-05 治理澄清

原文“DEM Import 本身不得改变 Navigation Center”范围过宽，遗漏了 4277 中已验证产品流程包含的 Initial Content Establishment Auto Frame。该规则现澄清为：普通数据状态变化不得任意修改 Navigation Center；正式的初次内容建立自动取景与显式 View Reframe 是合法 Center Authority Event。此澄清不恢复 Cursor-Anchored Editor Zoom，也不改变 K-SPA-003 capability 的范围。

### Gameplay / 显式 Screen Navigation capability

Gameplay 或明确要求 Screen-Anchored Navigation 的交互可以使用 K-SPA-003 capability：

```text
Screen XY → GroundPickResult → SurfaceSource + World XYZ → Anchored Dolly
```

Screen XY 可以来自 Mouse Cursor。Router 必须保留决定交互语义的输入 payload；有效 Surface 命中、SurfaceSource 与 fallback 必须明确。该 capability 不构成 Editor Wheel policy，也不得作为 Editor 使用 Cursor Anchor 的强制条件。

### 2026-10-10 Scope Clarification：候选相机只读净空查询

用户产品权威裁定：允许 Editor Wheel 在应用 Dolly 地形约束时，对**候选相机位置**执行狭义只读 Surface 查询，以检查相机净空。该查询只返回净空约束所需数据，不得执行 GroundPick、重选导航中心或改变 `Anchor`、Orbit Pivot、`ObservationCenter`；Editor Wheel 的缩放权威仍是当前 `ObservationCenter`。

该许可不等同于批准任意 Terrain Query。实现必须固定被约束的 Surface（源高度场、TerrainWorld 查询面或渲染 LOD 面不得混称），并定义 NoData、无效数据、查询失败、垂直夸张及边缘/采样策略；失败时不得静默改用未经声明的平面或另一 Surface。当前 Wave 2 CPU 合成测试不能证明真实 DEM 或 Vulkan 可见表面的净空，真实 Runtime/P4 仍独立验收。

此澄清限定 K-SPA-004 中“Editor Wheel 不执行 Terrain Query”的范围，不恢复 Cursor Anchor，不修改 ObservationCenter Authority，也不追认未验证的 Surface 等价关系。

### 2026-10-10 XYK Strengthen：起点入地时的 Zoom-Out 脱困

**Problem**：Editor Camera 的起点已经处于有效地形内部时，地形净空约束可能导致 Zoom-Out 无法移动。

**Root Cause**：旧的 `ConstrainDollyToTerrain` 在候选路径首次触发不安全判断时停止，保留 `lastSafe=start`，阻止相机脱困。

**Evidence**：`XYEPR2-NAV-SAFETY-INSIDE-001` 修复前 1 FAIL、修复后 1 PASS；Evidence Level 为 CPU 确定性回归，Commit=`5b464dcd219cf07d1569c56e5dbd05e6656beed2`，不等同于 Windows/Vulkan P3。

**Rule**：当相机起点已经处于已声明 Terrain Surface 的无净空状态时，普通 Editor Zoom-Out 应具有向外恢复能力。恢复过程必须保持 `ObservationCenter`、Zoom Anchor 和 Orbit Pivot 权威不变。相机开始向外移动不代表已经脱离地形；完整安全保证需要额外证明最终有效净空、路径状态及必要的失败处理。

**Known Limits**：64 段路径采样、NoData、Tile 边缘、高山脊、Render LOD 和真实 Vulkan 可见表面尚未完整证明。

**Suggested Action**：本条作为 K-SPA-004 的 STRENGTHEN 增量，不新建重复 Knowledge ID；继续为窄峰、边缘、NoData、真实 NASADEM/HGT 和 Vulkan 可视表面补充独立证据。

### Governance Supersede Record

- 事件：`EDITOR-ZOOM-AUTHORITY-SCOPE-SUPERSEDE-R1`
- 原因：产品上下文此前未区分 Editor 与 Gameplay，导致 capability rule 被误提升为全局 product authority。
- 决策：Scope Split / Supersede。K-SPA-003 的 Editor 全局 policy 解读由本条替代；其技术 capability 与历史事故证据保留。
- Effective Baseline：`4277b242391e273080c4c19b82c9acf53b348843`
- 批准：USER PRODUCT AUTHORITY + CHATGPT KNOWLEDGE AUDIT
- 日期：2026-10-05
- Version Event：NONE

### 替代证据与边界

- 用户报告 4277 (`fix: reconcile frozen orbit pivot contract`) 已通过 P4：DEM Import、DEM Orbit No-Refresh、DEM Orbit No-Flicker、Grid、ScaleBar、Pan 与 Navigation Chain 均 PASS。该验收事实来自用户授权/报告；本治理改动不伪称重新执行 P4。
- 用户明确决策：Editor Zoom / Orbit 共享稳定 Navigation Center；默认是 World Origin，Pan / Explicit Focus 后使用新的 ObservationCenter。
- 用户报告恢复候选 `fa55953a`、`d10adb31`、`8fb7e8ae`、`f75badb4` 均为 P4 FAIL；这些失败候选不用于定义本条合同。
- K-SPA-003 的 Screen XY payload、GroundPick SurfaceSource、完整 Screen→Surface capability 与有效命中不得被 fallback 静默覆盖等工程知识继续有效。

**关联 Knowledge**：K-SPA-001、K-SPA-002、K-SPA-003、K-VAL-002
**关联 DEC**：DEC-CAM-001

---

## K-ARCH-001 Composition Root 初始化顺序属于真实依赖合同

**状态**：Active
**优先级**：P0
**证据等级**：E1
**标签**：Composition Root、Initialization Order、Dependency、Startup
**适用范围**：Editor 组合根、Route/Service 装配、启动期依赖。

**确认时间**：2026-06-24 11:45（UTC+08:00，changelog 记录）；对应 Git Commit 时间为 2026-06-24 11:42:40（UTC+08:00）
**版本**：`v0.1.7.1-fix`
**Commit**：`359e3cee71f08b9a683753f089d53f01b4c5e7b2`
**来源**：`docs/archive/changelog/changelog-2026-06.md`、Git Commit `8.8-RZ-Fix1`

### 问题

某个服务字段“最终会被赋值”不代表构造期可以提前引用它。Composition Root 中的初始化先后关系实际上就是一张依赖图；顺序错误可在编译和大部分单测通过的情况下直接导致启动崩溃。

### 真实历史示例

`EditorShellComposition.Build()` 创建 `ProjectBootstrapRoute` 时传入 `ctx.HierarchyRoute`，但 `HierarchyRoute` 在之后才赋值。结果启动时 `hierarchyRoute` 为 null，用户看到退出码 `0xC0000005`，实际根因是 `NullReferenceException`。修复只是把 `HierarchyRoute` 初始化移到 `ProjectBootstrapRoute` 之前。

### 工程规则

若 `A` 构造需要 `B`：

```text
B = new B(...)
A = new A(B)
```

顺序就是合同。组合根不允许依靠 nullable 默认值、字段后赋值或“运行到使用点前应该已经初始化”作为隐式假设。

### 未来应用示例

新增 `RegionToolRoute` 依赖 `MapPickRoute` 与 `HistoryRoute` 时，必须先完成后两者构造再创建 RegionToolRoute；如果存在循环依赖，应重构边界，而不是把字段改成 nullable 并在运行时碰运气。

### 验证方法

- 启动冒烟必须属于组合根变更的正式验证；
- 对关键组合根可增加依赖非空合同测试；
- 构造签名优先接收已完成依赖，不在构造内部读取可能尚未赋值的共享 Context 字段。

### 注意

该历史 Commit 信息里曾记录旧时期 Warning 容忍口径；这只是历史事实，**不覆盖当前全解决方案 0W0E 门禁**。

**关联 Incident**：INC-2026-06-24-001
