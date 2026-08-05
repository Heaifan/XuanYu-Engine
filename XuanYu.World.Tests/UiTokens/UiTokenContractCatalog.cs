// Token 合同清单（ARCH-UI-SPEC-R1-D2）。
// 依据：docs/ui/玄域引擎_UI规范_1.0.md（UI Spec 1.0）§3/§4/§5/§6/§8/§9/§13/§15。
// 用途：Token 合同测试的双向核对——代码 Token 键集合 == 本合同清单（无缺失、无额外）。
// 规范变更必须同步更新本合同清单（规范 §21 变更流程）。

namespace XuanYu.World.Tests.UiTokens;

public static class UiTokenContractCatalog
{
    public static readonly string[] Keys =
    [
        // 字体（§3.1/§3.2/§3.4）
        "Font.Family",
        "Font.Meta.Size", "Font.Meta.LineHeight", "Font.Small.Size", "Font.Small.LineHeight",
        "Font.Label.Size", "Font.Label.LineHeight", "Font.Body.Size", "Font.Body.LineHeight",
        "Font.Section.Size", "Font.Section.LineHeight", "Font.Title.Size", "Font.Title.LineHeight",
        "Font.Page.Size", "Font.Page.LineHeight", "Font.Display.Size", "Font.Display.LineHeight",
        "Font.Weight.Regular", "Font.Weight.Medium", "Font.Weight.SemiBold", "Font.Weight.Bold",
        // 背景与语义色（§4.1/§4.2）
        "Color.Bg.Application", "Color.Bg.Panel", "Color.Bg.Control", "Color.Bg.Overlay",
        "Color.Border.Default", "Color.Border.Strong",
        "Color.Text.Primary", "Color.Text.Secondary", "Color.Text.Disabled",
        "Color.Accent", "Color.Accent.Hover", "Color.Selection.Bg", "Color.Hover.Bg", "Color.Focus",
        "Color.Success", "Color.Warning", "Color.Error", "Color.Danger",
        "Color.Object.System", "Color.Object.User",
        // 日志组件色（§4.3）
        "Log.Accent.Error", "Log.Accent.Warning", "Log.Accent.Info", "Log.Accent.Debug",
        "Log.Accent.Trace", "Log.RepeatText",
        // 文档状态组件色（§4.4）
        "DocStatus.SuccessBg", "DocStatus.SuccessBorder", "DocStatus.SuccessText",
        "DocStatus.WarningBg", "DocStatus.WarningBorder", "DocStatus.WarningText",
        "DocStatus.ErrorBg", "DocStatus.ErrorBorder", "DocStatus.ErrorText",
        "DocStatus.SaveHighlightBg", "DocStatus.SaveHighlightBorder",
        // 图层组件色（§12.2 + D4-F3 合同收敛）
        "Layer.Kind.Region.Bg", "Layer.Kind.Region.Border", "Layer.Kind.Region.Text",
        "Layer.Kind.System.Bg", "Layer.Kind.System.Border", "Layer.Kind.System.Text",
        "Layer.State.Visible", "Layer.State.Hidden", "Layer.State.Locked", "Layer.State.Unlocked",
        "Layer.State.VisibleBg", "Layer.State.LockedBg", "Layer.DropLine",
        "Tree.Guide",
        // 间距/内边距/圆角（§5.1/§5.2/§5.4）
        "Space.2", "Space.4", "Space.6", "Space.8", "Space.12", "Space.16", "Space.24", "Space.32",
        "Padding.Compact", "Padding.Standard", "Padding.Relaxed",
        "Radius.Small", "Radius.Standard", "Radius.Large",
        // 控件尺寸（§5.3/§6.1/§6.4/§9）
        "Control.Height.Compact", "Control.Height.Standard", "Control.Height.Emphasized",
        "Size.Width.64", "Size.Width.96", "Size.Width.128", "Size.Width.160", "Size.Width.240",
        "Control.LabelColumn.Width", "Control.Field.MinWidth",
        "Size.Hit.Compact", "Size.Hit.Standard", "Size.Hit.Touch",
        "Border.Width.Default", "Border.Width.Insert", "Border.Width.Focus", "Focus.Offset",
        "Shadow.OffsetY", "Shadow.Blur", "Shadow.Opacity",
        "LogTable.Columns",
        // 图标（§8.1）
        "Icon.Size.Standard", "Icon.Size.Tool", "Icon.Stroke.Width",
        // 动效（§15.3）
        "Motion.HoverMs", "Motion.ExpandMs",
    ];
}
