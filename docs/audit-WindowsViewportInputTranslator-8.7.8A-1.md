# 8.7.8A-1 — WindowsViewportInputTranslator 拆分审计

审计日期：2026-06-23
目标文件：`WindowsViewportInputTranslator.cs`（284 行）

---

## 一句话总结

这个文件干了 3 件事：**管理修饰键状态、翻译原始输入、查表找动作匹配**。3 件事混在一个文件里，可以拆成 3 个文件，每件独立。

---

## 当前职责分析

| 职责 | 行数 | 说明 |
|------|------|------|
| 修饰键状态管理 | 55 行 | UpdateModifierState、IsModifierKey、4 个 VK 常量、_currentModifiers 字段 |
| 原始事件翻译 | 100 行 | OnRawKeyDown/Up、OnRawPointerButtonDown/Moved/Up、OnRawMouseWheel、OnRawInputFocusLost |
| 查表匹配 | 40 行 | BuildSignature、ButtonCodeToName、_snapshot.Resolve、BeginDrag/EndDrag |
| Debug 跟踪 | 40 行 | s_traceEnabled + 各处 Debug.WriteLine |
| 框架 + 字段 | 50 行 | 类声明、_snapshot、_lastMouseX/Y、构造、SnapshotReplaced、CancelActiveDrag |

---

## 建议拆分方案

拆成 3 个文件，不改逻辑：

### 文件 1：`WindowsViewportInputTranslator.cs`（≈80 行）

只保留：事件分发 + 主流程编排

```csharp
public sealed class WindowsViewportInputTranslator
{
    // 字段：_snapshot, _lastMouseX, _lastMouseY
    // 构造 + OnSnapshotReplaced + Revision + CurrentModifiers
    // 所有 OnRaw* 方法（事件入口）
    // CancelActiveDrag, OnRawInputFocusLost
}
```

### 文件 2（新建）：`ModifierKeyState.cs`（≈55 行）

只负责：修饰键的跟踪和查询

```csharp
// 属性 CurrentModifiers
// UpdateModifierState(vk, pressed)
// IsModifierKey(vk)
// Reset()
// VK 常量
```

### 文件 3（新建）：`InputBindingResolver.cs`（≈90 行）

只负责：手势签名构建 + 查表匹配

```csharp
// BuildSignature(device, code, kind, modifiers) — 静态
// ButtonCodeToName(code) — 静态
// 持有 EditorInputBindingSnapshot 引用
// Resolve(sig) → EditorInputMatch?
// s_traceEnabled + debug 日志
```

---

## 拆分后变化

| 指标 | 之前 | 之后 |
|------|------|------|
| `WindowsViewportInputTranslator.cs` | 284 行 | ≈80 行 ✅ |
| 新增文件 | — | 2 个 |
| 修饰键状态 | 混在翻译器里 | 独立文件 |
| 查表匹配 | 散在各方法里 | 集中到 Resolver |

---

## 风险评估

| 风险点 | 影响 | 措施 |
|--------|------|------|
| _currentModifiers 移出 | 需要从文件 1 传到文件 2 | 文件 2 作为 `ModifierKeyState` 类，文件 1 持有它的实例 |
| _snapshot.Resolve 调用 | 散在多个方法里 | 集中到 Resolver，文件 1 持有 Resolver 实例 |
| OnRawMouseWheel 使用 `packedModifiers` | 特殊：不从 _currentModifiers 读 | 不含修饰键状态耦合，不受拆分影响 |
| Debug tracing | 各处 Debug.WriteLine | 可提取到共享 Trace 方法，也可保留在各文件中 |
| 外部调用方 | 多个 OnRaw* 方法 | 方法签名不变，外部不受影响 |

**结论：可以安全拆分，不影响输入行为。**

---

## 实施建议

```
先拆 ModifierKeyState（纯提取，不重构）
再拆 InputBindingResolver（纯提取，不重构）
主文件降到 80 行 → 从白名单删除
```

建议在 **8.7.8A-2** 执行拆分，**8.7.8A-1 只做审计不动代码**。
