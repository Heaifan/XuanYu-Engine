# 诊断日志与 UI 调度安全规范

## 一、背景

在 9.0D 调试 Move Gizmo 时，曾出现诊断代码导致 UI 调度器卡死的问题。根因是高频输入/启动组合阶段将诊断回调直接绑定到 UI 日志管道，导致 UI 线程被同步回调阻塞，窗口标题栏按钮、拖拽、日志面板全部失去响应。

本规范用于防止类似问题再次发生。

---

## 二、核心原则

诊断日志不得阻塞输入线程、渲染线程、UI 线程。

任何诊断代码都必须满足：

```
不等待 UI
不等待文件 IO
不使用同步 Dispatcher.Invoke
不使用 Task.Wait / Result
不在高频路径直接写文件
不在高频路径直接写 UI 日志面板
```

---

## 三、启动期规则

在以下阶段禁止绑定 UI 日志回调：

- EditorShellComposition.Build()
- 构造函数
- 服务装配阶段
- 窗口尚未 Loaded 前
- 项目尚未完成打开前

**禁止示例：**

```csharp
// 启动期将 UI 日志回调注入纯逻辑层
ctx.PointerRoute.Trace = msg => ctx.LogRoute.Info(msg);
```

原因：`LogRoute.Info` 可能依赖 UI 调度器、日志面板、缓冲队列或尚未初始化的组件。

**允许示例：**

```csharp
ctx.PointerRoute.Trace = null;
// 或
ctx.PointerRoute.Trace = msg => System.Diagnostics.Debug.WriteLine(msg);
```

---

## 四、高频路径规则

以下路径属于高频路径，任意路径都可能每秒触发数十到数百次：

- PointerMoved / MouseMove
- DragMove / 拖动求解
- RenderFrame / 帧提交
- Viewport Redraw
- Gizmo Hover / 命中检测
- Picking Hover

这些路径中**禁止直接调用**：

```
LogRoute.Info / Warn / Error
FeedbackRoute.Append
File.AppendAllText
Dispatcher.Invoke (同步)
Task.Wait / Task.Result
lock 后执行 UI / IO 操作
```

**允许调用：**

```
非阻塞 DiagnosticSink.TryWrite
System.Diagnostics.Debug.WriteLine
非阻塞 RingBuffer 写入
```

---

## 五、诊断 Sink 接口

诊断 Sink 必须使用非阻塞接口：

```csharp
public interface DiagnosticSink
{
    bool TryWrite(string category, string message);
}
```

要求：

```
TryWrite 不得 throw
TryWrite 不得阻塞
TryWrite 不得等待 UI
TryWrite 不得同步写文件
TryWrite 可以丢弃日志
TryWrite 可以合并重复日志
```

---

## 六、UI 日志输出规则

如果诊断信息需要显示到 UI 日志面板，必须经过异步投递：

```
DiagnosticSink.TryWrite
→ 内存队列 / RingBuffer
→ 节流器（每 100ms 最多刷新一次）
→ Dispatcher.UIThread.Post
→ 日志面板追加
```

禁止每条日志都调用一次 UI 调度。重复消息应合并计数。

---

## 七、文件日志规则

文件日志不得在高频路径中直接写入。

**禁止：**

```csharp
File.AppendAllText("trace.log", message);
```

**禁止（仍会造成 IO 风暴）：**

```csharp
Task.Run(() => File.AppendAllText("trace.log", message));
```

**推荐：**

```
内存队列 → 单一后台写入任务 → 批量 flush → 退出时安全关闭
```

---

## 八、Debug 与正式日志分离

诊断 Trace 与正式用户日志必须分离。

```
Trace：开发调试用，可丢弃，可节流，可只在 DEBUG 存在
Log：用户可见事件，不可随意丢弃，但不能在高频路径滥用
```

建议：

```csharp
#if DEBUG
diagnosticSink.TryWrite("TransformDrag", message);
#endif
```

严重错误和用户可见错误应走正式日志系统，不应被 `#if DEBUG` 完全移除。

---

## 九、代码审查清单

任何新增诊断代码必须通过以下检查：

```
1. 是否在启动期绑定 UI 回调？
2. 是否在 PointerMoved / DragMove 中写 UI 日志？
3. 是否有同步 Dispatcher.Invoke？
4. 是否有 File.AppendAllText？
5. 是否有 Task.Wait / Result？
6. 是否有未节流的日志风暴？
7. 是否有队列满后阻塞生产者？
8. 是否能在 UI 未初始化时安全运行？
```

只要有一项为「是」，不得合并。

---

## 十、修订记录

| 日期 | 修订人 | 说明 |
|---|---|---|
| 2026-06-24 | — | 初始版本，9.0D 诊断卡死事故后编定 |
