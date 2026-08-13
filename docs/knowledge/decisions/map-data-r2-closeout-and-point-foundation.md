# MAP-DATA-A-R2 收口与 Point Foundation 决策

## 决策

MAP-DATA-A-R2 在 Region（Polygon）与 Road（Polyline）均完成真机验证后正式关闭。Topology Weld 不属于几何编辑基础设施的必要收口项，移入 Future/Backlog；下一轮用 Map Marker 验证 Point Consumer。

## 依据

F3-E M01～M10 全部 PASS，且 `6a3d5b8` 已完成通用编辑生命周期、局部候选、Vertex/Segment Snap、Map-level History 与 Dataset Save/Reload。三种几何类型中尚未有正式 Point Consumer，因此 R3 优先补齐 Point，而不是扩张到拓扑共享或 Gameplay。

## 边界

Point Foundation 不包含城镇、资源、港口、势力、AI、Gameplay、Topology Weld、Shared Node、自动路口、交点、自动切分或节点增删。
