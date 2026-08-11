using System.Security.Cryptography;

namespace XuanYu.Editor.MapDocument;

public static class MapDatasetIdGenerator
{
    public const int MaxAttempts = 16;

    public static MapDocumentResult<string> Generate(
        string type, Func<string, bool> isTaken, Func<string>? suffixFactory = null)
    {
        if (!MapDatasetTypes.IsKnown(type))
            return MapDocumentResult<string>.Fail("InvalidDatasetType", "Dataset type 不受支持。", "Validate");
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var id = $"{type}-{(suffixFactory ?? RandomSuffix)()}";
            if (!isTaken(id)) return MapDocumentResult<string>.Ok(id);
        }
        return MapDocumentResult<string>.Fail(
            "DatasetIdGenerationExhausted", "Dataset ID 可用候选已耗尽，拒绝覆盖。", "Generate");
    }

    static string RandomSuffix()
    {
        Span<byte> bytes = stackalloc byte[3];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
