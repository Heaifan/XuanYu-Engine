# 玄域引擎知识索引

> 索引整理时间：2026-08-11 00:03（UTC+08:00）
> 状态：V3 正式入库，共 22 条 Knowledge、4 条 Lesson。
> 使用方法：AI 接到任务后先按标签/分类定位相关 ID，再读取对应主题文件全文；不要默认把全部知识无差别塞入实现上下文。

| ID | 类型 | 分类 | 标题 | 优先级 | 证据 | 首次关键证据 | 状态 |
|---|---|---|---|---|---|---|---|
| K-VAL-001 | Knowledge | Engineering | 用户运行产物必须与验证产物一致 | P0 | E1 | v0.2.25.18-stab · 2026-08-10 16:51:42 · 06b26e9 | Active |
| K-VAL-002 | Knowledge | Engineering | UI/Native 功能必须分层验收 | P0 | E2 | v0.2.24.50-fix · 2026-08-09 19:42:41 · 60fd339 | Active |
| L-VAL-001 | Lesson | Engineering | 修复存在但真机完全不变时先证明运行时实际路由 | P0 | E1 | MAP-DATA-A-R2-F2-F2-F1 · 2026-08-12 · 3d53de0 | Active |
| K-GOV-001 | Knowledge | Engineering | 历史唯一身份以 Commit Hash 为准 | P0 | E2 | SHR-2026-08-R2 · 涉及 3 组重复版本 | Active |
| K-GOV-002 | Knowledge | Engineering | 治理成果必须建立自动防回潮门禁 | P1 | E3 | 8.8-0 · 2026-06-23 23:09:45 · 4c4d82c | Active |
| K-SPA-001 | Knowledge | Architecture | 大地图 Screen↔World CPU 链使用双精度并做往返验证 | P0 | E2 | v0.2.25.12-rz · 2026-08-10 12:20:03 · 0594c4c | Active |
| K-SPA-002 | Knowledge | Architecture | 斜视 Metric 具有方向性，失败保持上一合法状态 | P1 | E1 | v0.2.25.17-stab · 2026-08-10 · c307c66 | Active |
| K-ARCH-001 | Knowledge | Architecture | Composition Root 初始化顺序属于依赖合同 | P0 | E1 | v0.1.7.1-fix · 2026-06-24 11:45 · 359e3ce | Active |
| K-ARCH-002 | Knowledge | Architecture | 产品模式持续膨胀时先建立 Workspace 边界 | P1 | E1 | MAP-A → EDITOR-A · 2026-08-11 · 6724079 | Active |
| K-REN-001 | Knowledge | Rendering | Editor Overlay 不得用世界坐标偏移制造视觉层级 | P0 | E2 | v0.2.25.13-rz · 2026-08-10 13:37:23 · ef12f4b | Active |
| K-REN-002 | Knowledge | Rendering | 共面 Overlay 应由独立 Depth Policy 与 Draw Order 表达 | P0 | E2 | v0.2.25.15-stab · 2026-08-10 14:22:43 · 751da52 | Active |
| K-REN-003 | Knowledge | Rendering | Background / Sky 必须具有明确且独立的 Depth 语义 | P0 | E2 | v0.2.21.21-fix · 2026-08-01 16:56:53 · e0a994a | Active |
| K-REN-004 | Knowledge | Rendering | Editor World Reference Grid 必须独立于 MapGround | P0 | E3 | v0.2.25.28-fix → .29-fix · 2c57893 / 6154078 | Active |
| K-NATIVE-001 | Knowledge | Rendering | Native Overlay 必须验证真实 HWND 层级与绘制状态 | P0 | E2 | v0.2.25.18-stab · 2026-08-10 16:51:42 · 06b26e9 | Active |
| L-REN-002 | Lesson | Rendering | 双精度回退必须发生在第一次降精度之前 | P0 | E2 | F1-FAR-SAFE-01 · 2026-08-11 | Active |
| L-REN-001 | Lesson | Rendering | 连续参数修补失败必须重新审查承载架构 | P0 | E3 | GRID-RW-2A/B · c1451df / 2c57893 / 6154078 | Active |
| L-ARCH-001 | Lesson | Architecture | 跨越完整交互链的产品切片必须先拆清 Workspace 边界 | P1 | E1 | MAP-A → EDITOR-A · 2026-08-11 · 6724079 | Active |
| K-INP-001 | Knowledge | Input | 同一 Pointer 手势必须只有一个实时 Owner | P0 | E2 | v0.2.25.9-fix · 2026-08-10 11:48:28 · d621755 | Active |
| K-INP-002 | Knowledge | Input | Win32 Mouse Capture 必须统一管理完整释放生命周期 | P0 | E2 | v0.1.8.10-fix · 2026-06-26 · 8d6e7fd | Active |
| K-UI-001 | Knowledge | UI | 冷启动错位/操作后恢复优先检查 Measure/Arrange 与命中热区 | P0 | E2 | v0.2.24.49-fix → .50-fix · 2026-08-09 · 60fd339 收口 | Active |
| K-DATA-001 | Knowledge | Data | 覆盖保存必须采用可回滚 Staging 事务 | P0 | E3 | v0.2.21.24-rz · 2026-08-02 14:10:00 · e089325 | Active |
| K-DATA-002 | Knowledge | Data | Load 必须 Candidate→Commit，结构失败与资源失败分级 | P0 | E3 | v0.2.21.25-rz · 2026-08-02 15:30:00 · cafe400 | Active |
| K-DATA-003 | Knowledge | Data | 异步危险确认必须捕获稳定对象身份并在确认后重新验证 | P0 | E1 | MAP-DATA-A-R2-F2-F2-F1 · 2026-08-12 · 3d53de0 | Active |
| K-ASSET-001 | Knowledge | Data | 数据归一化/Bake 后必须同步归一化相关元数据 | P0 | E2 | v0.2.21.23-fix · 2026-08-02 12:45:00 · a9c1ec6 | Active |
| K-ASSET-002 | Knowledge | Data | 确定性资源创建失败必须按 Key+Revision 负缓存 | P1 | E2 | v0.2.21.23-fix · 2026-08-02 12:45:00 · a9c1ec6 | Active |
| K-PERF-001 | Knowledge | Performance | Preview 高频路径与 Commit 重路径必须分离 | P0 | E2 | v0.1.8.7-fix · 2026-06-25 00:18 · 26f2006 | Active |

## 分类文件

- `engineering.md`：K-VAL-001、K-VAL-002、K-GOV-001、K-GOV-002
- `architecture.md`：K-SPA-001、K-SPA-002、K-ARCH-001、K-ARCH-002
- `rendering.md`：K-REN-001、K-REN-002、K-REN-003、K-REN-004、K-NATIVE-001
- `input.md`：K-INP-001、K-INP-002
- `ui.md`：K-UI-001
- `data.md`：K-DATA-001、K-DATA-002、K-DATA-003、K-ASSET-001、K-ASSET-002
- `performance.md`：K-PERF-001
- `incidents.md`：上述知识的代表性事故记录与映射
- `lessons.md`：L-ARCH-001、L-REN-001、L-REN-002、L-VAL-001 及后续可复用的停止条件与错误前提复盘
