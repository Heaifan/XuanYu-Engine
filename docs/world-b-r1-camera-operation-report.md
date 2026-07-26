# WORLD-B-R1 编辑器相机操作实装报告

版本：v0.2.20.2-rz
日期：2026-07-26 13:56:52
分支：feat/WORLD-B-editor-interaction
阶段：WORLD-B-R1：编辑器相机操作闭环

## 裁定

本轮只实现编辑器相机操作，不进入 WarCore、Rotate、Scale、Inspector Transform 或实体 Transform 改造。`ARCH-WORLD-R6` 仍是历史验收记录，当前开发入口保持 WORLD-B。

R1 采用显式唯一观察中心方案：`UiVm.Camera.cs` 持有 `_camera` 与唯一 `_observationCenter`。Frame、Orbit、Pan、Dolly 全部读取和更新同一个观察中心来源，不新增 Camera Service、Camera Entity 或第二套 World 权威。

## 实现变化

- `XuanYu.Editor.Camera` 新增 `CameraFrameResult` 与 `CameraNavigation` 纯算法，负责 Orbit / Pan / Dolly 的数学不变量。
- `EditorCameraFraming` 新增 `FrameAllWithCenter` / `FrameSelectedWithCenter`，保留旧 API 并返回兼容 `CameraState`。
- `UiVm.Camera` 在 Frame All / Frame Selected 时同步更新 `_observationCenter`，Resize 期间若存在相机会话则统一 Cancel。
- `UiVm.CameraNavigation` 新增最小 CameraSession：Begin / Preview / End / Cancel；Cancel 恢复完整起始 `CameraState` 与观察中心，不写 History。
- 视口输入新增 MMB Orbit、Shift+MMB Pan、Wheel Dolly；Win32 NativeHost 与 Avalonia Pointer 两条路径都接入同一 UiVm 相机入口。
- CameraSession 活动期间拒绝 Picking 与 Move Gizmo Begin；Gizmo Capture 活动期间拒绝 Camera Begin 与 Dolly。

## 明确未做

- 未修改 `GlobalWorld`、`EntityRegistry`、实体 Transform、`TransformSession` 或 `EditorHistoryOwner`。
- 未实现 Rotate、Scale、Local、Inspector Transform、WarCore、Soldier、Faction、Organization。
- 未引入新依赖、公共 Camera Service、正交相机、多相机系统或相机书签。

## 自动验证

本轮新增 11 个自动测试，基线 171 增至 182：

- `CameraNavigationTests`：Orbit 保距/保中心/Forward 指向中心、极点保护、Pan 平移不变量、Dolly 比例缩放与非法输入保护。
- `WorldCameraNavigationUiTests`：Frame 更新观察中心、Cancel 恢复起始相机、Gizmo Capture 抢占、旧 PointerUp 不影响新会话、Camera Capture 阻止 Dolly 与 Picking。

当前已完成的完整自动验证：

```text
dotnet build .\XuanYu.Engine.slnx --no-incremental
结果：10 项目，0 Warning / 0 Error

dotnet test .\XuanYu.Engine.slnx --no-build
结果：182 passed / 0 failed / 0 skipped（Core 77 + World 105）

scripts\arch-a-guard.ps1
结果：PASS

5+100
结果：PASS

SVG XML
结果：52/52 PASS

git diff --check
结果：PASS

版本一致性
结果：run.bat / UiWin.axaml / changelog.md / file-tree.md 均为 v0.2.20.2-rz
```

## 真机验收状态

自动测试已覆盖核心数学和会话生命周期，但 R1 尚未宣布真机 CLOSED。仍需用户运行 `run.bat` 执行：

- Frame Selected 后 MMB Orbit 围绕选中实体。
- Frame All 后 Orbit 围绕场景中心。
- Shift+MMB Pan 沿屏幕平面平移。
- Wheel Dolly 不穿越焦点且不改 FOV。
- Escape / LostCapture / WM_CANCELMODE / 窗口失焦恢复起始相机。
- Camera 与 Gizmo 不同时 Preview。
- Resize 后焦点、位置、距离稳定；操作中 Resize 明确 Cancel。

## 下一步

真机验收通过后补 `docs(editor): close WORLD-B R1 camera acceptance` 收口文档和 SVG，再普通 push。若真机发现代码错误，只修 R1 相机问题，不进入 R2、Rotate、Scale 或 WarCore。
