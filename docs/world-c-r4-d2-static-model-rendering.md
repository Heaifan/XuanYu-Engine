# WORLD-C-R4-D2 Vulkan 静态模型显示

## 裁定

`WORLD-C-R4-D2` 建立 `StaticModelData -> RenderStaticModelResource -> Vulkan Buffer -> CmdDrawIndexed` 的最小稳定链路。D2 不提供正式用户导入入口，也不写入 SceneDocument 或 `.xyassets`。

## 合同

- Render 合同落点：`XuanYu.Render.Abstractions/RenderStaticModel*.cs`。
- 实体只携带 `RenderStaticModelKey`；模型数据在 `RenderProjection.StaticModelResources`。
- 第三方类型、GLB 路径、Vulkan Handle、SceneDocument 类型均不进入合同。
- 同一 Key + Revision 复用 GPU 资源；Revision 变化时先创建新资源，成功后替换旧资源。

## Vulkan

- 顶点布局：Position、Normal、UV0。
- 索引：统一 `VK_INDEX_TYPE_UINT32`。
- Draw：每个 Primitive 使用自己的 `FirstIndex`、`IndexCount`、`BaseColorFactor`。
- Shader：基础方向光；Normal 在非均匀 Scale 下按 inverse-scale 再旋转。
- Depth：新增随 swapchain 尺寸重建的 depth attachment；模型 buffer 不随 resize 重传。
- 选择反馈：D2 使用 Bounds 轮廓，明确不是最终几何描边。

## 生命周期

- 首次引用：校验数据后创建 vertex/index buffer。
- 重复帧：同 Key+Revision 不重复上传。
- 多实例：多个实体共享同一 GPU 模型资源，各自提交 Transform。
- 替换失败：保留旧资源。
- 场景切换：未引用资源在命令重录安全点释放。
- Renderer Dispose：模型资源早于 CommandPool、RenderPass、Device 释放。

## 受控演示

设置 `XUANYU_D2_STATIC_MODEL_DEMO=1` 后，现有测试实体会被替换为 D2 静态模型演示资源。该入口不提供导入按钮，不读取用户文件，不写 `.xyscene`，D3 可自然替换。

## D3 边界

D3 才负责正式导入按钮、文件选择、AssetId 注册、`.xyassets` 托管复制、模型实体创建、层级树、Inspector、Picking、保存/重开和缺失资源处理。
