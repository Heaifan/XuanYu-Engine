# XuanYu Engine AI 开发入口

> 开发规则唯一权威来源：[`docs/玄域引擎_AI开发宪法.md`](docs/玄域引擎_AI开发宪法.md)。本文件仅提供安全入口和导航，不另设审批、治理体系或重复门禁。

## 角色与权限

- 用户提出需求、确定优先级和产品方向，并作最终产品验收。
- ChatGPT 负责规划、审计、维护开发宪法和独立 XYK。
- 执行 AI 按任务范围开发、验证并如实报告；不得自行 Commit / Push。
- Parent 汇总任务负责集成，并是本轮唯一 Commit / Push Owner。

## 仓库与安全入口

- Repository Bootstrap / Resolver First：写入前执行 `scripts/xye-bootstrap.ps1`，确认实际仓库、branch、HEAD、upstream、Ahead/Behind 和 Dirty 状态。
- .NET SDK 由 `scripts/resolve-dotnet.ps1` Resolver Chain 解析；正式命令使用 `scripts/xye-dotnet.ps1`。PATH 中没有 `dotnet` 不代表 SDK 不存在。
- Dirty 文件必须原样保全。不得自动 reset、clean、stash、checkout 或覆盖；未知修改只阻断与其重叠或依赖的操作。
- 跨设备同步先 fetch 并只读核对。只有 index/worktree clean、Ahead=0 且 Behind>0 时允许 `git pull --ff-only`；否则暂停同步并报告。
- 写入前明确 Owner、WriteScope 和依赖；同一路径同一时段只能有一个写入者。不可确认来源或范围冲突时先停止受影响的操作。

## 工作与证据

- MEDIUM / HIGH 及索引登记的任务域，按宪法执行 PLANNING 与 EXECUTION Knowledge Preflight；入口为 [`docs/knowledge/knowledge-index.md`](docs/knowledge/knowledge-index.md)。XYK 是独立知识系统，不属于 XYT 或普通任务 WriteScope。
- XYT 负责测试、回归和验收证据。按宪法风险级别选择必要 Build、Test、静态检查或真实运行验证；不得把未执行或局部结果报告为通过或全量。
- 自动验证不代替用户产品验收。真实 UI、渲染、输入或生命周期要求未获用户验收时，报告为待验收。
- 报告说明 Changed、Verified、Residual Risk；施工任务附 `XYK CANDIDATE` 纯文本区块。不要直接写入 XYK。

## 规则导航

- 开发原则、权限、验证与交付：[`docs/玄域引擎_AI开发宪法.md`](docs/玄域引擎_AI开发宪法.md)
- 代码专属硬规则：[`docs/CODE_CONSTITUTION.md`](docs/CODE_CONSTITUTION.md)
- Bootstrap、Resolver、命令和执行细节：[`docs/dev-rules.md`](docs/dev-rules.md)
- XYK 知识索引：[`docs/knowledge/knowledge-index.md`](docs/knowledge/knowledge-index.md)
