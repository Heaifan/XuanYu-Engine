# DEC-XYE-TERRAIN-HYDROLOGY-20261003｜地形与水文权威

**ACTIVE（设计裁定）｜2026-10-03～04｜用户批准编号1C/2C/3C以及后续多项水文/Layer合同。**

原始DEM默认只读；Scenario是地形、场景及编辑状态的拥有者，编辑通过非破坏性Layer和Dirty Region。Gameplay Height=Base DEM+权威编辑增量；渲染高度夸张和ScaleBar等显示参数不能进入Gameplay、寻路、地表水物理查询。Authoring控制点是逻辑编辑权威，贴地渲染采样点/LOD不能自动增加权威Dataset顶点，也不应写回DEM。

Tile不是编辑和水文的人工硬边界；Layer类型、合成顺序和Override必须显式，Bake保留Provenance。河流/水面遵从最终地形及Water Authority，可以有记录清楚的业务Override；Hydrology不直接改Base DEM。Terrain Commit之后仅在受影响流域/Dirty范围进行增量水文更新；全球应以Graph/流域层级、局部高精度替代全球全时实时流体模拟。

**边界**：这是一份用户批准的架构与玩法合同，并不证明上述Hydrology Solver、跨Tile操作、所有1C～24C子项或Terrain Editor均已实施、编译及真机验证。关联K-GEO-003/004、K-DATA-002、K-PERF-002。
