# MAP-DOC-A-R3 · Dataset Layer Editing

状态：`FUNCTION ACCEPTANCE PASS → R3-F2 UI CLOSEOUT FIX IMPLEMENTED`。R3 保持 OPEN，等待本轮 UI 真机验收后才可 Closeout。

| TODO | 内容 | 状态 |
|---|---|---|
| R3-F1-T01 | 左侧 Dataset 行与选中态满宽 | COMPLETE（待真机复验） |
| R3-F1-T02 | Dataset Name 合同与显式编辑 | COMPLETE（待真机复验） |
| R3-F1-T03 | Drag Container Stability | COMPLETE（待真机复验） |
| R3-F2-T01 | Dataset 主次信息与完整选中区域 | COMPLETE（待真机验收） |
| R3-F2-T02 | Dataset Layer 图标操作与紧凑行布局 | COMPLETE（待真机验收） |
| R3-F2-T03 | SelectedDatasetId → Dataset/Layer/Inspector 投影 | COMPLETE（待真机验收） |
| R3-F2-T04 | 新建与危险操作区轻量分组 | COMPLETE（待真机验收） |

冻结：Layer State 只含 DatasetId、IsVisible、IsLocked、Order；Dataset Descriptor 新增可选 Name（Name≠ID，允许重复）。`DatasetSelectedId` 是 Dataset 列表、Dataset Layer 与 Inspector 的唯一选择源；点击显隐或锁不得改变选择；锁定禁用解除注册并由 Registry fail-closed；排序必须同步左右投影且保持选择。旧 Manifest 缺少状态或 Name 时仍可打开；首次保存/Promotion 才持久化。禁止 Region 编辑、Schema 修改、新属性系统、Inspector 大改、分组、过滤、多选、样式、右键菜单和文件夹。
