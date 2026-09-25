# 玄域引擎知识索引

> 最后治理更新：2026-09-25
> 使用方法：先按任务域定位必须读取项，再读取对应正文；禁止默认把全部知识无差别塞入实现上下文。

## 任务域预检映射

MEDIUM / HIGH 任务，以及下表已登记任务域，开始设计或写入前执行 Knowledge Preflight。

| 任务域 | 典型触发 | 优先读取 |
|---|---|---|
| 通用验证 / 交付 | Build、测试、真机、产物、Git 基线 | K-VAL-001、K-VAL-002、K-GOV-001～K-GOV-003；EXP-GOVERNANCE-001；再检查其他相关 ACTIVE EXP |
| 架构 / 状态所有权 | 分层、Composition Root、Workspace、事实源 | K-ARCH-001、K-ARCH-002；EXP-ARCH-001；`decisions/` 中相关 DEC；其他相关 ACTIVE EXP |
| 空间 / 几何 | Camera、Screen↔World、Geometry、Snap、Topology | K-SPA-001、K-SPA-002、K-GEO-001、K-GEO-002；相关 DEC / EXP |
| Rendering / Native | Overlay、Depth、Grid、NativeHost、Vulkan | K-REN-001～K-REN-004、K-NATIVE-001～K-NATIVE-002、L-REN-001、L-REN-002、L-NATIVE-001；EXP-ARCH-001；其他相关 ACTIVE EXP |
| Input | Pointer、Capture、手势 Owner、真实生产输入接线、平台输入 | K-INP-001～K-INP-004；EXP-TEST-001；其他相关 ACTIVE EXP |
| UI / Inspector | Layout、Measure/Arrange、Inspector、冻结交互、稳定属性编辑目标、Diagnostic | K-UI-001、K-DIAG-001；相关 `decisions/`；EXP-UI-001、EXP-UI-002；其他 UI 类 ACTIVE EXP |
| Data / Save / Asset | 保存、加载、覆盖、资源归一化、异步确认 | K-DATA-001～K-DATA-003、K-ASSET-001、K-ASSET-002；DATA 类 ACTIVE EXP |
| Performance | Preview、Commit、高频路径 | K-PERF-001；相关 ACTIVE EXP |
| Agent 历史错误 | 当前任务命中已知错误模式 | `docs/governance/agent-error-log.md` + `docs/governance/agent-experience-rules.md` 中命中的 ACTIVE EXP |
| Diagnostic / Viewport | Diagnostic、Viewport、NativeControlHost、Vulkan、Popup、Pointer、Capture、Input Router | K-VAL-001、K-VAL-002、K-NATIVE-001、K-NATIVE-002、K-INP-001～K-INP-004、K-DIAG-001、K-GOV-003、L-VAL-001、L-NATIVE-001、L-TEST-001；EXP-ARCH-001、EXP-TEST-001、EXP-UI-002、EXP-GOVERNANCE-001 |

任务若横跨多个域，只加载与当前 Scope 直接相关条目，不机械全文读取。

---

## 当前知识条目

| ID | 类型 | 分类 | 标题 | 优先级 | 证据 | 首次关键证据 | 状态 |
|---|---|---|---|---|---|---|---|
| K-VAL-001 | Knowledge | Engineering | 用户运行产物必须与验证产物一致 | P0 | E1 | v0.2.25.18-stab · 2026-08-10 16:51:42 · 06b26e9 | Active |
| K-VAL-002 | Knowledge | Engineering | UI/Native 功能必须分层验收 | P0 | E2 | v0.2.24.50-fix · 2026-08-09 19:42:41 · 60fd339 | Active |
| L-VAL-001 | Lesson | Engineering | 修复存在但真机完全不变时先证明运行时实际路由 | P0 | E1 | MAP-DATA-A-R2-F2-F2-F1 · 2026-08-12 · 3d53de0 | Active |
| L-NATIVE-001 | Lesson | Rendering | Native/Avalonia UI 问题连续两次局部 Placement 修复失败后审查 Airspace 与 Ownership | P0 | E2 | Diagnostic + Viewport R1 · 2026-09-25 | Active |
| L-TEST-001 | Lesson | Engineering | 测试使用自己制造的错误平台前提时绿灯更危险 | P0 | E2 | Region Snap R1 · 2026-09-25 | Active |
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
| K-NATIVE-002 | Knowledge | Architecture | Native↔Avalonia 坐标必须显式跨空间转换 | P0 | E2 | Diagnostic + Viewport R1 · 2026-09-25 · screen-space 修复 | Active |
| L-REN-002 | Lesson | Rendering | 双精度回退必须发生在第一次降精度之前 | P0 | E2 | F1-FAR-SAFE-01 · 2026-08-11 | Active |
| L-REN-001 | Lesson | Rendering | 连续参数修补失败必须重新审查承载架构 | P0 | E3 | GRID-RW-2A/B · c1451df / 2c57893 / 6154078 | Active |
| L-ARCH-001 | Lesson | Architecture | 跨越完整交互链的产品切片必须先拆清 Workspace 边界 | P1 | E1 | MAP-A → EDITOR-A · 2026-08-11 · 6724079 | Active |
| K-INP-001 | Knowledge | Input | 同一 Pointer 手势必须只有一个实时 Owner | P0 | E2 | v0.2.25.9-fix · 2026-08-10 11:48:28 · d621755 | Active |
| K-INP-002 | Knowledge | Input | Win32 Mouse Capture 必须统一管理完整释放生命周期 | P0 | E2 | v0.1.8.10-fix · 2026-06-26 · 8d6e7fd | Active |
| K-INP-003 | Knowledge | Input | Input Router 只有接入真实生产 Source 才算完成 | P0 | E2 | WAVE-2.5 E0/E5 · 2026-09-24 | Active |
| K-INP-004 | Knowledge | Input | 平台输入编码必须在 Adapter 边界正规化 | P0 | E2 | Region Snap R1 · 2026-09-25 | Active |
| K-DIAG-001 | Knowledge | UI | Diagnostic 必须保持观察者与输入透明 | P0 | E2 | Diagnostic + Viewport R1 · 2026-09-25 | Active |
| K-GOV-003 | Knowledge | Engineering | 当前仓库入口/Resolver 高于 Agent 历史环境记忆 | P0 | E2 | SDK resolver audit · 2026-09-25 | Active |
| K-UI-001 | Knowledge | UI | 冷启动错位/操作后恢复优先检查 Measure/Arrange 与命中热区 | P0 | E2 | v0.2.24.49-fix → .50-fix · 2026-08-09 · 60fd339 收口 | Active |
| K-DATA-001 | Knowledge | Data | 覆盖保存必须采用可回滚 Staging 事务 | P0 | E3 | v0.2.21.24-rz · 2026-08-02 14:10:00 · e089325 | Active |
| K-DATA-002 | Knowledge | Data | Load 必须 Candidate→Commit，结构失败与资源失败分级 | P0 | E3 | v0.2.21.25-rz · 2026-08-02 15:30:00 · cafe400 | Active |
| K-DATA-003 | Knowledge | Data | 异步危险确认必须捕获稳定对象身份并在确认后重新验证 | P0 | E1 | MAP-DATA-A-R2-F2-F2-F1 · 2026-08-12 · 3d53de0 | Active |
| K-ASSET-001 | Knowledge | Data | 数据归一化/Bake 后必须同步归一化相关元数据 | P0 | E2 | v0.2.21.23-fix · 2026-08-02 12:45:00 · a9c1ec6 | Active |
| K-ASSET-002 | Knowledge | Data | 确定性资源创建失败必须按 Key+Revision 负缓存 | P1 | E2 | v0.2.21.23-fix · 2026-08-02 12:45:00 · a9c1ec6 | Active |
| K-PERF-001 | Knowledge | Performance | Preview 高频路径与 Commit 重路径必须分离 | P0 | E2 | v0.1.8.7-fix · 2026-06-25 00:18 · 26f2006 | Active |
| K-GEO-001 | Knowledge | Architecture | 可编辑几何能力契约与 Snap/Topology 边界 | P0 | E1 | MAP-DATA-A-R2-F3-E1 · 2026-08-13 · 本轮提交 | Active |
| K-GEO-002 | Decision | Architecture | R2 收口并以 Point Consumer 作为下一验证形态 | P0 | R2 | MAP-DATA-A-R2-CLOSEOUT · 2026-08-13 · 6a3d5b8 | Active |

## 分类文件

- `engineering.md`：K-VAL-001、K-VAL-002、K-GOV-001、K-GOV-002、K-GOV-003
- `architecture.md`：K-SPA-001、K-SPA-002、K-ARCH-001、K-ARCH-002、K-GEO-001；R2 Closeout / Point Foundation 见 K-GEO-002
- `rendering.md`：K-REN-001、K-REN-002、K-REN-003、K-REN-004、K-NATIVE-001
- `input.md`：K-INP-001～K-INP-004
- `ui.md`：K-UI-001、K-DIAG-001
- `data.md`：K-DATA-001、K-DATA-002、K-DATA-003、K-ASSET-001、K-ASSET-002
- `performance.md`：K-PERF-001
- `incidents.md`：代表性事故记录与映射
- `lessons.md`：L-ARCH-001、L-REN-001、L-REN-002、L-VAL-001、L-NATIVE-001、L-TEST-001 及后续复盘
- `decisions/`：已批准并仍有长期约束价值的 DEC
- `docs/governance/agent-error-log.md`：Agent 真实错误事实
- `docs/governance/agent-experience-rules.md`：去重后的防复发经验规则
