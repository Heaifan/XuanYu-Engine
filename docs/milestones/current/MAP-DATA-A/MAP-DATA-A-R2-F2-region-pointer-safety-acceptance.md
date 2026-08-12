# MAP-DATA-A-R2-F2 · Region Pointer Safety 真机验收

基线：`<F2 HEAD>`；版本：`<VERSION>`；状态：READY FOR USER ACCEPTANCE。

## F2-M01 空 Draft 移动

路径：区域编辑 → 区域面 → 绘制区域 → 不点击地图 → 移动鼠标。

输出：不闪退、不产生 Preview、不改变 Draft、History、Dataset、Layer 或 Selection。

结果：通过 / 不通过

## F2-M02 空 Draft 经过已有顶点

路径：绘制区域 → 不落点 → 鼠标移动到已有 Region 顶点。

输出：不闪退；顶点交互状态优先；Region Preview 不抢占。

结果：通过 / 不通过

## F2-M03 顶点按下与拖动

路径：鼠标移至已有顶点 → PointerDown → 轻微移动 → PointerUp。

输出：进入 Vertex Drag；Preview 暂停；无幽灵线；释放后行为正常。

结果：通过 / 不通过

## F2-M04 正常 Region Preview

路径：绘制区域 → 点击第一个点 → 移动鼠标。

输出：Preview 正常跟随，不自动增加 Draft 顶点。

结果：通过 / 不通过

## F2-M05 Cancel 安全

路径：绘制区域 → 落 1～2 点 → Esc → 移动鼠标。

输出：Draft 和 Preview 消失；不闪退；无幽灵点线。

结果：通过 / 不通过

## F2-M06 Region/Road 模式切换

路径：Region 开始 Draft → Road → Region → 移动鼠标。

输出：旧 Region Draft 取消，工具回到选择，不闪退。

结果：通过 / 不通过

## F2-M07 连续切换

路径：Region ↔ Road 连续切换至少 10 次，同时移动鼠标。

输出：无重复响应、无幽灵 Draft、无闪退。

结果：通过 / 不通过

## F2-M08 Region 完整绘制回归

路径：创建 Region Dataset → 区域面 → 3+ 顶点 → 完成 → Undo → Redo。

输出：原有 Region Polygon 流程正常。

结果：通过 / 不通过

## F2-M09 Road 最小回归

路径：Road Dataset → 道路 → Polyline → Enter 完成 → Undo/Redo。

输出：共享 Pointer Router 未破坏道路流程。

结果：通过 / 不通过

## F2-M10 CRASH-REPRO-01

路径：点击绘制区域 → 不落点 → 移动鼠标 → 经过已有顶点 → PointerDown → 轻微移动 → PointerUp。

输出：Editor 进程不终止；无 `IndexOutOfRangeException`；顶点交互与 Preview 边界符合预期。

结果：通过 / 不通过

## 总体

总体：通过 / 不通过

异常日志：

补充：
