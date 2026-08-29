using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Vector;

public enum XyuiVectorIcon { Info, Error, Warning, Search, Locate, Browse, Copy, Code, Tag, StatusDot, Section, Empty, ChevronDown, Clear, Filter, Eye, Calendar, Clock, ChevronLeft, ChevronRight, ScrubLeftRight }

public static class XyuiVectorIcons
{
    public const double LogicalIconSize = 24d;
    public static bool IsPlatformReady => global::Avalonia.Application.Current is not null;
    public static IReadOnlyDictionary<XyuiVectorIcon, string> PathData { get; } =
        new Dictionary<XyuiVectorIcon, string>
        {
            [XyuiVectorIcon.Info] = "M12 3 C7.029 3 3 7.029 3 12 C3 16.971 7.029 21 12 21 C16.971 21 21 16.971 21 12 C21 7.029 16.971 3 12 3 Z M12 10 V17 M12 7 V8",
            [XyuiVectorIcon.Error] = "M12 3 C7.029 3 3 7.029 3 12 C3 16.971 7.029 21 12 21 C16.971 21 21 16.971 21 12 C21 7.029 16.971 3 12 3 Z M9 9 L15 15 M15 9 L9 15",
            [XyuiVectorIcon.Warning] = "M12 3 L22 20 H2 Z M12 9 V14 M12 17 V17.5",
            [XyuiVectorIcon.Search] = "M10.5 4.5 C7.186 4.5 4.5 7.186 4.5 10.5 C4.5 13.814 7.186 16.5 10.5 16.5 C13.814 16.5 16.5 13.814 16.5 10.5 C16.5 7.186 13.814 4.5 10.5 4.5 Z M15 15 L21 21",
            [XyuiVectorIcon.Locate] = "M12 3 V7 M12 17 V21 M3 12 H7 M17 12 H21 M12 7 C9.239 7 7 9.239 7 12 C7 14.761 9.239 17 12 17 C14.761 17 17 14.761 17 12 C17 9.239 14.761 7 12 7 Z",
            [XyuiVectorIcon.Browse] = "M3 6 H9 L11 8 H21 V20 H3 Z M3 6 V4 H10 L12 6",
            [XyuiVectorIcon.Copy] = "M8 8 H20 V20 H8 Z M4 4 H16 V16 H4 Z",
            [XyuiVectorIcon.Code] = "M9 6 L3 12 L9 18 M15 6 L21 12 L15 18 M13 4 L11 20",
            [XyuiVectorIcon.Tag] = "M0 11 L11 0 H24 V22 H11 Z",
            [XyuiVectorIcon.StatusDot] = "M12 3 C16.971 3 21 7.029 21 12 C21 16.971 16.971 21 12 21 C7.029 21 3 16.971 3 12 C3 7.029 7.029 3 12 3 Z",
            [XyuiVectorIcon.Section] = "M3 2 H7 V22 H3 Z",
            [XyuiVectorIcon.Empty] = "M3 12 H21",
            [XyuiVectorIcon.ChevronDown] = "M6 9 L12 15 L18 9",
            [XyuiVectorIcon.Clear] = "M5 5 L19 19 M19 5 L5 19",
            [XyuiVectorIcon.Filter] = "M3 5 H21 L14 13 V19 L10 21 V13 Z",
            [XyuiVectorIcon.Eye] = "M2.5 12 C5.2 7.5 8.6 5.5 12 5.5 C15.4 5.5 18.8 7.5 21.5 12 C18.8 16.5 15.4 18.5 12 18.5 C8.6 18.5 5.2 16.5 2.5 12 Z M9.2 12 C9.2 10.45 10.45 9.2 12 9.2 C13.55 9.2 14.8 10.45 14.8 12 C14.8 13.55 13.55 14.8 12 14.8 C10.45 14.8 9.2 13.55 9.2 12 Z",
            [XyuiVectorIcon.Calendar] = "M5 4 H19 V21 H5 Z M8 2 V6 M16 2 V6 M5 9 H19 M8 12 H10 M12 12 H14 M16 12 H18 M8 16 H10 M12 16 H14 M16 16 H18",
            [XyuiVectorIcon.Clock] = "M12 3 C7.029 3 3 7.029 3 12 C3 16.971 7.029 21 12 21 C16.971 21 21 16.971 21 12 C21 7.029 16.971 3 12 3 Z M12 7 V12 L16 15",
            [XyuiVectorIcon.ChevronLeft] = "M15 6 L9 12 L15 18",
            [XyuiVectorIcon.ChevronRight] = "M9 6 L15 12 L9 18",
            [XyuiVectorIcon.ScrubLeftRight] = "M8 7 L3 12 L8 17 M16 7 L21 12 L16 17 M4 12 H20"
        };

    public static StreamGeometry Create(XyuiVectorIcon icon) => StreamGeometry.Parse(PathData[icon]);

    public static ResourceDictionary CreateResources()
    {
        var resources = new ResourceDictionary();
        foreach (var icon in PathData.Keys) resources[$"XY.Icon.{icon}"] = Create(icon);
        return resources;
    }
}
