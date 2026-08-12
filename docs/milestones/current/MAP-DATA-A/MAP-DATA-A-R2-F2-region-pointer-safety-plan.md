# MAP-DATA-A-R2-F2 · Region Pointer Safety

状态：实现完成，等待真机验收；F1 保持 USER ACCEPTANCE FAILED；R2 不得 CLOSED。

## 冻结目标

- T1：空 Draft、零 Anchor 的 Region PointerMove 显式安全 NO-OP，不读取 `Vertices[0]`，不产生任何状态副作用。
- T2：已有几何顶点交互优先于 Region Preview；活动 Vertex Drag 消费 PointerMove，绘制 Preview 暂停。
- T3：自动验证、真实事故路径记录、正式门禁与 Push；不把自动测试替代真机验收。

## 真实调用链

Native PointerMove → `NativePointerRoutePolicy` → `VulkanNativeHost.Pointer` → `PreviewDrawing` → `UiVm.RegionDrawingPointerMoved` → `RegionDrawingState.Draft`。
此前在 `Draft != null` 且 `Draft.Vertices.Length == 0` 时访问 `Vertices[0]`；现在先检查活动几何拖动、顶点命中和 `Vertices` 非空，再允许 Preview。

## 输入优先级

P0 Pointer Capture → P1 Vertex Drag → P2 Vertex Hit → P3 绘制 Click → P4 合法 Draft Preview → P5 普通 Picking。

## 禁止项

不修改 Dataset/Manifest/Feature Schema、Save/Load、Layer、Vulkan、相机、Picking 数学、XYUI 或整个 Pointer Router；本轮业务代码只触碰 5 个既有文件。
