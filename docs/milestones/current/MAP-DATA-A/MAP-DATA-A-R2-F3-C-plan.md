# MAP-DATA-A-R2-F3-C · Region Vertex-to-Edge Snap

状态：`OPEN`；F3-B 已 CLOSED；F3-C1 已 CLOSED，C2/C3 未启动。

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

状态：`NOT STARTED`。C1 正式门禁通过后才允许接入 F2 Preview → Commit 管线，并明确 Free、VertexSnapped、EdgeSnapped 状态。

## C3 · Formal Gate + User Acceptance

状态：`NOT STARTED`。C2 完成后才建立正式门禁与 8 项真机 IPO；C3 未开始前不得启动 Road Snap 或 Topology Weld。

## 明确不做

Edge → Edge、整条边贴边、自动增加或分割顶点、Road Snap、Polygon Boolean、Shared Vertex、Shared Edge、Topology Weld、强制消灭地图缝隙。
