# WORLD-C-R1 最小场景保存与打开闭环收口

版本：v0.2.21.5-fix
日期：2026-07-31 17:58:02
分支：feat/WORLD-C-scene-authoring
阶段：WORLD-C-R1：最小场景保存与打开闭环
状态：CLOSED

## 裁定

WORLD-C-R1 真机验收通过，仓库收口完成后正式 CLOSED。

用户真机裁定：

- 测试 01：启动进入空白未命名场景，PASS。
- 测试 02：打开 `samples/world-c-r1-ten-triangles.xyscene` 后出现十个实体，PASS。
- 测试 03：保存后顶部与底部明确显示绿色“保存成功”，标题清除未保存标记，PASS。
- 测试 04：修改实体后标题显示“（未保存）”，顶部与底部显示琥珀色“状态：未保存”，保存按钮轻量强调，PASS。
- 测试 05-09：用户裁定均已通过。
- 测试 10-13：不追加真机重复测试，由现有自动门禁覆盖。

## 完成边界

R1 已完成：

- 空白场景启动。
- 新建场景。
- 打开 `.xyscene` 场景。
- 保存与另存为。
- 未保存 Dirty 提示与保存检查点。
- 严格 `.xyscene` JSON 读写。
- 候选场景加载与失败保护。
- 原子保存。
- 保存、另存为、加载失败诊断日志。
- 重启后重新打开保存场景并恢复实体身份、名称、层级和 Transform。

R1 不包含：

- 项目管理器或项目容器。
- 实体完整 CRUD。
- 资产导入。
- Prefab。
- 通用组件序列化。
- 环境系统。
- Tag 或 Release。

## 修复轮记录

R1 主实现完成后，真机验收暴露两轮问题：

- R1-R1：打开 sample 失败且日志缺少 Path / Stage / Code / Message / Detail。已补场景加载低频诊断、生产入口 sample 回归和失败保护测试，并修复真实 sample 读取链。
- R1 保存反馈优化：保存与另存为成功反馈不够明显，Dirty 状态不够可见。已补顶部与底部文档状态投影、Dirty 中文标题、保存按钮轻量强调和保存成功/失败日志。

## 自动门禁

收口前已完成：

- 定向 `WorldCSceneDocumentTests`：10 passed。
- `XuanYu.Core.Tests`：129 passed。
- `XuanYu.World.Tests`：173 passed。
- `dotnet build XuanYu.Engine.slnx` 正常输出目录：10 项目，0 warning，0 error。
- `scripts\arch-a-guard.ps1`：PASS。
- `git diff --check`：PASS。
- `.xyscene` 严格 JSON：PASS。
- SVG XML：PASS。
- 5+100：PASS。

## 样例文件处理

真机验收期间直接保存了 `samples/world-c-r1-ten-triangles.xyscene`，diff 显示 Entity 1 的 X 坐标移动到 `0.12997666153893483`，同时文件被重新格式化并转义中文。

该变化裁定为真机测试污染，已恢复仓库基线，不制造无意义样例提交。

## 下一阶段

R1 关闭后，下一阶段进入 WORLD-C-R2：最小项目管理器 / 项目容器。

R2 不在本收口提交中实现。
