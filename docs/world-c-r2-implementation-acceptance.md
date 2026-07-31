# WORLD-C-R2 实施与验收报告

## 状态

`WAITING FOR REAL-MACHINE RETEST`（等待真机复测）。首次真机验收发现选中 Cube 叠加黄色旧三角形，并要求补全重命名全选体验；已进入 WORLD-C-R2-R2 最小修复，本报告不宣布 WORLD-C-R2 CLOSED。

## R2-R2 真机退回与修复

- 根因：Gizmo Draw 仍把前三个顶点留给旧 Legacy Triangle，GLSL 又在三类 Gizmo 分支前优先消费这三个顶点。
- 修复：最终帧由统一 DrawPlan 生成实体填充、实体轮廓和唯一活动 Gizmo；Legacy/Cube/Move/Rotate/Scale 模式互斥，Move/Rotate/Scale 顶点数为 36/864/252，不再携带旧三角形。
- 重命名：内联文本框实际可见后排队执行 Focus + SelectAll；F2 与层级右键使用同一入口合同。
- 边界：未通过透明、缩小或遮挡掩盖三角形；未加入地面、网格、更多几何、项目管理器、材质或资产系统。

## 实施范围

- 真正空白的生产启动与新建场景；成功替换场景后恢复默认斜上方相机。
- 唯一可添加类型 Cube；LegacyMinimalTriangle 仅用于 v1/R1 文件兼容。
- Add、Delete、Rename、Move、Rotate、Scale 共用同一历史栈，恢复原 EntityId。
- 名称 trim、空名拒绝、场景唯一和最小可用 ` 001` 后缀。
- SceneDocument v2 必写实体类型；v1 缺类型按 Legacy 读取；候选失败保持当前场景。
- Cube 填充、轮廓、Picking、Transform Preview 与 Gizmo 使用同一实体类型和 Transform。
- 顶部与层级右键添加；Delete、F2、右键删除/重命名与内联编辑。

## 架构边界

- 权威链保持 `SceneStateOwner → GlobalWorld → EntityRegistry`。
- `XuanYu.Core` 的 History 容器只保存不透明上层条目，不认识 Cube 或场景业务。
- R2 未引入通用 Mesh、材质、资产路径、组件、Prefab、项目管理器或相机持久化。

## 自动验证记录

- GLSL `scene.vert` 已由 `glslc` 编译，嵌入 `ShaderBytecode.Vert.cs`。
- 初始环境曾被 Avalonia 用户缓存权限、缺失 restore 资产和默认并行内存峰值阻断；授权 restore 后统一改用 `-m:1` 串行门禁。
- 10 项目串行 build：0 warning / 0 error。
- Core Tests：135 passed / 0 failed / 0 skipped。
- World Tests：191 passed / 0 failed / 0 skipped。
- `scripts/arch-a-guard.ps1`：PASS（含架构边界与全仓 5+100）。
- `git diff --check`、R2 SVG XML、仓库 `.xyscene` JSON：PASS。

## 真机入口

按 `docs/world-c-r2-ipo-manual-checklist.md` 重点复测 02、03、04。通过后另行裁定 CLOSED；本轮不创建 Tag/Release。
