using System.Buffers.Binary;
using System.Text;
using System.Text.Json;

namespace XuanYu.Editor.Assets;

sealed record GlbContainer(JsonDocument Json, byte[] Binary);

static class GlbContainerReader
{
    const uint Magic = 0x46546C67;
    const uint JsonType = 0x4E4F534A;
    const uint BinType = 0x004E4942;

    public static StaticModelImportResult Read(byte[] bytes, out GlbContainer? container)
    {
        container = null;
        if (bytes.Length == 0) return Fail(StaticModelImportErrorCode.EmptyFile, "GLB 文件为空。");
        if (bytes.Length < 20 || U32(bytes, 0) != Magic)
            return Fail(StaticModelImportErrorCode.InvalidGlbHeader, "GLB 文件头无效。");
        if (U32(bytes, 4) != 2)
            return Fail(StaticModelImportErrorCode.UnsupportedGlbVersion, "仅支持 GLB 2.0。");
        if (U32(bytes, 8) != bytes.Length)
            return Fail(StaticModelImportErrorCode.InvalidGlbHeader, "GLB 长度字段无效。");
        return ReadChunks(bytes, out container);
    }

    static StaticModelImportResult ReadChunks(byte[] bytes, out GlbContainer? container)
    {
        container = null;
        JsonDocument? json = null;
        byte[] binary = [];
        for (var offset = 12; offset + 8 <= bytes.Length;)
        {
            var length = checked((int)U32(bytes, offset));
            var type = U32(bytes, offset + 4);
            offset += 8;
            if (length < 0 || offset + length > bytes.Length)
                return Fail(StaticModelImportErrorCode.BufferOutOfRange, "GLB Chunk 越界。");
            StaticModelImportResult? error = null;
            if (type == JsonType) json = ParseJson(bytes, offset, length, out error);
            if (type == BinType) binary = bytes.Skip(offset).Take(length).ToArray();
            if (json is null && type == JsonType && error is not null) return error;
            offset += Align4(length);
        }
        if (json is null) return Fail(StaticModelImportErrorCode.MissingJsonChunk, "缺少 GLB JSON Chunk。");
        container = new GlbContainer(json, binary);
        return StaticModelImportResult.Success(null!);
    }

    static JsonDocument? ParseJson(byte[] bytes, int offset, int length, out StaticModelImportResult? error)
    {
        error = null;
        try { return JsonDocument.Parse(Encoding.UTF8.GetString(bytes, offset, length).TrimEnd('\0', ' ')); }
        catch (JsonException ex)
        {
            error = Fail(StaticModelImportErrorCode.MalformedJson, "GLB JSON Chunk 损坏。", ex.Message);
            return null;
        }
    }

    static uint U32(byte[] bytes, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset, 4));
    static int Align4(int value) => (value + 3) & ~3;
    static StaticModelImportResult Fail(StaticModelImportErrorCode c, string m, string d = "") =>
        StaticModelImportResult.Fail(c, m, d);
}
