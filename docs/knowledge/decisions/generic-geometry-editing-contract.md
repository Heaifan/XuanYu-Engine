# Generic Geometry Editing Contract

凡具有可编辑几何位置的地图 Feature，除非业务明确声明只读，默认必须支持选择、控制点编辑、Preview、提交、取消、Undo/Redo 与适用的几何吸附；不得为每一种 Feature 重复实现独立编辑状态机或独立 Snap Solver。

吸附候选必须来自局部空间查询，禁止 PointerMove 正式路径扫描全部 Feature。Snap 只改变独立几何坐标，不默认建立共享拓扑关系；Topology Weld 是独立能力。

通用模型将 Source 与 Target 解耦：拖动中的 Vertex 是 Source，附近的 Vertex 或 Segment 是 Target。Solver 只依赖 GeometryKind、Capabilities、顶点/线段候选和 Source Feature Exclusion，不依赖 Road/Region 类型组合。

默认仲裁顺序为 Vertex > Segment > Free，进入阈值 8px、释放阈值 12px；Target Lock 与稳定 Tie Break 保障拖动过程不抖动。Point 几何视为 VertexCount = 1，不因只有一个点而另建独立拖动系统。
