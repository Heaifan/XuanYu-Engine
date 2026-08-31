using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

// Typography 规范页数据模型（x:DataType 编译绑定需要具名类型）
public sealed record TypographyViewModel(IReadOnlyList<TypographySection> Sections);
