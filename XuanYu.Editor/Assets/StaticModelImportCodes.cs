namespace XuanYu.Editor.Assets;

public enum StaticModelImportErrorCode
{
    None,
    SourceNotFound,
    SourceUnreadable,
    EmptyFile,
    InvalidGlbHeader,
    UnsupportedGlbVersion,
    MissingJsonChunk,
    MissingBinaryChunk,
    MalformedJson,
    BufferOutOfRange,
    MissingPosition,
    MissingNormal,
    InvalidAccessor,
    UnsupportedComponentType,
    InvalidIndex,
    NoRenderablePrimitive,
    NonFiniteVertex,
    InvalidTransform,
    UnsupportedPrimitiveMode,
    UnsupportedRequiredExtension,
    SkinnedMeshNotSupported,
    CompressedMeshNotSupported,
    ParserFailure,
    UnexpectedImportFailure
}

public enum StaticModelImportWarningCode
{
    AnimationNotImported,
    MorphTargetNotImported,
    MissingUvUsedDefault,
    PrimitiveIgnored,
    MaterialLimited,
    TextureNotImported,
    CameraNotImported,
    LightNotImported
}
