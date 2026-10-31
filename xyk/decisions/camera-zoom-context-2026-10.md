# DEC-CAM-001｜Editor与Gameplay缩放权威按上下文分离

**状态**：ACTIVE（产品设计批准）｜**裁定**：2026-10-05｜**有效产品基线**：4277b242391e273080c4c19b82c9acf53b348843。
**历史依据**：用户认可的Editor Center Navigation恢复/Scope Supersede；[2026-10-17存档产品知识K-SPA-003/004](https://github.com/Heaifan/XuanYu-Engine/blob/71fc8c365fcdd83166b9d57fdd123fe81316d618/docs/knowledge/architecture.md)。

- Editor Wheel Dolly与Orbit保持ObservationCenter中心，不随鼠标XY执行地表重锚。初始Empty World中心原点；Initial Content Auto Frame和Pan/Explicit Focus/Frame是合法、受控的中心更换事件；平常LOD/更新/Zoom/Orbit不能隐式修改中心。
- Gameplay或显式Screen-Anchored交互可以启用Screen→Surface→Pure Dolly的K-SPA-003技术能力；该能力不被解释为Editor全局政策。
- Editor可为相机净空约束只读查询候选相机点的声明地形面，但不得改变Focus/Pivot/Anchor。起点入地的脱困只由CPU定向回归局部证明，不等于真实P4全场景安全。
- 原先K-SPA-003“Editor全局必须Cursor-Anchored”的扩大解释由K-SPA-004取代，保留K-SPA-003能力及旧事故数据。这是Scope Supersede而不是删除历史知识。

**本次R4只修正XYK迁移的编号/主题遗漏，不修改产品相机代码，也没有重新跑构建或真机验收。**
