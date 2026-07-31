# WORLD-C-R2 实施与验收报告

## 状态

`WAITING FOR REAL-MACHINE ACCEPTANCE`（等待真机验收）。本报告不宣布 WORLD-C-R2 CLOSED。

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
- Core Tests：130 passed / 0 failed / 0 skipped。
- World Tests：188 passed / 0 failed / 0 skipped。
- `scripts/arch-a-guard.ps1`：PASS（含架构边界与全仓 5+100）。
- `git diff --check`、R2 SVG XML、仓库 `.xyscene` JSON：PASS。

## 真机入口

按 `docs/world-c-r2-ipo-manual-checklist.md` 执行 8 组 IPO 测试。通过后另行裁定 CLOSED；本轮不创建 Tag/Release。
