# 审计资料目录规则

本目录保存可复核的审计输入、证据快照和原始审计包。

- `audit/requirements/`：审计请求、验收要求、复核要求。
- `audit/packages/`：原始 ZIP 审计包；不与展开目录混放。
- `audit/<审计案例>/`：需要在仓库中展开查看的审计证据快照。
- `docs/governance/audits/`：治理结论、治理复盘和治理审计报告。

规则：
1. 仓库根目录不放审计产物。
2. 不再使用 `docs/audit/`。
3. ZIP 只放 `audit/packages/`。
4. 审计请求只放 `audit/requirements/`。
5. 展开证据与治理报告分开：证据进 `audit/`，报告进 `docs/governance/audits/`。
