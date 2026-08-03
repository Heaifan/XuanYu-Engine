# ARCH-C-R8 综合真机验收与收口判断

版本：v0.2.17.31-fix
日期：2026-07-20  
性质：R8 验收清单细化修正；只验不扩。

## 1. 使用方式

本文件不是阶段口号，而是真机操作手册。验收时按顺序执行，每完成一个步骤，把对应日志复制出来。若任一 P0 失败，停止 R8，先定位首因。

每段回传都建议包含：

```text
日志列表复制文本
必要截图
是否出现错误 / 警告
是否出现 Present Stop / Start
是否出现 Swapchain 重建
关闭释放日志
```

## 2. R8-A 自动审计结果

```text
Branch: fix/RZ-VK3-A-surface-contract
Baseline HEAD: 22c03da
dotnet restore: PASS
dotnet build XuanYu.Engine.slnx: PASS, 7 projects, 0 warning, 0 error
dotnet test XuanYu.Engine.slnx: PASS, 78 passed / 0 failed / 0 skipped
scripts/arch-a-guard.ps1: PASS
git diff --check: PASS
5+100: PASS
file-tree: 331 / 331
SVG XML: PASS
```

## 3. R8-B 主链：Picking -> Selection -> X/Y/Z -> Commit -> Undo

目标：证明画面、Picking、Transform 和 Scene 正式事实一致。

### B1. 启动与初始 Picking

操作：

```text
1. 运行 run.bat，确认窗口标题为 v0.2.17.31-fix。
2. 展开底部日志。
3. 点击视口中可见实体三角形 / Gizmo 附近的实体区域。
4. 确认左侧树选中 ARCH-C-R1 Test Entity。
5. 确认右侧 Inspector 显示 EntityId(1)。
```

必须出现的日志：

```text
【ARCH-C-R2-F】视口拾取完成；结果=EntityId(1)
【ARCH-C-R3】选择已提交；结果=EntityId(1)
```

必须检查：

```text
[ ] Picking 逻辑坐标与当前点击画面位置一致。
[ ] 日志详情包含 DPI 当前值。
[ ] 候选 >= 1，精确检测 >= 1，真实命中 >= 1。
[ ] Selection Revision 正常递增。
[ ] 没有 NoHit。
```

失败判定：

```text
点击可见实体却 NoHit = P0
选中实体不是 EntityId(1) = P0
画面点击位置与日志逻辑坐标明显不一致 = P0
```

### B2. X 轴拖动并 Commit

操作：

```text
1. 切换到移动工具。
2. 点中红色 X 轴。
3. 按住拖动一小段。
4. 松开鼠标 Commit。
```

必须出现的日志：

```text
【ARCH-C-R5】移动工具会话开始；轴=X
提交捕获
【ARCH-C-R7】记录编辑历史
【ARCH-C-R5】移动工具会话结束；Axis=X
```

必须检查：

```text
[ ] Position 只有 X 分量变化。
[ ] Y / Z 不漂移。
[ ] 本次 Session 只有一次“记录编辑历史”。
[ ] Preview 计数大于 0。
[ ] 拖动期间没有 Present 泵停止 / 启动。
```

失败判定：

```text
拖 X 导致 Y / Z 改变 = P0
一次 MouseUp 出现多条 History = P0
拖动期间 Present Stop / Start = P0
```

### B3. Y 轴拖动并 Commit

按 B2 同样执行，但点绿色 Y 轴。

必须检查：

```text
[ ] 日志 Axis=Y。
[ ] 只有 Y 分量变化。
[ ] X / Z 保持上一步正式值。
[ ] 只产生一次 History。
```

### B4. Z 轴拖动并 Commit

按 B2 同样执行，但点蓝色 Z 轴。

必须检查：

```text
[ ] 日志 Axis=Z。
[ ] 只有 Z 分量变化。
[ ] X / Y 保持上一步正式值。
[ ] 只产生一次 History。
```

### B5. 连续三次 Undo

操作：

```text
1. 在完成 X / Y / Z 三次 Commit 后，连续按 Ctrl+Z 三次。
2. 如果日志折叠，允许看到“执行撤销 重复 3 次”。
```

必须出现的日志：

```text
【ARCH-C-R7】执行撤销
Remaining=2
Remaining=1
Remaining=0
```

若 UI 折叠显示，接受：

```text
【ARCH-C-R7】执行撤销  重复 3 次
```

但必须能在复制日志或详情中确认三次撤销确实发生。

必须检查：

```text
[ ] Undo 按 Z -> Y -> X 的 LIFO 顺序恢复。
[ ] 每次 Undo 后画面立即同步。
[ ] Undo 不新增 History。
[ ] Undo 后再次点击实体仍能 Picking 命中。
```

失败判定：

```text
Undo 顺序不是后进先出 = P0
Undo 后画面与 Scene Position 不一致 = P0
Undo 产生新 History = P0
```

## 4. R8-C Cancel / History / Session 交叉验收

目标：证明 Cancel、迟到输入、Captured Ctrl+Z 不污染 History / Scene。

### C1. Escape Cancel 后 Undo

准备：

```text
1. 先完成一次合法 Commit，形成 A -> B。
2. 记录此时 History Count。
```

操作：

```text
1. Begin 一次新的轴拖动。
2. 拖出 Preview，但不要 MouseUp。
3. 按 Escape。
4. 再按 Ctrl+Z。
```

必须检查：

```text
[ ] Escape 后出现 Cancel 日志。
[ ] Escape 不出现“记录编辑历史”。
[ ] Scene 回到 Begin 前的 B。
[ ] Ctrl+Z 直接撤销 B -> A。
```

失败判定：

```text
Escape 产生 History = P0
Ctrl+Z 先撤销 Cancel = P0
Escape 后 Preview 残留在画面 = P0
```

### C2. WM_CANCELMODE 后 Undo

操作建议：

```text
1. Begin 一次拖动并产生 Preview。
2. 通过让窗口失焦 / 系统取消捕获触发 WM_CANCELMODE。
3. 回到窗口后按 Ctrl+Z。
```

必须检查：

```text
[ ] 出现 WM_CANCELMODE / Cancel 相关日志。
[ ] 没有 Commit。
[ ] 没有新 History。
[ ] Ctrl+Z 撤销的是最后一次合法 Commit。
```

失败判定：

```text
WM_CANCELMODE 产生 Commit = P0
WM_CANCELMODE 产生 History = P0
```

### C3. Cancel 后迟到 MouseUp

操作：

```text
1. Begin Session=N。
2. 拖出 Preview。
3. Escape 或 WM_CANCELMODE 取消。
4. 观察后续是否出现 Session=N 的提交。
5. 再开始一次新拖动，确认新 Session 正常。
```

必须检查：

```text
[ ] 旧 Session=N 永久失效。
[ ] Cancel 后没有迟到提交。
[ ] Cancel 后没有迟到 History。
[ ] 新 Session 可以正常 Begin / Commit / End。
```

失败判定：

```text
旧 Session 在 Cancel 后提交 = P0
旧 Session 在 Cancel 后写 History = P0
```

### C4. Captured 状态 Ctrl+Z

操作：

```text
1. 先做一次合法 Commit，形成 A -> B。
2. Begin 新拖动，产生 Preview C。
3. 保持鼠标捕获状态，不 MouseUp。
4. 按 Ctrl+Z。
5. 再按 Escape 取消当前 Preview。
```

必须检查：

```text
[ ] Ctrl+Z 在 Captured 状态不执行“执行撤销”。
[ ] Scene 正式状态仍是 B。
[ ] 当前 Session 不进入双事实状态。
[ ] Escape 后 Preview C 消失，Scene 仍为 B。
[ ] 后续再按 Ctrl+Z，才允许撤销 B -> A。
```

失败判定：

```text
Captured 状态 Ctrl+Z 把 B 撤回 A = P0
Session 仍以 B 为 StartSnapshot 但 Scene 已变 A = P0
Escape 不能取消当前 Preview = P0
```

## 5. R8-D Resize / DPI / Vulkan 组合验收

目标：证明 Resize 后空间事实、Picking、Transform 和 Vulkan 生命周期仍一致。

### D1. Commit -> Resize -> Undo

操作：

```text
1. 做一次合法拖动 Commit，形成 A -> B。
2. 改变窗口大小，等待 Resize / Swapchain 日志稳定。
3. 按 Ctrl+Z。
```

必须检查：

```text
[ ] Resize 只在真实物理尺寸变化时重建 Swapchain。
[ ] Undo 正确 B -> A。
[ ] Undo 不触发第二次错误 Swapchain 重建。
[ ] Undo 不触发无意义 Present Stop / Start。
[ ] 画面与 Scene Position 一致。
```

失败判定：

```text
Undo 导致错误 Swapchain 重建 = P0
Undo 后画面与 Scene 不一致 = P0
```

### D2. Resize 后再次 Picking

操作：

```text
1. Resize 后点击实体。
2. 复制 Picking 日志详情。
```

必须检查：

```text
[ ] 日志中的逻辑尺寸为 Resize 后尺寸。
[ ] 物理尺寸与 DPI 关系正确。
[ ] Picking 结果仍为 EntityId(1)。
[ ] 没有使用旧尺寸缓存导致坐标偏移。
```

失败判定：

```text
Resize 后点击可见实体 NoHit = P0
Resize 后 Picking 坐标明显偏移 = P0
```

### D3. Resize 后再次 Transform

操作：

```text
1. Resize 后分别短拖 X / Y / Z 任意一轴。
2. 至少完成一次 Commit。
```

必须检查：

```text
[ ] 轴命中身份正确。
[ ] Transform 数学使用 Resize 后视口。
[ ] 只改变目标轴。
[ ] Commit / History 仍只发生一次。
```

失败判定：

```text
Resize 后点 X/Y/Z 轴身份错乱 = P0
Resize 后 Transform 单轴漂移 = P0
```

### D4. 当前 DPI 链确认

从日志详情记录：

```text
逻辑宽高
DPI
NativeHost 物理宽高
Vulkan Swapchain extent
```

必须检查：

```text
[ ] 逻辑尺寸 x DPI 与物理尺寸相符，允许整数取整差异。
[ ] Vulkan extent 与 NativeHost 物理尺寸一致。
[ ] 只验证当前 DPI，不写成动态 DPI 切换通过。
```

## 6. R8-E 连续编辑工作流与正常关闭

目标：模拟真实工作流，不只做单点测试。

### E1. 连续操作脚本

按顺序执行：

```text
1. 启动编辑器。
2. Picking 选中实体。
3. X 轴拖动 Commit。
4. Ctrl+Z 撤销。
5. Y 轴拖动 Commit。
6. 开始 Z 轴拖动，按 Escape Cancel。
7. Z 轴重新拖动 Commit。
8. Resize 窗口。
9. Resize 后再次 Picking。
10. Ctrl+Z 撤销 Z Commit。
11. 再拖动任意一轴并 Commit。
12. 再 Resize 一次。
13. 正常关闭窗口。
```

必须检查：

```text
[ ] 整段无 Error 日志。
[ ] SessionId 单调推进，旧 Session 不复活。
[ ] History Count 只随合法 Commit 增加，随 Undo 减少。
[ ] Cancel 不增加 History。
[ ] Resize 不恢复旧 Preview。
[ ] Present 泵最终停止。
[ ] Vulkan 资源按顺序释放。
```

### E2. 关闭释放检查

关闭日志必须满足：

```text
[ ] Present 泵已停止。
[ ] Render / Frame / Swapchain 资源释放完成。
[ ] Device 释放在 Swapchain 之后。
[ ] Surface / Instance 最后释放。
[ ] 无 AccessViolation。
[ ] 无后台线程晚到调用。
[ ] 无旧 Present 访问已释放资源。
[ ] 无 double free。
[ ] 无未捕获异常。
```

失败判定：

```text
关闭崩溃 = P0
后台线程晚到访问已释放资源 = P0
Present 继续访问已释放 Swapchain / Device = P0
```

## 7. 一次回传应包含什么

最小可审计回传：

```text
1. 日志复制文本，覆盖本轮操作从开始到结束。
2. 若日志折叠，包含折叠行详情。
3. 至少一张 Resize 后的界面截图。
4. 最终关闭释放日志。
5. 若你认为某项失败，标出大概时间点。
```

## 8. P0 阻断项

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

## 9. 禁区

R8 期间禁止新增：

```text
Redo / Rotate / Scale / Snapping / Local Transform / 多选 / History UI
资产系统 / 保存系统 / ECS 扩展 / 地平面 / 世界原点 / 世界坐标轴 / 天空盒
新渲染效果 / Gizmo 外观优化
```

## 10. 当前结论

```text
R8-A 自动审计：通过
R8-B~E 真机综合验收：等待用户回传
ARCH-C 最终收口：暂未判定
```
