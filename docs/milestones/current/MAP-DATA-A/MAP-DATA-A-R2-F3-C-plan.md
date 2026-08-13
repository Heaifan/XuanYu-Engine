# MAP-DATA-A-R2-F3-C · Region Vertex-to-Edge Snap

状态：`OPEN`；F3-B 已 CLOSED；F3-C1 已 CLOSED；F3-C2 已 CLOSED，F3-C3 为 `NEXT`。

## 冻结目标

- 仅实现 Region Vertex → Other Region Edge；进入 8 px，释放 12 px。
- Vertex Snap 优先于 Edge Snap；Edge Endpoint 不以 Edge 结果抢占 Vertex 目标。
- 排除 Source Region；保持两个 Region 为独立 JSON 几何，不引入 Shared Vertex、Shared Edge 或 Topology Weld。
- 使用 F3-A 的局部候选 Region；本阶段纯算法不接 UI、不接拖拽管线、不新增 EdgeSpatialIndex。

## C1 · Edge Snap Geometry

状态：`CLOSED`（自动门禁）；F3-C1 无 UI 入口，不设置真机 IPO。

纯算法范围：Point → Segment Closest Point、距离计算、Segment Interior 判断、稳定 RegionId/EdgeIndex 决胜、Vertex 优先级、自身 Region 排除、零长度边安全处理。

自动测试覆盖：水平边、垂直边、斜边、Segment 外侧 Clamp、Endpoint Vertex 优先、自身排除、多 Edge 稳定决胜、零长度 Edge。C1 专项 10/10；Core 339/339；World 1325/1325；WarCore 22/22；Solution 0 Warning/0 Error；ARCH-A、版本四处、43 个 AXAML XML 与 `git diff --check` 均 PASS。

## C2 · Drag Pipeline Integration

状态：`CLOSED`（自动门禁）；F3-C2 无新增视觉反馈，沿用 F2 Preview → Release → Dataset Commit 链。

接线：PointerMove 使用 F3-A 12px 局部 Region 查询；候选先由 C1 Resolver 执行 Vertex 优先，再执行 Edge；Edge 锁定 `TargetRegionId + TargetSegmentIndex`，在 12px 内沿同一 Segment 重投影，靠近 Vertex 8px 内允许升级。PointerReleased、Esc、Undo/Redo 均沿用既有几何路径，不新增 Snap History 或 Dataset 写入路径。

自动测试：C2 专项 15/15；Core 339/339；World 1342/1342；WarCore 22/22；覆盖 Free→Vertex、Free→Edge、Segment 重投影、8/12px 迟滞、Edge→Vertex 升级、Vertex 保持、Source Region 排除、12px 查询范围、查询失败清理、状态清理与现有提交路径合同。

## C3 · Formal Gate + User Acceptance

状态：`NEXT`。C3 仅负责综合门禁与真机体验验收准备；不在 C3 偷渡功能开发。Road Snap 与 Topology Weld 仍未启动。

## 明确不做

Edge → Edge、整条边贴边、自动增加或分割顶点、Road Snap、Polygon Boolean、Shared Vertex、Shared Edge、Topology Weld、强制消灭地图缝隙。
