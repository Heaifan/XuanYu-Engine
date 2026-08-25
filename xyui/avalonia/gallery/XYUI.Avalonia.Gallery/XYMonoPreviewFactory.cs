using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static class XYMonoPreviewFactory
{
    public static Control Create()
    {
        var mono = new XYMonoText();
        mono.Rows.Add(new("X 坐标", "142.583", "m"));
        mono.Rows.Add(new("Y 坐标", "-26.410", "m"));
        mono.Rows.Add(new("Z 坐标", "0.000", "m"));
        mono.Rows.Add(new("帧耗时", "16.67", "ms"));
        mono.Rows.Add(new("对象数", "1,284"));
        mono.Rows.Add(new("内存", "428.6", "MB"));
        return mono;
    }
}
