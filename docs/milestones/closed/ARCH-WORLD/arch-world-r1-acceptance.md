# ARCH-WORLD-R1 真机验收报告

- 版本：`v0.2.19.2-rz`（维持，不升版）
- 状态：**✅ CLOSED**
- 日期：2026-07-24
- 验收方式：项目负责人逐张截图 + 运行日志人工核对（`run.bat` 启动编辑器）
- 环境：Windows + RTX 3060 + Vulkan + Avalonia 编辑器
- 配套图：`arch-world-r1-acceptance.svg`（验收证据图）

> R1 为纯归属重构（建立 `XuanYu.World` 物理程序集边界），**运行行为不变**。本验收只验证"跨程序集迁移后核心风险链路是否断链"，不验证新功能。R2 起才做行为收敛。

## 一、验收范围与核心风险

R1 把 `GlobalWorld / Region / Scene / Spatial 索引 / Transform` 从 `XuanYu.Core` 迁到新程序集 `XuanYu.World`，最危险的风险是：

- 程序集未加载 / `TypeLoadException` / `MissingMethodException`
- 构造路径断裂导致启动崩溃
- Editor 世界投影绑定断链（Hierarchy / Project 树空）
- Picking / Selection / Move Gizmo / Undo-Redo 跨 Core-World-Editor 过渡边界失效
- Vulkan 生命周期（Resize / Swapchain / 关闭）被迁移连带破坏

## 二、真机证据链（逐项 PASS）

### 1. 启动 / 程序集加载 / Vulkan 主链 — PASS
`v0.2.19.2-rz` 正常启动，RTX 3060 被正确选中；Instance → Surface → Device → Swapchain → Framebuffer → Pipeline → Present 全链建立成功；首帧从启动 `16×16` 自动恢复到真实 `714×639`。10 个实体持续正常渲染，说明 World → Snapshot → Vulkan 链路未断。基本排除程序集缺失 / 构造路径断裂类风险。

### 2. Project / Hierarchy 世界投影 — PASS
Project 树与 Hierarchy 树均正常存在并可切换；世界根节点 → 主相机 / 地面 / 区域 0,0,0（多实体）/ 区域 1,0,0（实体 05/10）层级完整。说明 R1 迁移后 Editor 的世界投影绑定未断。

### 3. Hierarchy Selection + Viewport Picking — PASS
日志出现两条独立入口：`来源=层级树；键=EntityId(7)` 与 `来源=视口；键=EntityId(1/3)`，其后均有 `选择投影同步 → PublishSceneRenderSnapshot → 选择提交完成`。两条路径都能进入同一选择投影链，证明 Picking/Selection 跨过渡边界仍工作。

### 4. Inspector 与实体状态 — PASS
选中实体后 Inspector 正确显示实体编号 / 路径 / 区域 0,0,0 / 活动状态 / 全局位置 / Transform，证明 `EntityId → World Entity State → Scene Projection → Editor Inspector` 未因程序集拆分断链；Region 信息仍可被 UI 正常读取。

### 5. Move Gizmo Preview → Commit — PASS
日志明确出现 `当前工具切换为：移动 → 移动工具会话开始 → 变换捕获开始 → … → 编辑历史已记录 → 变换捕获提交 → 提交捕获 → 移动工具会话结束`，且画面实体布局实际变化。最危险的过渡债务 D1（`Editor Gizmo → TransformSession → SceneStateOwner → World → Render Snapshot`）当前完整工作；R1 搬家未破坏运行行为。

### 6. Undo / Redo — PASS
日志直接出现 `撤销已执行` / `重做已执行`，执行后视口持续正常绘制、选择链未崩；`Commit → History → Undo → Redo → World State → Snapshot → Render` 链路通过。

### 7. Resize / Swapchain 多代际重建 — PASS
实际经历 `16×16 → 714×639 → 714×274 → 1234×442 → 714×274`，Swapchain 代际 `0 → 1 → 2 → 3 → 4`，每次 `Present Out-of-date → 自愈查询 → Swapchain 重建 → Framebuffer 重建 → CommandBuffer 重录 → 恢复 Present`，后续 Resize 回调正确跳过重复重建。明确判定：**R1 未破坏 Vulkan 生命周期**。

### 8. 关闭生命周期 — PASS
结尾干净：`呈现泵已停止 → 图形 Pipeline 资源释放完成 → RenderPass + Framebuffer 释放成功 → Swapchain 释放成功 → 逻辑设备释放成功 → Surface 释放 → Instance 销毁 → 分离完成`；无旧 Session 复活、无关闭崩溃、无明显释放顺序异常。

## 三、观察项（非阻断，不判 bug，不阻断 R1 收口）

### O1. Camera Inspector 占位数据（非阻断，另登记 Editor/Inspector 小债务）
截图选中"主相机"时 Inspector 顶部显示"主相机 / 相机"，但"基础信息"仍显示 `名称：玄域示例项目 / 类型：项目 / 路径：玄域示例项目`，疑似 Camera Inspector 复用项目占位数据。性质上不像 World 程序集拆分造成的核心错误，更像既有 Inspector 数据投影未完整实现。**不阻断 R1 收口**，登记为独立 Editor/Inspector 小债务，不夹入 R2。

### O2. Preview 高频日志（不判 bug）
`14:15:55` 附近大量 `PublishSceneRenderSnapshot / CommandBuffer录制` 连续出现，结合当时处于移动 Gizmo 操作，疑似 Preview 高频更新。无错误、无卡死、最终正常 Commit、Undo/Redo 正常，暂不判性能问题；未来诊断日志治理时可避免 Preview 高频路径刷屏。

### O3. Frame All / Frame Selected 无独立日志证据（不伪称）
提供证据中未见到明确的 `Frame All` / `Frame Selected` 日志标记；截图相机/视口尺度变化不能可靠归因（同时发生 Resize）。**不伪称"有日志证明"**。该两项非 R1 程序集拆分的阻断项，且自动测试已覆盖 Camera Framing 主链，真机未观察到相机/视口回归。

## 四、受控债务（保留至原定轮次）

| 编号 | 内容 | 收口轮次 | 红线约束 |
|---|---|---|---|
| D1 | `TransformSession` 暂居 World 含 Gizmo/Editor 语义 | R4 Editor 剥离 | 禁止新增 World→`Core.Gizmo` 依赖 |
| D2 | `SceneRenderSnapshot` 含 EditorCamera/Gizmo/Selection 污染 Core | R5 Snapshot 边界 | — |
| D3 | `Core.Tests`/`World.Tests` 跨层测试依赖 | R4/R5 | 仅 Test 程序集，不破运行时 |

（详见 `docs/arch-world-debts.md`）

## 五、验收裁定

```text
ARCH-WORLD-R1  v0.2.19.2-rz
物理程序集边界             PASS
Core → World 红线          PASS
World → Core 单向依赖      PASS
ARCH-WORLD 自动守卫         PASS
测试 API 后门治理           PASS
9 项目 Build 0W0E          PASS
158 Tests                  PASS
Hierarchy / World Projection PASS
Hierarchy Selection        PASS
Viewport Picking           PASS
Move Preview / Commit      PASS
Undo / Redo                PASS
Resize / Swapchain         PASS
Shutdown Lifecycle         PASS

D1/D2/D3                   保留受控债务
O1 Camera Inspector占位    非阻断，另行登记
O2 Preview 高频日志         非阻断，不判 bug
O3 Frame All/Selected      无独立日志，不伪称

最终裁定：✅ PASS / CLOSED
```

## 六、后续

**ARCH-WORLD-R1 CLOSED**。下一步正式进入 **ARCH-WORLD-R2：单一空间权威**：

- 当前双轨：`GlobalWorld → WorldQuery → SpatialIndexOwner A` 与 `SceneStateOwner → SpatialIndexOwner B`。
- 目标：收敛为 `GlobalWorld → WorldQuery → 唯一 SpatialIndexOwner`，Scene / Picking / 后续 Streaming 共用同一权威查询源。
- R2 不碰 D1 `TransformSession`、不碰 D2 `SceneRenderSnapshot`、不碰 O1 Camera Inspector、不碰 Large World；只做实"同一 EntityId 在世界里只允许一个权威位置答案"。
