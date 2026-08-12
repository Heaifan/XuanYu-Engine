# MAP-DATA-A-R1-F3 · Region Authoring UX 真机验收

状态：`READY FOR USER ACCEPTANCE`。自动测试不替代人工验收；F3 六项全部 PASS 后继续 R1-M02～R1-M07，R1 保持 OPEN。

| 序号 | 路径 | 输入 I | 过程 P | 输出 O |
|---|---|---|---|---|
| F3-M01 | 编辑模式 → 区域编辑 → 左侧区域工作区 | 正常 Region Dataset | 观察左侧并点击“绘制区域” | 左侧显示当前 Dataset、绘制入口、草稿状态和区域内容数量；顶部不再出现区域专属绘制按钮；入口只存在一份。 |
| F3-M02 | 左侧区域工作区 → 视口 | 正在绘制 Draft | 点击 4 个点，观察“顶点”和“状态” | 左侧显示顶点数量；0 点为“尚未开始绘制”，1～2 点为“至少需要 3 个顶点”，3 点以上为“可以闭合”。 |
| F3-M03 | 左侧区域工作区 → 撤销/重做及快捷键 | P1→P2→P3→P4 | 点击撤销/重做顶点并使用 Ctrl+Z/Ctrl+Y | Draft 顶点按预期回退/恢复；新点击顶点后 Draft Redo 清空；活动 Draft 不污染 Map History。 |
| F3-M04 | 左侧区域工作区 → 完成/取消 | 至少 3 个 Draft 顶点或活动 Draft | 点击“完成区域”或“取消绘制” | 完成创建一个正式 Region History Entry；取消清空 Draft；四个子按钮在无 Draft 时全部不可用。 |
| F3-M05 | 区域编辑 → 右下图层列表 | Dataset-backed Region Layer | 双击图层名称，输入“广东”，按 Enter、Esc、失去焦点分别验证 | Enter/失焦提交并同步 Dataset/Layer/Inspector；Esc 放弃；名称权威仍为 Dataset Name。 |
| F3-M06 | 区域编辑 → 右下图层列表 → 删除 | 选中未锁定 Region Dataset Layer | 点击删除并确认 | 从当前地图解除注册区域图层；对应 Dataset 文件保留；锁定图层删除按钮不可用；取消确认不改变任何内容。 |

验收结论：用户填写 `PASS / FAIL`。F3 未全部 PASS 前，不得标记 R1 CLOSED；R1 全量通过前不得启动 R2。
