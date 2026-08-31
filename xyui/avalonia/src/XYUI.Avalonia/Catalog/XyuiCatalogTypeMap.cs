namespace XYUI.Avalonia.Catalog;

internal static class XyuiCatalogTypeMap
{
    public static readonly IReadOnlyDictionary<string, string> Types = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["XYUI0-0.2"] = "XYUI.Avalonia.Foundation.XyuiColorTokens", ["XYUI0-0.3"] = "XYUI.Avalonia.Typography.XyuiTypography", ["XYUI0-0.3-C"] = "XYUI.Avalonia.Typography.XyuiTypographyTokens",
        ["XYUI0-0.6"] = "XYUI.Avalonia.Spatial.XyuiSpatialTokens", ["XYUI0-0.9"] = "XYUI.Avalonia.Spatial.XyuiSpatialTokens", ["XYUI0-0.20"] = "XYUI.Avalonia.Interaction.XyuiInteractionState",
        ["XYUI-1-01"] = "XYUI.Avalonia.Controls.XYText", ["XYUI-1-02"] = "XYUI.Avalonia.Controls.XYLabel", ["XYUI-1-03"] = "XYUI.Avalonia.Controls.XYCaption", ["XYUI-1-04"] = "XYUI.Avalonia.Controls.XYHeading",
        ["XYUI-1-05"] = "XYUI.Avalonia.Controls.XYSectionTitle", ["XYUI-1-06"] = "XYUI.Avalonia.Controls.XYLink", ["XYUI-1-07"] = "XYUI.Avalonia.Controls.XYCodeText", ["XYUI-1-08"] = "XYUI.Avalonia.Controls.XYMonoText",
        ["XYUI-1-09"] = "XYUI.Avalonia.Controls.XYBadge", ["XYUI-1-10"] = "XYUI.Avalonia.Controls.XYStatusBadge", ["XYUI-1-11"] = "XYUI.Avalonia.Controls.XYStatusDot", ["XYUI-1-12"] = "XYUI.Avalonia.Controls.XYIcon",
        ["XYUI-1-13"] = "XYUI.Avalonia.Controls.XYIconLabel", ["XYUI-1-14"] = "XYUI.Avalonia.Controls.XYSeparator", ["XYUI-1-15"] = "XYUI.Avalonia.Controls.XYHelpText", ["XYUI-1-16"] = "XYUI.Avalonia.Controls.XYErrorText",
        ["XYUI-1-17"] = "XYUI.Avalonia.Controls.XYWarningText", ["XYUI-1-18"] = "XYUI.Avalonia.Controls.XYShortcutHint", ["XYUI-1-19"] = "XYUI.Avalonia.Controls.XYTooltip", ["XYUI-1-20"] = "XYUI.Avalonia.Controls.XYRichText",
        ["XYUI-1-21"] = "XYUI.Avalonia.Controls.XYSelectableText", ["XYUI-1-22"] = "XYUI.Avalonia.Controls.XYEmptyText", ["XYUI-1-23"] = "XYUI.Avalonia.Controls.XYSearchHighlight", ["XYUI-1-24"] = "XYUI.Avalonia.Controls.XYTruncatedText",
        ["XYUI-2-01"] = "XYUI.Avalonia.Controls.XYButton", ["XYUI-2-02"] = "XYUI.Avalonia.Controls.XYIconButton", ["XYUI-2-03"] = "XYUI.Avalonia.Controls.XYToggleButton", ["XYUI-2-04"] = "XYUI.Avalonia.Controls.XYSplitButton", ["XYUI-2-05"] = "XYUI.Avalonia.Controls.XYDropDownButton", ["XYUI-2-06"] = "XYUI.Avalonia.Controls.XYCheckbox", ["XYUI-2-07"] = "XYUI.Avalonia.Controls.XYRadioButton", ["XYUI-2-08"] = "XYUI.Avalonia.Controls.XYSwitch", ["XYUI-2-09"] = "XYUI.Avalonia.Controls.XYTextField", ["XYUI-2-10"] = "XYUI.Avalonia.Controls.XYNumberField", ["XYUI-2-11"] = "XYUI.Avalonia.Controls.XYSlider", ["XYUI-2-12"] = "XYUI.Avalonia.Controls.XYComboBox", ["XYUI-2-13"] = "XYUI.Avalonia.Controls.XYSelect", ["XYUI-2-14"] = "XYUI.Avalonia.Controls.XYTextArea", ["XYUI-2-15"] = "XYUI.Avalonia.Controls.XYSearchField", ["XYUI-2-16"] = "XYUI.Avalonia.Controls.XYPasswordField", ["XYUI-2-17"] = "XYUI.Avalonia.Controls.XYDatePicker", ["XYUI-2-18"] = "XYUI.Avalonia.Controls.XYTimePicker", ["XYUI-2-19"] = "XYUI.Avalonia.Controls.XYColorPicker", ["XYUI-2-20"] = "XYUI.Avalonia.Controls.XYBoolProperty", ["XYUI-2-21"] = "XYUI.Avalonia.Controls.XYNumberProperty", ["XYUI-2-22"] = "XYUI.Avalonia.Controls.XYVectorProperty", ["XYUI-2-23"] = "XYUI.Avalonia.Controls.XYEnumProperty", ["XYUI-2-24"] = "XYUI.Avalonia.Controls.XYReferenceProperty",
        ["XYUI-3-3.01"] = "XYUI.Avalonia.Controls.XYMenuBar", ["XYUI-3-3.02"] = "XYUI.Avalonia.Controls.XYMenu", ["XYUI-3-3.03"] = "XYUI.Avalonia.Controls.XYContextMenu", ["XYUI-3-3.04"] = "XYUI.Avalonia.Controls.XYSubMenu"
    };

    public static readonly IReadOnlySet<string> GalleryIds = new HashSet<string>(Types.Keys, StringComparer.Ordinal);
}
