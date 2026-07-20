# ARCH-C-R8 综合真机验收与收口判断

版本：v0.2.17.30-rz  
日期：2026-07-20  
性质：综合验收轮；只验不扩。

## 1. 阶段目标

R8 不是新增功能开发轮，而是确认 ARCH-C 主链是否可以正式收口。

必须同时成立：

```text
画面看到的位置
= 鼠标 / Picking 理解的位置
= Transform / Gizmo 操作的位置
= Scene 正式记录的位置
```

完整链路：

```text
Picking -> Selection -> Preview -> Commit / Cancel -> Undo -> Resize -> 再交互 -> 正常关闭
```

## 2. R8-A 自动审计结果

```text
Branch: fix/RZ-VK3-A-surface-contract
Baseline HEAD: 22c03da
Worktree before R8: clean
dotnet restore: PASS
dotnet build XuanYu.Engine.slnx: PASS, 7 projects, 0 warning, 0 error
dotnet test XuanYu.Engine.slnx: PASS, 78 passed / 0 failed / 0 skipped
scripts/arch-a-guard.ps1: PASS
git diff --check: PASS
5+100: PASS
file-tree: 331 / 331
SVG XML: PASS
```

说明：首次普通沙箱 `dotnet restore` 因 nuget.org 网络权限失败；已按审批流程提权联网重跑，通过。构建 / 测试使用临时 OutDir，避免正在运行的编辑器锁定默认输出 DLL。

## 3. R8-B 主链真机验收

```text
[ ] 初始 Picking 命中与画面一致
[ ] Selection EntityKey 正确
[ ] 当前 DPI 下坐标换算正确
[ ] X 轴拖动只改 X
[ ] Y 轴拖动只改 Y
[ ] Z 轴拖动只改 Z
[ ] Preview 连续稳定
[ ] MouseUp 最多 Commit 一次
[ ] 连续三次 Commit 后 Undo 按 LIFO 回退
[ ] Undo 后再次 Picking / Transform 仍从正式 Scene 状态开始
```

## 4. R8-C Cancel / History / Session 交叉验收

```text
[ ] Escape Cancel 不产生 History
[ ] WM_CANCELMODE 不产生 Commit / History
[ ] Cancel 后迟到 MouseUp 不复活旧 Session
[ ] Captured 状态 Ctrl+Z 不执行 History Undo
[ ] Captured 状态 Escape 仍可取消当前 Preview
```

Captured Ctrl+Z 是 P0：不得出现 Scene 已 Undo、Session 仍以旧 StartSnapshot 为基线的双事实状态。

## 5. R8-D Resize / DPI / Vulkan 组合验收

```text
[ ] Commit -> Resize -> Undo 正常
[ ] Undo 不触发错误 Swapchain 重建
[ ] Transform / Undo 不触发无意义 Present Stop / Start
[ ] Resize 后 Picking 坐标无偏移
[ ] Resize 后 X/Y/Z Transform 数学仍正确
[ ] 当前 DPI 逻辑尺寸 x DPI = NativeHost 物理尺寸 = Vulkan Viewport 理解
```

当前 DPI 验证只代表当前机器 / 当前缩放环境；不得写成动态 DPI 切换已通过。

## 6. R8-E 持续操作与关闭释放

建议真机顺序：

```text
启动
-> Picking
-> X Commit
-> Undo
-> Y Commit
-> Escape Cancel
-> Z Commit
-> Resize
-> Picking
-> Undo
-> 再拖动
-> Resize
-> 正常关闭
```

关闭必须满足：

```text
[ ] 无 AccessViolation
[ ] 无后台线程晚到调用
[ ] 无旧 Present 访问已释放资源
[ ] 无 double free
[ ] 无未捕获异常
```

## 7. P0 阻断项

任一失败均不得封 ARCH-C：

```text
P0-1 Picking 与画面位置不一致
P0-2 Transform 单轴漂移
P0-3 Commit 重复
P0-4 Cancel 后旧 Session 复活
P0-5 Cancel 产生 History
P0-6 Undo 恢复错误正式状态
P0-7 Captured Ctrl+Z 导致双事实
P0-8 Resize 后 Picking / Transform 空间错位
P0-9 Undo 触发错误 Swapchain / Present 生命周期
P0-10 正常关闭生命周期异常
```

## 8. 禁区

R8 期间禁止新增：

```text
Redo / Rotate / Scale / Snapping / Local Transform / 多选 / History UI
资产系统 / 保存系统 / ECS 扩展 / 地平面 / 世界原点 / 世界坐标轴 / 天空盒
新渲染效果 / Gizmo 外观优化
```

## 9. 当前结论

```text
R8-A 自动审计：通过
R8-B~E 真机综合验收：等待用户回传
ARCH-C 最终收口：暂未判定
```

只有 R8-B~E 全部 P0 通过后，才允许写出：

```text
ARCH-C 正式收口
```
