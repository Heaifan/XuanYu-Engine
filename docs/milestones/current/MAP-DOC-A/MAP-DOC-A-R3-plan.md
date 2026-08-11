# MAP-DOC-A-R3 · Dataset Layer Editing

状态：`R3-F2 USER ACCEPTANCE FAIL → R3-F3 UI SPEC COMPLIANCE REWORK IMPLEMENTED`。R3 保持 OPEN，等待 F3 真机验收后才可 Closeout；MAP-DATA-A 禁止进入。

| TODO | 内容 | 状态 |
|---|---|---|
| R3-F1-T01 | 左侧 Dataset 行与选中态满宽 | COMPLETE（待真机复验） |
| R3-F1-T02 | Dataset Name 合同与显式编辑 | COMPLETE（待真机复验） |
| R3-F1-T03 | Drag Container Stability | COMPLETE（待真机复验） |
| R3-F2-T01 | Dataset 主次信息与完整选中区域 | COMPLETE（待真机验收） |
| R3-F2-T02 | Dataset Layer 图标操作与紧凑行布局 | COMPLETE（待真机验收） |
| R3-F2-T03 | SelectedDatasetId → Dataset/Layer/Inspector 投影 | COMPLETE（待真机验收） |
| R3-F2-T04 | 新建与危险操作区轻量分组 | COMPLETE（待真机验收） |
| R3-F3-T01 | 记录 F2 FAIL，保留历史 | COMPLETE |
| R3-F3-T02～T05 | Dataset 28 DIP 单行与操作区重组 | COMPLETE（待真机验收） |
| R3-F3-T06～T08 | Layer 32 DIP 单行与正式状态样式 | COMPLETE（待真机验收） |
| R3-F3-T09～T10 | Dataset Inspector 优先级与六项属性 | COMPLETE（待真机验收） |
| R3-F3-T11 | 静态 + Headless Bounds 合同 | COMPLETE |

冻结：Dataset 列表严格 28 DIP 单行，仅 Name + Status；Dataset Layer 严格 32 DIP 单行，顺序为 Drag / Name / Status / Visible / Lock，复用 `LayerPanel.States.axaml` 的 26×24 开关。`DatasetSelectedId` 是 Dataset 列表、Dataset Layer 与 Inspector 的唯一选择源，且选中 Dataset 必须隐藏 MapFormPanel 并显示六项 Dataset 属性。Layer State 只含 DatasetId、IsVisible、IsLocked、Order；点击显隐或锁不得改变选择；锁定禁用解除注册并由 Registry fail-closed；排序必须同步左右投影且保持选择。禁止 Schema、Region、Renderer、MAP-DATA-A、通用 Layer 业务模型、Dataset Registry、拖动算法、保存协议、全局 UI Token 以及 Inspector 大改。
