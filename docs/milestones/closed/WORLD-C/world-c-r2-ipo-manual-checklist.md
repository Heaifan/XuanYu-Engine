# WORLD-C-R2 中文 IPO 真机验收卡

状态：`WORLD-C-R2 CLOSED`
真机裁定日期：2026-08-01

| 测试 | 结果 | 验收证据摘要 |
| --- | --- | --- |
| 01 空白场景 | PASS | 层级、检查器和选择为空，无十个测试实体 |
| 02 添加立方体 | PASS | Cube 正常，黄色 Legacy Triangle 残影消失 |
| 03 Transform | PASS | Move、Rotate、Scale 均正常，无几何串线 |
| 04 重命名 | PASS | Focus → SelectAll，可直接覆盖旧名称 |
| 05 删除 | PASS | 删除、选择清理与 Undo/Redo 正常 |
| 06 Dirty 保存点 | PASS | 保存点前后 Clean/Dirty 正确 |
| 07 保存恢复 | PASS | 身份、名称、类型、Transform 与顺序恢复 |
| 08 R1 兼容 | PASS | Legacy 场景可打开并升级保存 |

以下 IPO 条目保留为已执行的验收合同。

## 01 空白场景

- I：启动编辑器或新建场景。
- P：观察视口、层级、检查器和 Gizmo。
- O：0 实体；层级/检查器/选择为空；Gizmo 隐藏；相机斜上方观察原点。

## 02 添加立方体

- I：顶部“场景→添加→基础实体→立方体”，再用层级右键重复添加。
- P：观察层级、视口、工具和未保存状态。
- O：原点出现 Cube；自动选择；Cube 内不得出现黄色旧三角形；工具不切换；名称依次为立方体、立方体 001、立方体 002。

## 03 Transform

- I：依次移动、旋转、缩放。
- P：测试 Preview、Escape、Commit、Ctrl+Z、Ctrl+Y。
- O：Move/Rotate/Scale 下均无黄色旧三角形；实体/轮廓/Picking/Gizmo 同步；Cancel 不入历史；Commit 单条历史；Dirty 正确。

## 04 重命名

- I：使用 F2 和层级右键重命名。
- P：测试 trim、重复名、空名、Enter、Escape 和失焦。
- O：文本框立即获得焦点并全选旧名称，直接输入会整体替换；F2/右键行为一致；合法名提交；重复名补最小后缀；空名不提交；Escape 取消；EntityId 不变。

## 05 删除

- I：使用 Delete 和层级右键删除；另在文本框聚焦时按 Delete。
- P：观察选择、检查器、Gizmo、Undo/Redo。
- O：实体删除后 UI 清空；Ctrl+Z 恢复原 ID/名称/Transform；文本编辑 Delete 不删除实体。

## 06 Dirty 保存点

- I：保存后 Add，再 Undo/Redo；修改后保存，再 Undo。
- P：观察标题、顶部与底部状态。
- O：回保存内容为 Clean；离开保存内容为 Dirty；保存后的 Undo 也为 Dirty。

## 07 保存恢复

- I：完成 Add/Rename/Transform/Delete 后保存并重新打开。
- P：核对内容与身份。
- O：数量、名称、类型、Transform、ParentId、SiblingOrder、EntityId 全部恢复；无十个测试三角形。

## 08 R1 兼容

- I：打开合法 WORLD-C-R1 `.xyscene` 并再次保存。
- P：观察旧三角形与输出 Schema。
- O：旧场景可开；旧实体保持 Legacy Triangle；再保存升级 v2；编辑器不崩溃。
