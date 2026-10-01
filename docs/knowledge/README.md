# 历史知识镜像（只读）

> 本目录已经退出“正式知识库”职责，只保留迁移前的历史快照和审计线索。

## 唯一权威位置

玄域正式知识库固定为：

```text
branch: xyk/main
path:   xyk/**
index:  xyk/index/knowledge-index.md
ledger: xyk/ledger/knowledge-events.tsv
```

正式 Knowledge / Experience / Lesson / Decision 的新增、强化、替代和退役，只允许写入 `xyk/main`。

## 本目录规则

- `docs/knowledge/**` 全部视为历史只读资料。
- 禁止在本目录新增或更新正式知识条目。
- 本目录中的旧索引、路径和状态不得作为“当前 XYK 状态”引用。
- 需要当前知识时，应读取 `origin/xyk/main:xyk/**`。
- XYK 不合并、不变基、不 cherry-pick 到产品分支；施工 Agent 只读，正式写回由 ChatGPT / XYK Coordinator 负责。
- 旧文件保留仅用于追溯，不做无痕删除。
