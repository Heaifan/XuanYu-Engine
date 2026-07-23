# WORLD-A-R2-R4 Editor Camera Framing

版本：v0.2.18.17-rz

## 目标

本轮补齐 WORLD-A 真机验收的可观察性基础设施：启动看全当前调试可见实体、
`聚焦` 选中实体、`查看全部` 当前可见集合。

这不是 WORLD-B 场景建设，也不是完整 Blender 相机系统。

## 分支治理

- 从干净 HEAD `ac75bf0` 创建 `feat/WORLD-A-global-world`。
- 已推送并设置 upstream。
- 旧 `fix/RZ-VK3-A-surface-contract` 保留，不重写、不删除、不 force push。
- 分支治理规则已同步到开发宪法和 `dev-rules.md`。

## 实现边界

- Frame All 只针对当前加载 / 当前调试可见集合。
- Camera Framing 生成正式 `CameraState`。
- Render、Picking、Gizmo 继续消费同一 `ViewProjectionState`。
- 不缩放 Renderer 模型，不修改实体 Transform，不伪造屏幕坐标。

## 验收项

- 启动后当前调试实体可直接进入视野。
- `查看全部` 让当前可见实体集合进入视野。
- `聚焦` 选中 Entity 后完整进入画面。
- 跨 Region、Undo、Redo、Preview Cancel、Resize 后 Camera / Picking / Gizmo 不分叉。

## 当前裁定

`WORLD-A-R2-R4` 是 R2 最终验收前的必要可观察性修正轮。

本轮真机 Gate 已通过：

- 启动后当前调试实体直接进入视野。
- `查看全部` 可以重新 Frame All 当前可见集合。
- `聚焦` 可以 Frame Selected 当前选中 Entity。
- EntityId(2) 跨 Region Commit 到 `Region(1,0,0)` 后 Selection、Inspector、Hierarchy、Render 不丢不串。
- Undo 回 `Region(0,0,0)` / X=1.5，Redo 再回 `Region(1,0,0)` / X=5.9043412667376955。
- Preview -> Escape Cancel 后仍停留在 `Region(0,0,0)` / X=1.5，日志记录“移动工具会话取消，原因=Escape”。
- Maximize / Resize 后 NativeHost 尺寸合并，`查看全部` 与 Undo / Redo 仍通过。

本轮提交与推送完成后，`WORLD-A-R2` 具备最终 CLOSED 裁定条件。
