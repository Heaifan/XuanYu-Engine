# XYT

XYT is the repository's single formal entry point for validation and closeout
dispatch.

Use `xyt` or `xyt.bat` from the repository root:

- `xyt quick` / `xyt 快速验证`
- `xyt module` / `xyt 模块收口`
- `xyt global` / `xyt 全局收口`

The initial dispatcher performs an empty run and emits a unified startup
report. Mode-specific implementations belong under this directory.
