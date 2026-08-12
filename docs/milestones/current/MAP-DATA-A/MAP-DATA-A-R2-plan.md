# MAP-DATA-A-R2 · Road Dataset / Polyline

状态：实现完成，等待 R2 真机验收。R1 已按用户裁决 CLOSED。

## 冻结目标

- T1：保留 R1 Closeout 与旧 Region Dataset 兼容；旧 `0.2.0` Region 文件不因读取而被强制改写。
- T2：Dataset `0.3.0` 增加 Road/Polyline 合同：稳定 32 hex ID、`geometry.type=polyline`、2～1024 个有限且不相邻重复的节点、`name/kind` 属性。
- T3：道路工作区完成 Authoring → Render → Save/Reload；正式道路提交只产生一条 Map History 记录。

## 明确不做

Road Graph、寻路、宽度/坡度、Feature Picking、已完成道路节点编辑、XYUI 全面改造及其他非道路功能不属于 R2。

## 兼容与边界

Region/Polygon 保持原有模型和旧版本读取；新建 Dataset 使用 `0.3.0`。Road 复用现有用户数据图层投影，不新增 Vulkan 依赖或跨层 UI 依赖。
