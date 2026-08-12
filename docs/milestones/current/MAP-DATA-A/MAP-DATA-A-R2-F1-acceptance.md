# MAP-DATA-A-R2-F1 · Regional Authoring Hierarchy

## 真机验收

基线：<F1 HEAD>

### F1-M01 顶层 Workspace

结果：通过 / 不通过

下拉是否只有“地图编辑”“区域编辑”：

是否还存在“道路编辑”：

异常：

### F1-M02 区域编辑子模式

结果：通过 / 不通过

进入区域编辑后是否看到“区域面”“道路”：

两种子模式是否可以互相切换：

异常：

### F1-M03 工具切换与 Draft 安全

结果：通过 / 不通过

区域面绘制、道路绘制是否显示正确工具：

切换子模式时旧 Draft 是否取消、工具是否回到“选择”：

切换子模式是否没有自动创建 Dataset：

异常：

### F1-M04 统一图层栈

结果：通过 / 不通过

区域编辑中 Region Layer 与 Road Layer 是否同时可见：

切换子模式后另一类型图层是否仍保留：

异常：

### F1-M05 Layer → Mode 同步

结果：通过 / 不通过

点击 Road Layer 是否选中 Road Dataset 并切到“道路”：

点击 Region Layer 是否选中 Region Dataset 并切到“区域面”：

点击 Eye / Lock 是否不错误切换子模式：

异常：

### F1-M06 Region/Road 回归

结果：通过 / 不通过

Region Polygon：Bootstrap / 绘制 / Draft Undo-Redo / 完成 / Save-Reload：

Road Polyline：Bootstrap / 绘制 / Draft Undo-Redo / Enter 完成 / Save-Reload：

异常：

### F1-M07 Dataset / Layer 回归

结果：通过 / 不通过

Rename / Unregister / Selection / Inspector / Order / 保存后重开：

Eye / Lock 操作是否只改变状态、不改变 AuthoringMode：

异常：

### F1-M08 持久化与兼容

结果：通过 / 不通过

Region 0.2.0 与 Road 0.3.0 数据是否可保存、重开且身份不变：

旧地图是否无需迁移即可打开：

异常：

总体：通过 / 不通过
