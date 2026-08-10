# MAP-A-R3 Backlog

R2 已关闭。本文件登记 R3 当前裁定与候选方向；每轮先冻结目标和范围。

## 当前裁定

- D1：CLOSED。
- D2：OPEN。S01、S02 已由既有真机证据证明 PASS；A03～A06 BLOCKED。
- D2-F1-C2：CLOSED。RF-M01、RF-M02-A、RF-M03 PASS；RF-M02-B 转交 F1-V。
- D2-F1-V1：OPEN。本轮只处理 Region/Draft Vector Overlay；F1-V2、F1-V3 暂不启动。
- F1-V2：BLOCKED BY V1；F1-V3：BLOCKED BY V2。
- D3：禁止启动；F2：不创建。

## MAP-A-R3-D2-F1-C2：正式收口

### 自动与代码结果

- 地图模式“查看全部”按 MapBounds 构图；“聚焦”按 Draft 顶点 AABB → Selected Entity → 相机不变执行。
- Native 中键路由已由 `NativePointerRoutePolicy` 统一裁决：环绕、Shift+中键平移、滚轮缩放与 Draft Preview 互斥且可恢复。
- C2-R01～C2-R09、Native Route Policy 与 F1-C 稳定性/日志回归已补齐。
- RF-M02-B 的巨大线宽/控制点不再归入 C2，而归入 F1-V Vector Overlay。

### 真机 IPO 裁定

- RF-M01：中键环绕、Shift+中键平移、滚轮缩放均 PASS。
- RF-M02-A：Draft Framing PASS。
- RF-M02-B：转交 F1-V，不作为 C2 未完成项。
- RF-M03：导航结束释放中键后，普通移动鼠标可自动恢复 Draft Preview；无需重新选工具、无输入丢失、无崩溃，PASS。
- 正式记录：[R3-C2-closure.md](R3-C2-closure.md)。

## MAP-A-R3-D2-F1-V1：Region Vector Overlay

### 冻结范围

- `RenderVectorOverlayResource` 是 Region/Draft 唯一渲染数据合同。
- Fill 使用世界坐标三角形；Stroke 使用 Vulkan shader 屏幕空间展开；Marker 使用屏幕空间 DIP 半径。
- Draft Preview 线宽 2 DIP；正式 Region Stroke 1.5 DIP；普通点 5.5 DIP；首点 6.5 DIP；Close Candidate 8.5 DIP。
- Region/Draft 不再创建或走 `RenderStaticModelResource` / `DrawRegionModel` 正式路径。
- V2 公制网格、V3 比例尺、Inspector、History、Layer、持久化、DGD、地形和全引擎性能重构均不在本轮。

### TODO 状态

| TODO | 内容 | 状态 |
| --- | --- | --- |
| V1-T01 | 建立 MapVector Overlay 数据合同 | DONE |
| V1-T02 | 建立 Vulkan Vector Pass | DONE |
| V1-T03 | Draft Point 迁移 | DONE |
| V1-T04 | Draft Stroke 迁移 | DONE |
| V1-T05 | Formal Region Stroke 迁移 | DONE |
| V1-T06 | Formal Region Fill 迁移 | DONE |
| V1-T07 | Ear Clipping 凹多边形三角化 | DONE |
| V1-T08 | Dynamic Buffer 复用 | DONE |
| V1-T09 | PointerMove latest-state-wins | DONE |
| V1-T10 | 删除 Region StaticModel 正式路径 | DONE |
| V1-T11 | 自动专项门禁 | DONE |
| V1-T12 | 完整 0W0E 门禁 | DONE |
| V1-T13 | Commit + Push | PENDING |
| V1-T14 | 真机验收 | PENDING |

### V1 自动验收

- 已覆盖 V1-R01～R11：首点 Marker、双点 Stroke、Cursor 更新、正式 Region Fill/Closed Stroke、凹多边形三角化、屏幕空间宽度/半径、无 StaticModel 区域路径、容量足够时缓冲复用与 latest-state-wins。
- 自动门禁已通过；F1-V1 仍等待 V1-M01～M05 真机验收，不提前宣布完成。

## 后续解锁顺序

`F1-V1 PASS → F1-V2 1m Metric Adaptive Grid → F1-V3 Scale Indicator → F1 Final → A03～A06 → D2 Closeout`
