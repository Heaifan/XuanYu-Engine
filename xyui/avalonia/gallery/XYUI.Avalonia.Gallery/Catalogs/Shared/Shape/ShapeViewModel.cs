using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

// Shape 规范页数据模型（x:DataType 编译绑定）
public sealed record ShapeViewModel(IReadOnlyList<ShapeSection> Sections);
