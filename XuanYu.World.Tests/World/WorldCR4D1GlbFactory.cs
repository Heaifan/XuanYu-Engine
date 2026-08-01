using System.Buffers.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace XuanYu.World.Tests.World;

static class WorldCR4D1GlbFactory
{
    public static byte[] Triangle(
        bool indices = true,
        bool uvs = true,
        int indexComponent = 5123,
        float[]? translation = null,
        int mode = 4,
        JsonObject? extraRoot = null)
    {
        var bin = new List<byte>();
        var views = new JsonArray();
        var accessors = new JsonArray();
        var attrs = new JsonObject
        {
            ["POSITION"] = AddAccessor(bin, views, accessors, Floats([0, 0, 0, 2, 0, 0, 0, 3, 0]), 5126, "VEC3", 3),
            ["NORMAL"] = AddAccessor(bin, views, accessors, Floats([0, 0, 1, 0, 0, 1, 0, 0, 1]), 5126, "VEC3", 3)
        };
        if (uvs) attrs["TEXCOORD_0"] = AddAccessor(bin, views, accessors, Floats([0, 0, 1, 0, 0, 1]), 5126, "VEC2", 3);
        var prim = new JsonObject { ["attributes"] = attrs, ["mode"] = mode };
        if (indices) prim["indices"] = AddAccessor(bin, views, accessors, IndexBytes([0, 1, 2], indexComponent), indexComponent, "SCALAR", 3);
        var node = new JsonObject { ["mesh"] = 0 };
        if (translation is not null) node["translation"] = Arr(translation);
        return Build(views, accessors, [prim], [node], bin.ToArray(), extraRoot);
    }

    public static byte[] MultiPrimitive()
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
        return Build(views, accessors, [Prim(0, 0), Prim(3, 1)], [new JsonObject { ["mesh"] = 0 }], bin.ToArray(),
            new JsonObject { ["materials"] = new JsonArray(Mat(1, 0, 0), Mat(0, 1, 0)) });
    }

    public static byte[] InvalidHeader() => Encoding.ASCII.GetBytes("not a glb");

    public static byte[] WithRoot(JsonObject root) => Build([], [], [], [], [], root);

    static byte[] Build(JsonArray views, JsonArray accessors, JsonObject[] prims, JsonObject[] nodes, byte[] bin, JsonObject? extra)
    {
        var root = extra ?? [];
        root["asset"] = new JsonObject { ["version"] = "2.0" };
        root["buffers"] = new JsonArray(new JsonObject { ["byteLength"] = bin.Length });
        root["bufferViews"] = views;
        root["accessors"] = accessors;
        root["meshes"] = new JsonArray(new JsonObject { ["primitives"] = new JsonArray(prims) });
        root["nodes"] = new JsonArray(nodes);
        root["scenes"] = new JsonArray(new JsonObject { ["nodes"] = new JsonArray(Enumerable.Range(0, nodes.Length).Select(x => JsonValue.Create(x)).ToArray()) });
        root["scene"] = 0;
        return Pack(JsonSerializer.SerializeToUtf8Bytes(root), bin);
    }

    static int AddAccessor(List<byte> bin, JsonArray views, JsonArray accessors, byte[] data, int component, string type, int count)
    {
        var offset = Align(bin.Count);
        while (bin.Count < offset) bin.Add(0);
        bin.AddRange(data);
        views.Add(new JsonObject { ["buffer"] = 0, ["byteOffset"] = offset, ["byteLength"] = data.Length });
        accessors.Add(new JsonObject { ["bufferView"] = views.Count - 1, ["componentType"] = component, ["count"] = count, ["type"] = type });
        return accessors.Count - 1;
    }

    static byte[] Floats(float[] values) => values.SelectMany(BitConverter.GetBytes).ToArray();
    static byte[] IndexBytes(uint[] v, int c) => c == 5125 ? v.SelectMany(x => BitConverter.GetBytes(x)).ToArray() : v.SelectMany(x => BitConverter.GetBytes((ushort)x)).ToArray();
    static JsonArray Arr(float[] values) => new(values.Select(x => JsonValue.Create(x)).ToArray());
    static JsonObject Mat(double r, double g, double b) => new() { ["pbrMetallicRoughness"] = new JsonObject { ["baseColorFactor"] = new JsonArray(r, g, b, 1) } };
    static int Align(int value) => (value + 3) & ~3;
    static byte[] Pack(byte[] json, byte[] bin) { var jp = Pad(json, 0x20); var bp = Pad(bin, 0); var total = 12 + 8 + jp.Length + 8 + bp.Length; var dst = new byte[total]; W(dst, 0, 0x46546C67); W(dst, 4, 2); W(dst, 8, total); W(dst, 12, jp.Length); W(dst, 16, 0x4E4F534A); jp.CopyTo(dst, 20); var o = 20 + jp.Length; W(dst, o, bp.Length); W(dst, o + 4, 0x004E4942); bp.CopyTo(dst, o + 8); return dst; }
    static byte[] Pad(byte[] data, byte pad) { var len = Align(data.Length); Array.Resize(ref data, len); for (var i = data.Length; i < len; i++) data[i] = pad; return data; }
    static void W(byte[] data, int offset, int value) => BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(offset, 4), (uint)value);
}
