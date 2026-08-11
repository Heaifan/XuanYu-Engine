# MAP-DOC-A-R3 · Dataset Layer Editing

状态：`USER ACCEPTANCE FAILED → R3-F1 IMPLEMENTING`。R3-M02 Visibility、R3-M03 Lock 已由真机确认；R3-M01 左侧行未满宽、R3-M04 拖拽导致列表消失，R3 保持 OPEN。

| TODO | 内容 | 状态 |
|---|---|---|
| R3-F1-T01 | 左侧 Dataset 行与选中态满宽 | COMPLETE（待真机复验） |
| R3-F1-T02 | Dataset Name 合同与显式编辑 | COMPLETE（待真机复验） |
| R3-F1-T03 | Drag Container Stability | COMPLETE（待真机复验） |

冻结：Layer State 只含 DatasetId、IsVisible、IsLocked、Order；Dataset Descriptor 新增可选 Name（Name≠ID，允许重复）。点击显隐或锁不得改变选择；锁定禁用解除注册并由 Registry fail-closed；排序必须同步左右投影且保持选择。旧 Manifest 缺少状态或 Name 时仍可打开；首次保存/Promotion 才持久化。禁止分组、过滤、多选、样式、右键菜单和文件夹。
