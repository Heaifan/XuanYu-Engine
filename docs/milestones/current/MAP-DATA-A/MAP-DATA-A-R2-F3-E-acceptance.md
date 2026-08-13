# MAP-DATA-A-R2-F3-E · Generic Geometry Editing & Snap Integration 验收

状态：`USER ACCEPTANCE PASS · CLOSED`。

用户真机验收：M01～M10 全部 PASS。用户确认道路顶点能够稳定贴合区域边界/顶点，跨 Feature 通用吸附链成立；最终验证对象为 `6a3d5b8`。

| 编号 | 路径 | 输入/过程 | 预期输出 |
|---|---|---|---|
| M01 | 区域编辑 → 区域面 | 拖动 Region 顶点 | 自由拖动、Preview/Release/Commit 正常 |
| M02 | 区域编辑 → 区域面 | Region Vertex/Edge Snap | 与既有 8/12px 手感一致，Vertex 优先 |
| M03 | 区域编辑 → 道路 | 拖动 Road 起点/中间点/终点 | Road 自由拖动，开放 Polyline 不闭合 |
| M04 | 区域编辑 → 道路 | Road 顶点靠近 Region Vertex/Segment | 通用 Snap 生效，Source/Target 身份正确 |
| M05 | 区域编辑 → 道路 | Road 顶点靠近另一 Road Vertex/Segment | 通用 Snap 生效，不吸自身 Feature |
| M06 | 区域编辑 → 道路 | 沿 Segment 移动后靠近 Vertex | Segment 锁定与 Vertex 升级稳定，不抖动 |
| M07 | 区域编辑 → 区域面/道路 | Esc、Undo、Redo | 取消不写 Dataset；一次拖动一条 History；回退/重做正确 |
| M08 | 区域编辑 → 区域面 ↔ 道路 | 连续编辑 Region A、Road A、Road B | 无 stale drag/snap 状态，身份不串写 |
| M09 | 地图保存/重载 | 保存后重新打开 | 几何、Feature/Dataset/Layer 身份保持 |
| M10 | 大量 Region + Road | 反复拖动顶点 | 无明显卡顿；PointerMove 使用局部候选，不退化全量扫描 |

自动证据：Generic focused `24/24`；正式门禁：Solution 0W/0E、Core.Tests 339/339、World.Tests 1365/1365、WarCore.Tests 22/22、ARCH-A PASS、AXAML/XML PASS、版本四处一致、`git diff --check` PASS。用户真机 M01～M10 全部 PASS，F3-E 正式 `CLOSED`。
