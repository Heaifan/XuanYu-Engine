# MAP-DOC-A-R3-F4 · Dataset/Layer 文字对齐真机验收

状态：`USER ACCEPTANCE PASS`。用户于 2026-08-11 确认 F4-M01、F4-M02、F4-M03 全部通过；R3 可以 Closeout。范围严格限定为 Dataset 与 Dataset Layer 列表文字对齐；未修改 UI Token、Schema、Region、Renderer、Registry、拖动算法或保存协议。

| 序号 | 路径 | 输入 I | 过程 P | 输出 O |
|---|---|---|---|---|
| F4-M01 | 地图编辑器 → 数据集 | 至少两个 Dataset | 观察每个 28 DIP Dataset 行 | Name 在其列内水平、垂直居中；Status 在固定 64 DIP 区内水平、垂直居中。 |
| F4-M02 | 地图编辑器 → 图层 | 至少两个 Dataset | 观察每个 32 DIP Layer 行 | Name 与 Status 均水平、垂直居中；Eye 与 Lock SVG 图标保持原位、清晰且可操作。 |
| F4-M03 | 数据集 / 图层 | 已有 Dataset | 切换、Visible、Lock、Drag、Rename、Create、Unregister、Save/Reload | 既有 Dataset/Layer 能力没有回归；Inspector 和表单标签仍保持左对齐。 |

真机结果：F4-M01 PASS；F4-M02 PASS；F4-M03 PASS；Overall PASS。
