using System.Text.Json;

namespace XuanYu.Editor.Assets;

static class GltfJsonAccess
{
    public static JsonElement At(JsonElement root, string name, int index) =>
        root.GetProperty(name)[index];

    public static bool TryArray(JsonElement obj, string name, out JsonElement value) =>
        obj.TryGetProperty(name, out value) && value.ValueKind == JsonValueKind.Array;

    public static int Int(JsonElement obj, string name, int fallback = 0) =>
        obj.TryGetProperty(name, out var v) ? v.GetInt32() : fallback;

    public static string String(JsonElement obj, string name, string fallback = "") =>
        obj.TryGetProperty(name, out var v) ? v.GetString() ?? fallback : fallback;

    public static bool Has(JsonElement obj, string name) => obj.TryGetProperty(name, out _);

    public static IReadOnlyList<int> IntArray(JsonElement obj, string name)
    {
        if (!TryArray(obj, name, out var a)) return [];
        return a.EnumerateArray().Select(x => x.GetInt32()).ToArray();
    }

    public static IReadOnlyList<float> FloatArray(JsonElement obj, string name)
    {
        if (!TryArray(obj, name, out var a)) return [];
        return a.EnumerateArray().Select(x => x.GetSingle()).ToArray();
    }

    public static StaticModelColor BaseColor(JsonElement material)
    {
        if (!material.TryGetProperty("pbrMetallicRoughness", out var pbr) ||
            !TryArray(pbr, "baseColorFactor", out var color) ||
            color.GetArrayLength() != 4) return StaticModelColor.Neutral;
        return new StaticModelColor(
            color[0].GetDouble(), color[1].GetDouble(),
            color[2].GetDouble(), color[3].GetDouble());
    }
}
