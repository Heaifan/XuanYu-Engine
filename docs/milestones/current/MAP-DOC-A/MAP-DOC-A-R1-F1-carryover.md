# MAP-DOC-A-R1-F1 · Manifest Identity Carryover

## 历史事实

R1 真机验收中，M01～M06 已通过；M07 发现重新打开 `map.json` 后，Manifest 的 Tooltip/Copy 已是新 ID，但行内压缩 Text 仍可能保留旧值；M08 尚待最终复验。原 M07 不回写为通过。

按本轮裁定，R1-F1 不阻塞 R2 开发；本文件只记录修复范围与待补验，不改写原验收历史。

## F1-01 · ID 即时同步

`MapIdText` 是当前 Manifest ID 的唯一 VM 权威；`MapIdDisplay` 是其纯显示派生值。Manifest 成功打开、保存或 Save As 后，VM 立即通知 `MapIdText` 与 `MapIdDisplay`，因此 Text、Tooltip、Copy 均消费当前同一 ID。

## F1-02 · Copy 行布局

ID 行改为值列 `*`、复制按钮列 `Auto`。ID 允许 Ellipsis，复制按钮保留可达宽度；Tooltip 和 Copy 继续使用完整 ID。

## 自动覆盖

- A → B → C → B 立即刷新 Text/Tooltip/Copy 权威值。
- Save 与 Save As 不生成新 ID。
- 默认布局为 `*,Auto`，复制按钮不再依赖重新进入 Workspace 才可见。

最终真机补验：`R1F1-M01～M04`，并与 R2 验收一并完成；R1 仍不得单独宣布 CLOSED。
