using System.Collections.Generic;
using Avalonia.Styling;
using XYUI.Avalonia.Interaction;

namespace XYUI.Avalonia.Interaction;

// 焦点语义：仅占用边框环，与 Hover（底色）/ Selected（选中环）视觉分离。
// Focus 不污染背景，确保 Hover / Selected / Focus 三态可同时成立且互不覆盖。
public static class XyuiFocusStyles
{
    public static IEnumerable<Style> Create()
    {
        yield return XyuiInteractionState.Build("xyui-focusable", XyuiInteractionState.Focused,
            XyuiInteractionState.BorderBrushProperty, XyuiInteractionState.FocusBorderBrush);
        yield return XyuiInteractionState.Build("xyui-focusable", XyuiInteractionState.Focused,
            XyuiInteractionState.BorderThicknessProperty, XyuiInteractionState.FocusWidth);
    }
}
