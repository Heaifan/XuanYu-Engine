# 玄域引擎工程教训库

> 教训记录“为什么会沿着错误前提持续投入”，并定义何时必须停止局部修补、回到共同依赖与承载架构审查。

## L-REN-002 双精度回退必须发生在第一次降精度之前

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Large World、Camera、Projection、Float Precision、Failure Safety
**确认时间**：2026-08-11（UTC+08:00）
**来源**：F1 极远 Dolly 真机异常栈；`ViewProjection 矩阵不可逆`。

### 已确认事实

- 相机与世界位置使用 `Vector3d`，但现有 ViewProjection 建立会将 eye 与 target 转为 `Vector3`。
- 极远 Dolly 可触发不可逆 ViewProjection；异常曾从名称为 Try 的 Metric 路径穿透至 UI。
- 已有 WorldRay 的双精度分支若必须先创建 ViewProjection，便不能保护这一次 VP 创建本身。

### 教训

精度保护不得放在第一次 float 转换或 float 矩阵构建之后。任何“大世界 double fallback”都必须能在不构造该 float 对象的前提下判定、诊断并安全失败。

### 正确做法

1. 把可能失败的 VP 建立包装为真正不抛异常的 Try API。
2. 在危险构建之前，用相机 basis、FOV 和视口的 double 几何收集诊断。
3. 失败时保持 UI 与上一帧合法状态；不得让 Metric、Scale 或诊断成为崩溃入口。
4. 长期方案另行评审 Camera-relative Rendering / Render Origin Rebasing，禁止在 F1 安全轮次偷偷引入。
5. 动态安全边界必须可随当前需求收缩；禁止把历史峰值直接作为下一帧 FarPlane、预算或精度范围的下限。

### 验证

- 极远距离回归必须证明 Metric/RenderProjection 不抛异常。
- 用户日志必须能读取预 VP 的距离、Near/Far、Metric 与中心射线数据。
- 真机继续覆盖 Grid、Axis、Scale 与 Editor 存活性。

---

## L-REN-001 连续参数修补失败时必须重新审查承载架构和错误前提

**状态**：Active
**优先级**：P0
**证据等级**：E3
**标签**：Rendering、Architecture Review、Depth、Grid、Stop Condition
**适用范围**：编辑器辅助层、Vulkan Pass、Depth/Blend/Draw Order、出现跨组件视觉不稳定的任何渲染路径。

**确认时间**：2026-08-10 23:50:35（UTC+08:00）
**版本链**：`v0.2.25.26-fix` → `v0.2.25.29-fix`
**关键 Commit**：`c1451df`、`2c57893`、`6154078`
**来源**：`changelog.md` / GRID-DIAG-GROUND-01、GRID-RW-2A、GRID-RW-2B。

### 已确认事实

- 旧 Reference Grid 以世界空间 LineList 与 MapGround 的深度承载关系工作，并使用 Depth Test 与负 Depth Bias；
- Ground 隔离实验改变了旧 Grid 的表现，而 World Axis 在同一条件下连续缩放稳定；
- RW-2A 将 Grid 改为独立 Fullscreen World XY（Z=0）层、关闭 DepthTest/DepthWrite 后，真机核心目标通过；
- RW-2B 的 CPU 全帧统一 Step、1/2/5 序列与 24~80 DIP 回滞已有自动合同和真机通过记录。

### 高置信机制解释（尚未直接 GPU 捕获证明）

旧 Grid 与 Ground 的共面或近共面深度竞争，叠加世界空间 1px LineList 的亚像素覆盖变化，是持续闪烁的主要机制解释。该解释不得写成已经由 GPU Capture 直接证明的事实。

### 教训

连续多个局部修复只能改变症状、不能消灭问题时，禁止继续默认当前架构前提正确。必须回到对象身份、所有权、承载层和共同依赖重新审查。

本事故中，错误前提是把 World Grid 定义为“必须压在 MapGround 上的一批真实世界线”。正确身份是独立 Editor Environment Layer。

### 停止条件

- **STOP-01**：同一问题连续 2 次针对性修复失败，且症状跨多个子系统出现时，停止参数调优，审查共同依赖。
- **STOP-02**：A 与 B 同时闪烁时，先寻找 `A/B → Shared Dependency`，不得先分别给 A、B 叠加 workaround。
- **STOP-03**：一个编辑器辅助功能同时依赖 World Z Offset、Clip Bias、Depth Bias、特殊 Draw Order 或特殊 Ground 配合才能稳定时，必须重新判断其是否应属于独立 Editor Overlay / Environment Layer。

### 禁止做法

- 把第三次以上的 LOD、Fade、Bias 或线宽调整称为根因修复；
- 在没有身份审查前继续叠加多个视觉补偿；
- 将高置信机制解释伪装成已直接捕获的 GPU 事实。

### 正确做法

1. 列出共享状态、共享平面、共享 Pass、共享 Depth 与共享输入链；
2. 用一次只排除一个变量的实验检验共同依赖；
3. 重新定义对象属于 Domain、Map Surface、Editor Overlay 还是 Editor Environment；
4. 将最终规则写为 Knowledge，并以测试/Gate 固化可机判边界。

### 验证与 Gate

- `ReferenceGridShaderContractTests` 锁定 World XY、CPU Step、无 Fragment LOD、无 Ground Bias；
- `WorldGridIndependenceContractTests` 锁定 MapGround 恢复与 World Grid 的 Z=0 独立性；
- F1 FINAL 真机验收覆盖 Ground ON/OFF、缩放、Region、Navigation、Resize 与 Picking。

**关联 Incident**：INC-2026-08-10-006
**关联 Knowledge**：K-REN-001、K-REN-002、K-REN-004
