using Avalonia.Styling;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Interaction;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Theme;

public static class XYUIBootstrap
{
    public static Styles Create()
    {
        var styles = new Styles();
        
        // 1. 统一加载 Token (Light/Dark Theme Dictionaries)
        styles.Resources.MergedDictionaries.Add(XyuiTheme.CreateThemeDictionaries());
        
        // 2. 统一加载 Typography
        styles.AddRange(XyuiTextStyles.Create());
        
        // 3. 统一加载 Spatial
        styles.AddRange(XyuiShapeStyles.Create());
        
        // 4. 统一加载 Interaction
        styles.AddRange(XyuiInteractionStyles.Create());
        
        // 5. 统一加载 Component Styles (XYUI-1, XYUI-3)
        styles.AddRange(XyuiComponentStyles.Create());
        
        // 6. 统一加载 Control Styles (XYUI-2)
        styles.AddRange(XyuiControlStyles.Create());
        
        return styles;
    }
}