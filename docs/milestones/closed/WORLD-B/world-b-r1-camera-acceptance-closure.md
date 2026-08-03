# WORLD-B-R1 编辑器相机操作验收收口

版本：v0.2.20.3-rz
日期：2026-07-26
分支：feat/WORLD-B-editor-interaction
阶段：WORLD-B-R1：编辑器相机操作闭环

## 裁定

WORLD-B-R1 真机证据已通过，R1 正式 CLOSED。

本收口只记录 R1 验收结果，不重做 R1 审计，不进入 WarCore、Rotate、Scale、Inspector Transform 或实体 Transform 改造。下一开发入口保持 WORLD-B-R2：选择与工具状态闭环。

## 验收范围

R1 验收覆盖以下中文 IPO 项：

1. “聚焦”后围绕选中实体环绕；
2. “查看全部”后围绕场景中心环绕；
3. Shift + 鼠标中键平移视图；
4. 滚轮调整观察距离；
5. Escape 取消“环绕”；
6. Escape 取消“平移”；
7. 窗口失焦或鼠标捕获丢失时取消相机会话；
8. 调整窗口大小后保持相机焦点；
9. 相机操作期间不改变选择；
10. 实体“移动”与相机输入互斥；
11. 相机会话中调整窗口大小。

## 结果

```text
中文 IPO 真机验收：PASS
相机焦点合同：PASS
相机 / 选择 / 移动输入互斥：PASS
窗口调整与捕获取消：PASS
R1 状态：CLOSED
```

## 自动证据

R1 实装基线与后续治理补丁已通过：

```text
dotnet build .\XuanYu.Engine.slnx --no-incremental
dotnet test .\XuanYu.Engine.slnx --no-build
scripts\arch-a-guard.ps1
git diff --check
5+100
SVG XML
```

最新已知自动测试数量：182 passed / 0 failed / 0 skipped。

## 禁止项确认

- 未进入 WarCore；
- 未创建 Soldier / Faction / Organization；
- 未实现 Rotate / Scale；
- 未实现 Inspector Transform；
- 未创建新 Camera 系统；
- 未创建新 Selection 系统；
- 未改变 GlobalWorld 权威。

## 下一步

立即进入 WORLD-B-R2：选择与工具状态闭环。
