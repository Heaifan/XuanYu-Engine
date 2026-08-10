# MAP-A-R3-D2-F1-CLOSEOUT

**状态**：OPEN · FINAL ACCEPTANCE FAILED · 5 ITEMS REMAIN
**基线**：`feat/MAP-A-R3` / `d7a7adf`
**自动门禁**：2026-08-11 00:03:14（UTC+08:00）完整 Build 0W0E、Core 335/335、World 1115/1115、WarCore 22/22、ARCH-A、5+100、SPIR-V 与 diff-check PASS
**关闭条件**：下列 F1-M01～F1-M15 全部由用户真机确认 PASS。

## 真机 IPO 验收

| 编号 | 路径 | 输入 | 过程 | 预期输出 | 结果 |
|---|---|---|---|---|---|
| F1-M01 | Map View | Ground ON | 打开地图 | World Grid 正常显示 | PASS |
| F1-M02 | Map View | Ground OFF | 关闭 Ground | Grid 仍独立显示 | PASS |
| F1-M03 | Map View | 滚轮 | 连续放大/缩小 | Grid 不闪、不消失 | FAIL |
| F1-M04 | Map View | 滚轮 | 拉远再拉近 | 100→200→500→1k 整体减密、无抖档 | FAIL |
| F1-M05 | Map View | 滚轮 | 连续缩放 | World Axis 稳定 | FAIL |
| F1-M06 | Region Tool | 三点/多点 | 绘制、预览、闭合 | Region 正常 | FAIL |
| F1-M07 | Region Tool | 视角切换 | 俯视、45°、低角度 | Region 不闪 | PASS |
| F1-M08 | Region Tool | Gizmo 点击/拖动 | 保持工具激活 | Gizmo 仍可操作 | PASS |
| F1-M09 | Map View | 中键/Shift+中键/滚轮 | 环绕、平移、Dolly | 相机正常 | PASS |
| F1-M10 | Region Tool | 结束导航 | 松开中键后移动鼠标 | Draft Preview 自动恢复 | PASS |
| F1-M11 | Map View | 滚轮 | 改变尺度 | Scale Indicator 更新真实尺度 | PASS |
| F1-M12 | Map View | Resize | 调整窗口大小 | Grid/Region/Scale/Gizmo 不消失、不漂移 | PASS |
| F1-M13 | Region Tool | 点击 | 落点与顶点比对 | 坐标一致 | PASS |
| F1-M14 | Region Tool | 单击 | 多次单击 | 无重复 Hit、无多点 | PASS |
| F1-M15 | Editor | 综合操作 | 全流程 | 无 Error/Warning/崩溃/输入卡死 | FAIL（极远 Dolly 触发 VP 不可逆异常） |

## F1-FAR-SAFE-01

- 实机崩溃已定位为 `ViewProjectionState.Create` 将极远 `Vector3d` 相机位置降为 `Vector3` 后，`eye` 与 `eye + forward` 合并，导致 ViewProjection 不可逆；旧 `F1-FAR-DIAG-01` 既不可见又依赖该 VP，已撤销。
- `ViewportMetricScale.TryCreate` 改为真正失败安全；渲染投影无法构造时返回失败快照，Vulkan 清理该帧投影而不让异常穿透 UI。
- Dolly 候选相机在 VP 构建前以纯 double 几何计算中心射线和 X/Y Metric，并在 10km、100km、1000km 等跨档时写入编辑器日志；不改动 Camera 上限、FarPlane、Fullscreen Grid、Depth、Ground、Step 或 Region。
- 当前验收裁定为 10/15 PASS；本轮目标是保证极远 Dolly 不崩溃且可读地收集 M03～M05/M15 证据。M06 仍留给独立的 `F1-REGION-CLOSE-01`。

## 范围冻结

- 禁止修改 World Grid Fullscreen 架构、World XY（Z=0）、DepthTest/DepthWrite 关闭、Ground 独立性；
- 禁止修改全帧 Step、1/2/5、24~80 DIP 回滞、Camera、Region、Picking、Vector Overlay Depth Policy；
- RW-2C/RW-2D 为 `DEFERRED · NON-BLOCKING VISUAL IMPROVEMENT`；
- 未取得 15/15 PASS 前，F1 不得 CLOSED，D3 不得启动。

## 自动回归结论

F1 的自动门禁已全部通过；该结论不替代上表 15 项真机 IPO。当前唯一关闭阻塞是用户对 F1-M01～F1-M15 的逐项 FINAL 裁定。
