# MAP-DATA-A-R2-F2 · Geometry Vertex Editing

状态：`CLOSED`；用户已完成 C01～C07 真机验收并全部 PASS；F3 吸附已解除冻结。

## 冻结目标

- F2-T1：Region/Road 已完成几何可通过点击 feature 选择，并显示顶点控制柄。
- F2-T2：顶点拖动采用 Preview → Commit；Esc 取消；一次拖动最多产生一条 Map History。
- F2-T3：Region 执行现有多边形校验，Road 拒绝相邻重复节点；Save/Reload 保持几何和稳定 ID。

## 明确不做

不做吸附、磁性贴合、共享边界、拓扑联动、多选、批量编辑、Road Graph、寻路、宽度/坡度、Schema 变化、Vulkan 重写或 Picking 全面重构。

## 自动证据

覆盖领域单历史、Undo/Redo、非法区域、道路零长度段和屏幕空间 feature/vertex 命中；自动门禁不能替代本文件的 F2 真机 IPO。
