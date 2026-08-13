using System.Text.RegularExpressions;

namespace XuanYu.Editor.MapDocument;

public static partial class MapDatasetDocumentValidator
{
    static readonly Regex IdPattern = BuildIdPattern();

    public static MapDocumentResult<MapDatasetDocument> Validate(MapDatasetDocument? document)
    {
        if (document is null) return Fail("MissingDocument", "Dataset 文档为空。", "document");
        if (document.Format != MapDatasetDocument.CurrentFormat)
            return Fail("InvalidFormat", "Dataset format 不受支持。", "format");
        if (document.Version is not (MapDatasetDocument.LegacyVersion or "0.2.0" or MapDatasetDocument.CurrentVersion))
            return Fail("UnsupportedVersion", "Dataset version 不受支持。", "version");
        if (!IdPattern.IsMatch(document.Id))
            return Fail("InvalidId", "Dataset ID 不符合稳定标识规则。", "id");
        if (!MapDatasetTypes.IsKnown(document.Type))
            return Fail("InvalidType", "Dataset type 不在允许的类型之内。", "type");
        if (document.Features.IsDefault)
            return Fail("InvalidFeatures", "Dataset features 必须是数组。", "features");
        if (document.Version == MapDatasetDocument.LegacyVersion && !document.Features.IsEmpty)
            return Fail("InvalidFeatures", "0.1.0 Dataset features 必须为空数组。", "features");
        if (document.Version is "0.2.0" or MapDatasetDocument.CurrentVersion)
        {
            var features = document.Version == "0.2.0"
                ? MapRegionDatasetCodec.Validate(document.Type, document.Features)
                : document.Type == MapDatasetTypes.Road
                ? MapRoadDatasetCodec.Validate(document.Type, document.Features)
                : document.Type == MapDatasetTypes.Marker
                ? MapMarkerDatasetCodec.Validate(document.Type, document.Features)
                : MapRegionDatasetCodec.Validate(document.Type, document.Features);
            if (!features.Succeeded) return features;
        }
        return MapDocumentResult<MapDatasetDocument>.Ok(document);
    }

    [GeneratedRegex("^[a-z0-9][a-z0-9_-]{0,127}$", RegexOptions.CultureInvariant)]
    private static partial Regex BuildIdPattern();

    static MapDocumentResult<MapDatasetDocument> Fail(string code, string message, string detail) =>
        MapDocumentResult<MapDatasetDocument>.Fail(code, message, "Validate", detail);
}
