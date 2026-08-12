# XYUI Core Pack 0.1

> XYUI-PILOT 产物：供 XYLab Agent 消费的 Core Pack。
> 状态：`PARTIAL_012_READY`（XYUI-0/1/2 就绪；XYUI-3 Source 待提供）。

## 内容

```text
xyui/packs/core-0.1/
├─ manifest.json     ← pack 元数据 + SHA/commit 固定依赖
├─ AGENT-GUIDE.md    ← XYLab Agent 消费指南（强制先读）
├─ README.md         ← 本文件
└─ gaps.json         ← 统一 GAP 注册（5 项）
```

## 已就绪规范

| 层 | 状态 | 路径 |
|---|---|---|
| XYUI-0 Foundation Registry | VALIDATED（44 项） | `xyui/registry/foundation/foundation-registry.json` |
| Canonical Token Architecture | A3-R2 CLOSED（426 条） | `xyui/tokens/architecture/token-canonical-map.json` |
| XYUI-1 Text & Information | R1 reconciled（24 组件） | `xyui/specs/XYUI1/XYUI-1.canonical.md` |
| XYUI-2 Buttons & Inputs | R2 reconciled（24 控件） | `xyui/specs/XYUI2/XYUI-2.canonical.md` |

## 依赖固定方式

- 不复制 Foundation Registry；引用 + SHA-256 固定（manifest 中）
- pack 随 git commit 走版本（`c92a873e` 基线）

## 缺失声明（不伪造）

- `XYUI-3.md` 正式 Source 尚未提供 → `XYUI3_SOURCE_MISSING`（唯一阻塞项）
- Source 由人类提供后执行 R3 reconciliation，0123 即完整闭环

## 已知 GAP（5 项）

```text
XYUI1-GAP-001  Icon glyph registry 未建立（glyph 名暂用组件级常量）
XYUI2-GAP-001  XY.Size.Switch 复合 token 子属性访问待 A3 定义
XYUI2-GAP-002  TextArea.MaxHeight=SceneToken（依赖场景/视口上下文，待裁定）
XYUI2-GAP-003  Inspector SharedPropertyColumnRule 未在 Foundation 定义
XYUI3_SOURCE_MISSING（阻塞）
```

## 本 pack 禁止

A3-R3 Light/Dark、AXAML/C# 生成、XYUI-4+、XuanYu/XYLab 业务实现、修改 A2 Registry / A3-R2 Architecture。
