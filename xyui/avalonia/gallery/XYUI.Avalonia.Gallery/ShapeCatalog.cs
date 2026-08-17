namespace XYUI.Avalonia.Gallery;

// Shape 规范页数据（Spacing/Radius/Border/Elevation token 表）
public sealed record ShapeItem(string TokenId, string Value, string Note);

public sealed record ShapeSection(string Title, IReadOnlyList<ShapeItem> Items);

public static class ShapeCatalog
{
    public static IReadOnlyList<ShapeSection> BuildSections() =>
    [
        new("Spacing（4 DIP 基础单位）", [
            new("XY.Space.1", "4 DIP", "最小间距，字段行内"),
            new("XY.Space.2", "8 DIP", "Panel 内边距 / Section 间距"),
            new("XY.Space.3", "12 DIP", "控件间"),
            new("XY.Space.4", "16 DIP", "区块内分组"),
            new("XY.Space.6", "24 DIP", "大分组"),
            new("XY.Space.8", "32 DIP", "Section 之间"),
            new("XY.Space.10", "40 DIP", "页面级留白"),
            new("XY.Space.12", "48 DIP", "最大档"),
        ]),
        new("Radius（圆角表达角色，非装饰）", [
            new("XY.Radius.Panel", "0 DIP", "Panel 方正"),
            new("XY.Radius.Row", "0 DIP", "Tree Row 不做圆角卡片"),
            new("XY.Radius.Toolbar", "2 DIP", "极轻圆角"),
            new("XY.Radius.Control", "4 DIP", "Input/Button/Control"),
            new("XY.Radius.Popup", "6 DIP", "浮层稍圆"),
            new("XY.Radius.Full", "999 DIP", "Tag/Badge"),
        ]),
        new("Border（Container 用 Divider，Control 完整边框）", [
            new("XY.Border.Width.None", "0 DIP", "Container/Panel 无完整外框"),
            new("XY.Border.Width.Default", "1 DIP", "Input/Button/Control"),
            new("XY.Border.Width.Strong", "2 DIP", "关键结构"),
            new("XY.Border.Width.Focus", "2 DIP", "Focus 独立 Outline"),
            new("XY.Border.Width.Selected", "2 DIP", "Selected 独立"),
        ]),
        new("Elevation（阴影只表达 Z 轴脱离）", [
            new("XY.Shadow.None", "None", "Panel/Button/Input 不用阴影"),
            new("XY.Shadow.Tooltip", "0/3/10/0.12", "轻阴影"),
            new("XY.Shadow.Popup", "0/6/18/0.14", "中轻阴影"),
            new("XY.Shadow.DragPreview", "0/6/18/0.14", "拖拽预览略强"),
        ]),
    ];
}
