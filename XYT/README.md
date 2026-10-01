# XYT

XYT 是玄域仓库的测试真实性、证据与收口控制面。

## 唯一入口

从仓库根目录使用：

- `xyt` / `xyt.bat`
- `xyt quick` / `xyt 快速验证`
- `xyt module` / `xyt 模块收口`
- `xyt global` / `xyt 全局收口`

根目录入口只负责转发，不承载业务实现。

## 目录职责

- `XYT/**`：XYT 自身运行时、执行、Witness、Acceptance、Integration、Report 和自测试。
- `scripts/governance/xyt-*`：面向仓库执行的治理脚本和 Gate。
- `tools/governance/xyt-*`：治理维护工具、账本/真值修复工具，不作为普通用户入口。
- `docs/governance/xyt-*`：XYT 合同、规则、Schema 说明和治理记录。
- 根目录 `xyt.ps1` / `xyt.bat`：稳定启动入口。

## 边界规则

1. 新的 XYT 运行时代码优先进入 `XYT/**`，不得继续散落到根目录。
2. 新的仓库执行 Gate 进入 `scripts/governance/`。
3. 只用于维护治理数据的工具进入 `tools/governance/`。
4. 规范、合同和报告进入 `docs/governance/`。
5. 不为了“目录统一”把四类职责机械合并；先按职责判断，再决定物理位置。
6. 根目录只保留稳定入口，不新增新的 XYT 实现脚本。
