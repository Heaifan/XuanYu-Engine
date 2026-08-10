# Architecture 架构知识

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
