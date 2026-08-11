# MAP-DOC-A-R3 · Dataset Layer Editing

状态：`PLANNED`。仅完成五项：满宽 Layer Row、Visibility、Lock、右侧拖拽排序、保存重开恢复。

| TODO | 内容 | 状态 |
|---|---|---|
| R3-T01 | DatasetLayerState 合同与 Manifest 持久化 | TODO |
| R3-T02 | Layer Dock 满宽布局、状态图标与拖拽排序 | TODO |
| R3-T03 | 行为接线、自动门禁、真机 IPO 与收口 | TODO |

冻结：Layer State 只含 DatasetId、IsVisible、IsLocked、Order；Dataset 本体与 JSON 不增加这些字段。点击显隐或锁不得改变选择；锁定禁用解除注册；排序必须同步左右投影且保持选择。禁止分组、过滤、名称、多选、样式、右键菜单和文件夹。
