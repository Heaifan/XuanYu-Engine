# WORLD-C-R4-D1：GLB 静态模型解析与玄域数据转换

## 收口结论

`WORLD-C-R4-D1` 完成静态 GLB 数据导入核心：合法 GLB 可以离线转换为玄域自有 `StaticModelData`。本轮不接入 Vulkan、UI、SceneDocument、AssetId 注册或场景实体创建。

## 实现边界

- 入口：`GlbImportService.ImportFile`、`ImportStream`、`ImportBytes`。
- 解析库：`SharpGLTF.Core` 1.0.6，仅在导入服务内部用于 GLB 解析验证和必需扩展拦截。
- 输出：自有顶点、索引、Primitive、LocalBounds、Metadata 和去重 Warning。
- 坐标：按 D0 合同执行 `(x, y, z) -> (x, -z, y)`，保持右手系，不翻转三角形绕序。
- 节点：静态 Node Transform 烘焙进顶点；Matrix 优先，TRS 按 glTF 规则求值。
- 法线：使用逆转置法线矩阵并归一化；不可逆或非法数值失败。
- Bounds：基于最终转换后的顶点数据计算，不盲信 Accessor min/max。

## 支持范围

- GLB 2.0。
- TRIANGLES Primitive。
- 多 Primitive、多 Node、多 Mesh 引用。
- 16 位和 32 位索引；无显式索引时生成顺序索引。
- POSITION、NORMAL、TEXCOORD_0。
- `baseColorFactor`。

## 拒绝与降级

- Required Extension：失败。
- Skinned Mesh：失败。
- 非 TRIANGLES：跳过并 Warning；若没有可绘制三角形则失败。
- 缺少 UV：继续导入，使用默认 UV，并记录一次 Warning。
- 动画：不导入，记录一次 Warning。
- 贴图、相机、灯光、Morph Target：不进入 D1 输出合同。

## 后续入口

`WORLD-C-R4-D2` 才允许进入 Vulkan 静态模型显示、Vertex/Index Buffer、多 Primitive Draw、基础颜色显示、GPU 资源缓存和释放链。
