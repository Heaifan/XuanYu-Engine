# MAP-A-R3 Backlog

R2 已关闭。本文件登记 R3 候选方向，不代表已批准开发；每轮先冻结目标与范围。

## 当前批准轮次

`MAP-A-R3-D1 — Existing Region Contract Hardening` 已完成：复用 R2 的 MapRegion、MapRegionDraft、MapPoint、Region Layer 与 MapEditSession 历史链路，补齐非相邻边相交、接触、重叠拒绝，以及 Region 正式 Create/Delete 提交入口。

`MAP-A-R3-D2 — Region Drawing` 已完成实现，等待真机验收；本轮不启动 D3。

## MAP-A-R3-D2：Region Drawing

本轮已完成实现并停在真机验收前：区域绘制模式、地图表面拾取、草稿顶点/边/预览、首点闭合候选、Esc 取消、`CreateRegion` 正式提交，以及正式区域与草稿渲染均已接入。L1 静态 UI PASS；L2 Headless PASS；L3 Visual Regression NOT ENABLED；L4 真机验收 PENDING。

真机 IPO 清单：

1. D2-A01 / 进入区域绘制 / 点击“添加→区域绘制” / 工具状态显示“区域绘制”。
2. D2-A02 / 表面拾取与首点 / 在有效地图面左键一次 / 草稿首点出现且未产生文档历史噪声。
3. D2-A03 / 连续绘制与预览 / 左键增加至少两个点并移动指针 / 草稿边与指针预览线连续显示。
4. D2-A04 / 闭合提交 / 移至首点命中范围后左键 / 首点闭合候选显示，合法草稿仅调用一次 `CreateRegion` 并出现正式区域。
5. D2-A05 / 非法闭合 / 绘制自相交草稿并闭合 / 显示轻量错误，草稿保留，不自动修复。
6. D2-A06 / Esc 取消 / 有草稿时按 Esc / 草稿消失，工具回到“选择”，地图内容与历史不变。

用户完成并确认 6/6 后，才将 D2 标记 CLOSED。

## 候选主题

- Inspector 完整编辑闭环：字段编辑、提交、错误反馈、撤销/重做与真机路径。
- 地图数据落盘：`.xymap` 保存/打开、原子写入、失败保护、Dirty 与历史一致性。
- 区域绘制与地形表达：从编辑输入到地图区域/地形数据的真实闭环。
- DGD 衔接：地图数据与 DGD 节点/资源编辑链路的最小真实入口。

## 约束

- 未经 R3 冻结，不修改生产代码、不新增测试目标。
- 不把 R2 的遗留编号重新包装成 R3 主目标。
- 每个候选项先补范围、数据权威、Undo/Dirty 语义、真机 IPO 与门禁，再决定是否实施。
