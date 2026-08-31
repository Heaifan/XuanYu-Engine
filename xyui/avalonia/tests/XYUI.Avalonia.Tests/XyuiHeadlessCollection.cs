using Xunit;

namespace XYUI.Avalonia.Tests;

// Headless 会话共享 collection：多个 Avalonia session 并行会冲突，必须串行
[CollectionDefinition("XyuiHeadless", DisableParallelization = true)]
public class XyuiHeadlessCollection;
