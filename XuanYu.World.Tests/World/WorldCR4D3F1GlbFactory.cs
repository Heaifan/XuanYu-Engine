using System.Text.Json.Nodes;

namespace XuanYu.World.Tests.World;

// D3-F1：确定性多 Primitive / 越界索引测试 GLB 工厂（5+100 拆分自 WorldCR4D1GlbFactory）。
static partial class WorldCR4D1GlbFactory
{
    public static byte[] ThreePrimitives()
    {
        var bin = new List<byte>();
        var views = new JsonArray();
        var accessors = new JsonArray();
        JsonObject Prim(float x, int mat)
        {
            var attrs = new JsonObject
            {
                ["POSITION"] = AddAccessor(bin, views, accessors, Floats([x, 0, 0, x + 1, 0, 0, x, 1, 0]), 5126, "VEC3", 3),
                ["NORMAL"] = AddAccessor(bin, views, accessors, Floats([0, 0, 1, 0, 0, 1, 0, 0, 1]), 5126, "VEC3", 3)
            };
            return new JsonObject { ["attributes"] = attrs, ["indices"] = AddAccessor(bin, views, accessors, IndexBytes([0, 1, 2], 5125), 5125, "SCALAR", 3), ["material"] = mat };
        }
        return Build(views, accessors, [Prim(0, 0), Prim(3, 1), Prim(6, 2)],
            [new JsonObject { ["mesh"] = 0 }], bin.ToArray(),
            new JsonObject { ["materials"] = new JsonArray(Mat(1, 0, 0), Mat(0, 1, 0), Mat(0, 0, 1)) });
    }

    public static byte[] BadIndexTriangle()
    {
        var bin = new List<byte>();
        var views = new JsonArray();
        var accessors = new JsonArray();
        var attrs = new JsonObject
        {
            ["POSITION"] = AddAccessor(bin, views, accessors, Floats([0, 0, 0, 2, 0, 0, 0, 3, 0]), 5126, "VEC3", 3),
            ["NORMAL"] = AddAccessor(bin, views, accessors, Floats([0, 0, 1, 0, 0, 1, 0, 0, 1]), 5126, "VEC3", 3)
        };
        var prim = new JsonObject { ["attributes"] = attrs, ["indices"] = AddAccessor(bin, views, accessors, IndexBytes([0, 1, 9], 5125), 5125, "SCALAR", 3) };
        return Build(views, accessors, [prim], [new JsonObject { ["mesh"] = 0 }], bin.ToArray(), null);
    }
}
