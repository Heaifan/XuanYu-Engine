# Editor / Gameplay Zoom Authority Context Split

**ID:** DEC-CAM-001
**类型：** DEC
**状态：** ACTIVE
**批准日期：** 2026-10-05
**批准方：** USER PRODUCT AUTHORITY + CHATGPT KNOWLEDGE AUDIT
**适用范围：** XuanYu Editor Navigation；Gameplay / 明确 Screen-Anchored Navigation
**Effective Baseline：** `4277b242391e273080c4c19b82c9acf53b348843`
**Version Event：** NONE

## 决策

Zoom Authority 必须按产品上下文区分：

- **Editor：** 当前 `ObservationCenter` 是 Navigation Center。默认 Empty World 中它是 World Origin `(0,0,0)`。Editor Wheel Zoom 与 Orbit 围绕同一当前中心；Orbit Begin 将中心冻结为当前 Session Pivot。
- **Gameplay / 明确 Screen-Anchored Navigation：** 可以使用 K-SPA-003 的 Screen→Surface Anchored Dolly capability。保留 Mouse Cursor Screen XY、SurfaceSource 与有效命中语义。
- **中心变更：** Empty World 初始化设为 World Origin。初次建立内容并执行 Initial Content Establishment Auto Frame 时，`ObservationCenter` 更新为 framing center；Pan、Explicit Focus、Explicit Frame / View command 或其它正式授权的 View Reframe 操作也可更新中心。普通 Zoom、Orbit、Selection、Terrain LOD、Terrain refresh 与普通 Render update 不改变 Navigation Center。

## 2026-10-05 治理澄清：初次内容建立自动取景

原决定将“DEM Import 不得改变 Navigation Center”写得过于绝对，遗漏了已验证产品基线 4277 的 Initial Content Establishment Auto Frame。现明确：普通数据状态变化不得任意修改 Navigation Center；初次内容建立触发的正式 Auto Frame 属于合法 Center Authority Event，生成的 framing center 随后同时供 Editor Zoom 与 Orbit 使用。后续普通 Terrain refresh / LOD / render update 不得隐式重设中心。

本澄清不改变 Editor Wheel 的 `ObservationCenter` policy，不恢复 Cursor-Anchored Editor Zoom，也不删除 K-SPA-003 的 Screen→Surface capability。

## 治理决定

事件 `EDITOR-ZOOM-AUTHORITY-SCOPE-SUPERSEDE-R1` 完成 K-SPA-003 的 scope split：K-SPA-003 中将 Cursor-Anchored Zoom 解释为全局 Editor Wheel policy 的部分由 K-SPA-004 替代；K-SPA-003 的历史事故、Screen→Surface capability、payload 保留、SurfaceSource 与 fallback 规则继续有效。

此决定区分技术能力与产品策略：能力存在不意味着所有产品上下文都必须使用该能力。Editor 不使用 Cursor Anchor 不构成与 Gameplay capability 的冲突。

## 决策依据

- 用户报告基线 4277 已通过 P4：DEM Import、DEM Orbit No-Refresh、DEM Orbit No-Flicker、Grid、ScaleBar、Pan、Navigation Chain 均 PASS。
- 用户明确要求 Editor Zoom / Orbit 共享 Navigation Center：默认 World Origin，Pan / Explicit Focus 后使用新的 `ObservationCenter`。
- 用户报告 `fa55953a`、`d10adb31`、`8fb7e8ae`、`f75badb4` recovery candidates 均 P4 FAIL；它们不作为新产品知识的定义依据。
- 4277 的 P4 结果与候选失败状态作为用户提供的验收证据记录；本 DEC 不声称本轮重新执行了人工 P4。

## 关联知识

- K-SPA-003：Screen→Surface Anchored Dolly capability，保留且限于 Gameplay / 明确 Screen Anchor。
- K-SPA-004：Editor / Gameplay Zoom Authority context-scoped policy。
