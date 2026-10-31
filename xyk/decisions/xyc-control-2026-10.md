# DEC-XYE-XYC-CONTROL-20261014｜语义控制独立平面

**ACTIVE（能力设计裁定）｜2026-10-14～15。**

XYC Control为原生后台语义命令入口，不依靠占用用户鼠标；XYT负责测试治理，XYK负责知识资产，XYE/XYUI仍拥有产品业务状态。XYC复用Editor/World/WarCore既有写线程、Revision、Undo/Redo和权限，不应复制第二套场景、Camera或History State；Core能力不依赖MCP特定协议。

能力原则分READ/DIAGNOSTIC/SANDBOX_WRITE/LIVE_WRITE；真实写操作需明确作用域、授权与事务回读。Silent/Offscreen与真实可视UiWin是不同承载。后续单位、装备、Linux Agent目标是规划，不由本DEC证明已经开发。

阶段产品提交：6b1979899d7421ee63d75833ebc7ceb0648f1505（P0），d2d011ac98314f172e0e3d729583e67832099f1f（P1-A），fa666cb17d47fa96eeb13867ffc06572c803dac9（P2-A）。相关K-XYC-001～003。
