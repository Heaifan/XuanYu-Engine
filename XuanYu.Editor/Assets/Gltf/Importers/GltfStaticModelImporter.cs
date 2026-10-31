using System.Numerics;
using System.Text.Json;
namespace XuanYu.Editor.Assets;
sealed class GltfStaticModelImporter
{
    readonly JsonElement _root;
    readonly string _displayName;
    readonly int _sourceBytes;
    readonly GltfAccessorReader _accessors;
    readonly StaticModelBuilder _builder = new();
    readonly StaticModelWarningSet _warnings = new();
    public GltfStaticModelImporter(GlbContainer container, string displayName, int sourceBytes)
    {
        _root = container.Json.RootElement;
        _displayName = displayName;
        _sourceBytes = sourceBytes;
        _accessors = new GltfAccessorReader(_root, container.Binary);
    }
    public StaticModelImportResult Import()
    {
        try
        {
            AddGlobalWarnings();
            foreach (var nodeId in SceneNodeIds()) ImportNode(nodeId, Matrix4x4.Identity);
            if (_builder.IndexCount == 0)
                return Fail(StaticModelImportErrorCode.NoRenderablePrimitive, "GLB 没有可导入的三角形 Primitive。");
            return StaticModelImportResult.Success(_builder.Build(_displayName, _sourceBytes, _warnings.ToList()));
        }
        catch (ImportStop stop)
        {
            return Fail(stop.Code, stop.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Fail(StaticModelImportErrorCode.NonFiniteVertex, ex.Message);
        }
        catch (Exception ex)
        {
            return Fail(StaticModelImportErrorCode.UnexpectedImportFailure, ex.Message);
        }
    }
    void ImportNode(int nodeId, Matrix4x4 parent)
    {
        var node = GltfJsonAccess.At(_root, "nodes", nodeId);
        var world = GltfNodeTransform.WorldMatrix(node, parent);
        if (GltfJsonAccess.Has(node, "camera")) _warnings.Add(StaticModelImportWarningCode.CameraNotImported, "GLB 相机未导入。");
        if (GltfJsonAccess.Has(node, "skin")) throw new ImportStop(StaticModelImportErrorCode.SkinnedMeshNotSupported, "D1 不支持蒙皮模型。");
        if (GltfJsonAccess.Has(node, "mesh")) ImportMesh(GltfJsonAccess.Int(node, "mesh"), world);
        foreach (var child in GltfJsonAccess.IntArray(node, "children")) ImportNode(child, world);
    }
    void ImportMesh(int meshId, Matrix4x4 world)
    {
        var mesh = GltfJsonAccess.At(_root, "meshes", meshId);
        foreach (var primitive in mesh.GetProperty("primitives").EnumerateArray())
        {
            if (GltfJsonAccess.Int(primitive, "mode", 4) != 4)
            { _warnings.Add(StaticModelImportWarningCode.PrimitiveIgnored, "非 TRIANGLES Primitive 已忽略。"); continue; }
            ImportPrimitive(primitive, world);
        }
    }

    void ImportPrimitive(JsonElement primitive, Matrix4x4 world)
    {
        var attributes = primitive.GetProperty("attributes");
        if (!attributes.TryGetProperty("POSITION", out var p)) throw new ImportStop(StaticModelImportErrorCode.MissingPosition, "Primitive 缺少 POSITION。");
        var pos = ReadVec3(p.GetInt32());
        var normal = attributes.TryGetProperty("NORMAL", out var n) ? ReadVec3(n.GetInt32()) : throw new ImportStop(StaticModelImportErrorCode.MissingNormal, "Primitive 缺少 NORMAL。");
        var uvs = attributes.TryGetProperty("TEXCOORD_0", out var uv) ? ReadUv(uv.GetInt32()) : DefaultUvs(pos.Count);
        if (!attributes.TryGetProperty("TEXCOORD_0", out _)) _warnings.Add(StaticModelImportWarningCode.MissingUvUsedDefault, "缺少 UV，已使用默认 UV。");
        var indices = GltfJsonAccess.Has(primitive, "indices") ? ReadIndices(GltfJsonAccess.Int(primitive, "indices")) : Sequential(pos.Count);
        var vertices = BuildVertices(pos, normal, uvs, world);
        if (indices.Any(i => i >= vertices.Count)) throw new ImportStop(StaticModelImportErrorCode.InvalidIndex, "索引引用不存在的顶点。");
        _builder.AddPrimitive(vertices, indices, ColorOf(primitive));
    }

    IReadOnlyList<int> SceneNodeIds()
    {
        var scene = GltfJsonAccess.Int(_root, "scene", 0);
        return GltfJsonAccess.IntArray(GltfJsonAccess.At(_root, "scenes", scene), "nodes");
    }

    void AddGlobalWarnings()
    {
        if (GltfJsonAccess.TryArray(_root, "animations", out _)) _warnings.Add(StaticModelImportWarningCode.AnimationNotImported, "动画数据未导入。");
    }

    IReadOnlyList<StaticModelVertex> BuildVertices(IReadOnlyList<XuanYu.Core.Math.Vector3d> p, IReadOnlyList<XuanYu.Core.Math.Vector3d> n, IReadOnlyList<StaticModelUv> uv, Matrix4x4 w) =>
        p.Select((x, i) => new StaticModelVertex(GltfNodeTransform.Position(new Vector3((float)x.X, (float)x.Y, (float)x.Z), w),
            GltfNodeTransform.Normal(new Vector3((float)n[i].X, (float)n[i].Y, (float)n[i].Z), w, out var nn) ? nn : throw new ImportStop(StaticModelImportErrorCode.InvalidTransform, "法线矩阵不可逆。"), uv[i])).ToArray();

    IReadOnlyList<XuanYu.Core.Math.Vector3d> ReadVec3(int id) => _accessors.ReadVec3(id, out var v).Succeeded ? v : throw new ImportStop(StaticModelImportErrorCode.InvalidAccessor, "VEC3 读取失败。");
    IReadOnlyList<StaticModelUv> ReadUv(int id) => _accessors.ReadVec2(id, out var v).Succeeded ? v : throw new ImportStop(StaticModelImportErrorCode.InvalidAccessor, "UV 读取失败。");
    IReadOnlyList<uint> ReadIndices(int id) => _accessors.ReadIndices(id, out var v).Succeeded ? v : throw new ImportStop(StaticModelImportErrorCode.InvalidAccessor, "索引读取失败。");
    static IReadOnlyList<StaticModelUv> DefaultUvs(int count) => Enumerable.Repeat(StaticModelUv.Zero, count).ToArray();
    static IReadOnlyList<uint> Sequential(int count) => Enumerable.Range(0, count).Select(x => (uint)x).ToArray();
    StaticModelColor ColorOf(JsonElement p) => GltfJsonAccess.Has(p, "material") && GltfJsonAccess.TryArray(_root, "materials", out _) ? GltfJsonAccess.BaseColor(GltfJsonAccess.At(_root, "materials", GltfJsonAccess.Int(p, "material"))) : StaticModelColor.Neutral;
    static StaticModelImportResult Fail(StaticModelImportErrorCode c, string m) => StaticModelImportResult.Fail(c, m);
}
