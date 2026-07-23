# WORLD-A-R2-R3 Inspector Manual Gate Fix

版本：v0.2.18.16-rz

## 退回原因

`v0.2.18.15-rz` 自动 Gate 通过后补做 `run.bat` 真机验收，发现
Inspector 页只显示名称、类型、路径，未显示 Gate 要求的
`EntityId / Region / Activity / GlobalPosition` 字段。

这不是 World 核心失败，而是 UI 展示没有消费既有 `InspectorFields`。

## 修正范围

- `Right.axaml`：检查器基础信息面板改为绑定 `InspectorFields`。
- `UiVm.WorldProjection.cs`：未修改，继续作为 Inspector 字段唯一来源。
- `GlobalWorld / Partition / History / Gizmo / Vulkan`：未修改。

## 真机证据

- `run.bat` 可启动编辑器窗口，标题为当前开发版本。
- 视口显示多个实体，Hierarchy 显示 Region 分组。
- 选中 `EntityId(2)` 后 Selection / Inspector / Hierarchy 保持同一实体。
- Move Gizmo Commit 后 Entity 可跨 Region 分组迁移。
- Undo / Redo 会同步恢复 Region 分组与 Inspector 路径。
- Preview 状态下按 Escape 可取消，会话取消日志显示 `原因=Escape`。
- Restore / Maximize 后 NativeHost 尺寸合并日志刷新，窗口无黑屏或崩溃。

## 当前裁定

`WORLD-A-R2` 仍不可标记 CLOSED。

本轮只处理真机 Gate 退回缺口；完成验证、提交和推送后，
`WORLD-A-R2-R3` 可作为 Inspector 真机修正轮进入待复验状态。

