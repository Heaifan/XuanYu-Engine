# XYUI Core Pack 0.1

> XYUI-PILOT 产物：供 XYLab Agent 消费的 Core Pack。
> 状态：`READY_WITH_MISSING_SOURCES`（XYUI-0/1 就绪；XYUI-2/3 Source 缺失，见 manifest known_gaps）。

## 内容

```text
xyui/packs/core-0.1/
├─ manifest.json     ← pack 元数据 + SHA/commit 固定依赖
├─ AGENT-GUIDE.md    ← XYLab Agent 消费指南（强制先读）
├─ README.md         ← 本文件
└─ gaps.json         ← 统一 GAP 注册（含 2/3 SOURCE_MISSING）
```

## 已就绪规范

| 层 | 状态 | 路径 |
|---|---|---|
| XYUI-0 Foundation Registry | VALIDATED（44 项） | `xyui/registry/foundation/foundation-registry.json` |
| Canonical Token Architecture | A3-R2 CLOSED（426 条） | `xyui/tokens/architecture/token-canonical-map.json` |
| XYUI-1 Text & Information | R1 reconciled（24 组件） | `xyui/specs/XYUI1/XYUI-1.canonical.md` |

## 依赖固定方式

- 不复制 Foundation Registry；引用 + SHA-256 固定（manifest 中）
- pack 随 git commit 走版本（`a2076df7` 基线）

## 缺失声明（不伪造）

- `XYUI-2.md` 正式 Source 不存在 → `XYUI2_SOURCE_MISSING`
- `XYUI-3.md` / 对应正式导出大纲不存在 → `XYUI3_SOURCE_MISSING`
- 两者均为阻塞项；Source 由人类提供后执行 R2/R3 reconciliation

## 本 pack 禁止

A3-R3 Light/Dark、AXAML/C# 生成、XYUI-4+、XuanYu/XYLab 业务实现、修改 A2 Registry / A3-R2 Architecture。
