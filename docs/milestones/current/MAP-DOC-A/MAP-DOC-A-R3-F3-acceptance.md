# MAP-DOC-A-R3-F3 · UI Spec Compliance Rework 真机验收

状态：`IMPLEMENTED · READY FOR USER ACCEPTANCE`，不是 `CLOSED`。基线将在本轮 commit 后填写。范围严格限定为 Dataset/Layer UI Spec 合规与 Dataset Inspector；不修改 Schema、Region、Renderer、Registry、拖动算法、保存协议或全局 Token。

| 序号 | 路径 | 输入 I | 过程 P | 输出 O |
|---|---|---|---|---|
| F3-M01 | 地图编辑器 → 数据集 | 已有 Dataset | 观察和切换列表 | 严格 28 DIP 单行、满宽选中、仅 Name + Status；无 Type·ID 第二行。 |
| F3-M02 | 地图编辑器 → 数据集 | 已选 Dataset | 修改名称、创建、观察危险操作 | 操作区层级清楚；无 Unicode `＋`；解除注册独立分组。 |
| F3-M03 | 地图编辑器 → 图层 | 至少两个 Dataset | 观察右侧图层 | 严格 32 DIP 单行，顺序为 Drag / Name / Status / Visible / Lock，无拥挤或第二行。 |
| F3-M04 | 地图编辑器 → 图层 | 已选 Dataset | 切换 Visible / Lock | 使用正式状态开关；Visible、Hidden、Locked、Unlocked 在选中态仍清晰。 |
| F3-M05 | 数据集 → 图层 | 至少两个 Dataset | 点击左侧任一行 | 右侧同 ID Layer 同时选中。 |
| F3-M06 | 图层 → 数据集 | 至少两个 Dataset | 点击右侧任一行 | 左侧同 ID Dataset 同时选中。 |
| F3-M07 | 检查器 | 已选 Dataset | 观察检查器 | 不显示地图属性；显示“数据集属性”及名称、类型、ID、状态、可见、锁定。 |
| F3-M08 | 数据集 / 图层 | 已有多个 Dataset | Visible、Lock、Drag、Rename、Create、Unregister、Save/Reload | 全部既有能力保持有效。 |

仅当 F3-M01～F3-M08 由用户真机通过后，R3 才可进入纯 Closeout；此前 MAP-DATA-A 禁止进入。
