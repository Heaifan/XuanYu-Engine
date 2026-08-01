using System.Buffers.Binary;
using System.Text.Json;
using XuanYu.Core.Math;

namespace XuanYu.Editor.Assets;

sealed class GltfAccessorReader
{
    readonly JsonElement _root;
    readonly byte[] _bin;

    public GltfAccessorReader(JsonElement root, byte[] bin)
    {
        _root = root;
        _bin = bin;
    }

    public StaticModelImportResult ReadVec3(int accessorId, out IReadOnlyList<Vector3d> values)
    {
        values = [];
        var a = Accessor(accessorId);
        if (!ValidateAccessor(a, "VEC3", 5126, out var view, out var stride, out var start, out var count))
            return Fail(StaticModelImportErrorCode.InvalidAccessor, "VEC3 Accessor 无效。");
        values = Enumerable.Range(0, count)
            .Select(i => new Vector3d(F32(start + i * stride), F32(start + i * stride + 4), F32(start + i * stride + 8)))
            .ToArray();
        return StaticModelImportResult.Success(null!);
    }

    public StaticModelImportResult ReadVec2(int accessorId, out IReadOnlyList<StaticModelUv> values)
    {
        values = [];
        var a = Accessor(accessorId);
        if (!ValidateAccessor(a, "VEC2", 5126, out _, out var stride, out var start, out var count))
            return Fail(StaticModelImportErrorCode.InvalidAccessor, "VEC2 Accessor 无效。");
        values = Enumerable.Range(0, count)
            .Select(i => new StaticModelUv(F32(start + i * stride), F32(start + i * stride + 4)))
            .ToArray();
        return StaticModelImportResult.Success(null!);
    }

    public StaticModelImportResult ReadIndices(int accessorId, out IReadOnlyList<uint> values)
    {
        values = [];
        var a = Accessor(accessorId);
        var component = GltfJsonAccess.Int(a, "componentType");
        if (!ValidateIndexAccessor(a, component, out var stride, out var start, out var count))
            return Fail(StaticModelImportErrorCode.UnsupportedComponentType, "索引 Accessor 类型不支持。");
        values = Enumerable.Range(0, count).Select(i => ReadIndex(start + i * stride, component)).ToArray();
        return StaticModelImportResult.Success(null!);
    }

    JsonElement Accessor(int id) => GltfJsonAccess.At(_root, "accessors", id);

    bool ValidateAccessor(JsonElement a, string type, int component, out JsonElement view, out int stride, out int start, out int count)
    {
        view = default;
        count = GltfJsonAccess.Int(a, "count");
        var viewId = GltfJsonAccess.Int(a, "bufferView", -1);
        if (viewId < 0 || GltfJsonAccess.String(a, "type") != type || GltfJsonAccess.Int(a, "componentType") != component)
        { stride = start = 0; return false; }
        view = GltfJsonAccess.At(_root, "bufferViews", viewId);
        var elementSize = type == "VEC3" ? 12 : 8;
        stride = GltfJsonAccess.Int(view, "byteStride", elementSize);
        start = GltfJsonAccess.Int(view, "byteOffset") + GltfJsonAccess.Int(a, "byteOffset");
        return count > 0 && start >= 0 && start + ((count - 1) * stride) + elementSize <= _bin.Length;
    }

    bool ValidateIndexAccessor(JsonElement a, int component, out int stride, out int start, out int count)
    {
        count = GltfJsonAccess.Int(a, "count");
        var view = GltfJsonAccess.At(_root, "bufferViews", GltfJsonAccess.Int(a, "bufferView", -1));
        stride = component == 5125 ? 4 : component == 5123 ? 2 : component == 5121 ? 1 : 0;
        start = GltfJsonAccess.Int(view, "byteOffset") + GltfJsonAccess.Int(a, "byteOffset");
        return stride > 0 && count > 0 && start >= 0 && start + ((count - 1) * stride) + stride <= _bin.Length;
    }

    float F32(int offset) => BinaryPrimitives.ReadSingleLittleEndian(_bin.AsSpan(offset, 4));
    uint ReadIndex(int offset, int c) => c == 5125 ? BinaryPrimitives.ReadUInt32LittleEndian(_bin.AsSpan(offset, 4)) :
        c == 5123 ? BinaryPrimitives.ReadUInt16LittleEndian(_bin.AsSpan(offset, 2)) : _bin[offset];
    static StaticModelImportResult Fail(StaticModelImportErrorCode c, string m) => StaticModelImportResult.Fail(c, m);
}
