/// Minimal SPIR-V generator for FluidWarfare basic 3D shaders.

// Navigate up from bin/Debug/net10.0/ to repo root
var repoRoot = Path.GetFullPath(Path.Combine(
    AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
var shadersDir = Path.Combine(repoRoot, "FluidWarfare.Render.Vulkan", "Shaders");
var compiledDir = Path.Combine(shadersDir, "Compiled");
Directory.CreateDirectory(compiledDir);

var vert = BuildVertex();
var frag = BuildFragment();

File.WriteAllBytes(Path.Combine(compiledDir, "basic_3d.vert.spv"), vert);
File.WriteAllBytes(Path.Combine(compiledDir, "basic_3d.frag.spv"), frag);

Console.WriteLine("SPIR-V generated:");
Console.WriteLine($"  basic_3d.vert.spv: {vert.Length} bytes");
Console.WriteLine($"  basic_3d.frag.spv: {frag.Length} bytes");

static byte[] BuildVertex()
{
    var w = new SW();
    w.H(34); // bound (max id + 1 = 33+1)
    // IDs used: 1=ext, 2=void, 3=func, 4=float, 5=v4, 6=v3,
    // 7=struct{mat4}, 8=mat4, 9=ptr(pc,mat4), 10=ptr(pc,struct{mat4}),
    // 11=ptr(in,v3), 12=ptr(in,v4), 13=inPosVar, 14=ptr(out,v4),
    // 15=inColorVar, 16=ptr(out,v4), 17=outColorVar, 18=glPosVar,
    // 19=int, 20=ptr(pc,mat4), 21=float1, 22=int0,
    // 23=func, 24=pcVar, 25=label, 26=ac, 27=mvp, 28=pos, 29=ext,
    // 30=result, 31=color, 32=retlabel

    w.Cap(1);         // Shader
    w.Ext(1, "GLSL.std.450");
    w.MM(0, 1);       // Logical, GLSL450
    w.EP(0, 23, "main", [13, 15, 17, 18]); // Vertex

    w.N(23, "main");
    w.N(13, "inPosition");
    w.N(15, "inColor");
    w.N(17, "outColor");
    w.N(18, "gl_Position");

    w.D(13, 30, 0);    // Location 0
    w.D(15, 30, 1);    // Location 1
    w.D(17, 30, 0);    // Location 0
    w.D(18, 11, 0);    // BuiltIn Position

    w.MD(7, 0, 5, 0);   // ColMajor
    w.MD(7, 0, 35, 0);  // Offset 0
    w.MD(7, 0, 7, 16);  // MatrixStride 16
    w.D(10, 2);          // Block (on the struct pointer variable %10... wait)
    // Actually Block decoration goes on the struct type, not the pointer
    w.D(7, 2);            // Block on struct type %7

    // Types
    w.TV(2);
    w.TF(3, 2);
    w.TFloat(4, 32);
    w.TVec(5, 4, 4);       // vec4
    w.TVec(6, 4, 3);       // vec3
    w.TMat(8, 5, 4);       // mat4 = 4 x vec4
    w.TStr(7, [8]);        // struct { mat4 mvp }
    w.TPtr(9, 9, 7);       // ptr(pushconst, struct) - storage class 9
    w.TPtr(11, 1, 6);      // ptr(input, vec3)
    w.TPtr(12, 1, 5);      // ptr(input, vec4)
    w.TPtr(14, 2, 5);      // ptr(output, vec4)
    w.TPtr(16, 2, 5);      // ptr(output, vec4)
    w.TInt(19, 32, 1);     // signed int32
    w.TPtr(20, 9, 8);      // ptr(pushconst, mat4) - for access chain

    // Const
    w.Const(21, 4, 0x3F800000); // float 1.0
    w.Const(22, 19, 0);         // int 0

    // Variables
    w.Var(24, 9, 9);   // push constant
    w.Var(13, 11, 1);  // inPosition (input)
    w.Var(15, 12, 1);  // inColor (input)
    w.Var(17, 14, 2);  // outColor (output)
    w.Var(18, 16, 2);  // gl_Position (output)

    // Main function
    w.Func(23, 2, 0, 3);
    w.Lab(25);

    w.AC(26, 20, 24, 22);   // %26 = accesschain ptr %24[%22]
    w.Ld(27, 8, 26);         // %27 = load %8 %26
    w.Ld(28, 6, 13);         // %28 = load %6 %13
    w.CC(29, 5, [28, 21]);   // %29 = vec4(%28, %21)
    w.MTV(30, 5, 27, 29);    // %30 = %27 * %29
    w.St(18, 30);            // gl_Position = %30
    w.Ld(31, 5, 15);         // %31 = load %5 %15
    w.St(17, 31);            // outColor = %31

    w.Ret();
    w.FE();

    return w.B();
}

static byte[] BuildFragment()
{
    var w = new SW();
    w.H(14); // bound (max id = 13)
    // IDs: 1=ext, 2=void, 3=func, 4=float, 5=vec4,
    // 6=inColorVar, 7=fragColorVar, 8=main, 9=ptr(in,vec4),
    // 10=ptr(out,vec4), 11=label, 12=load, 13=retlabel

    w.Cap(1);
    w.Ext(1, "GLSL.std.450");
    w.MM(0, 1);
    w.EP(4, 8, "main", [6, 7]); // Fragment

    w.N(8, "main");
    w.N(6, "inColor");
    w.N(7, "fragColor");

    w.D(6, 30, 0);    // Location 0
    w.D(7, 30, 0);    // Location 0

    w.TV(2);
    w.TF(3, 2);
    w.TFloat(4, 32);
    w.TVec(5, 4, 4);     // vec4
    w.TPtr(9, 1, 5);     // ptr(input, vec4)
    w.TPtr(10, 2, 5);    // ptr(output, vec4)

    w.Var(6, 9, 1);   // inColor
    w.Var(7, 10, 2);  // fragColor

    w.Func(8, 2, 0, 3);
    w.Lab(11);

    w.Ld(12, 5, 6);    // %12 = load inColor
    w.St(7, 12);       // fragColor = %12

    w.Ret();
    w.FE();

    return w.B();
}

// ─── Minimal SPIR-V Writer ─────────────────────────────────────────

internal sealed class SW
{
    private readonly List<uint> _w = [];

    public void H(int bound) {
        _w.Add(0x07230203); _w.Add(0x00010000);
        _w.Add(0); _w.Add((uint)bound); _w.Add(0);
    }

    public void Cap(uint v) { Op(17, v); }

    public void Ext(uint id, string s) {
        var sw = new List<uint> { id };
        sw.AddRange(Str(s));
        Op(11, sw.ToArray());
    }

    public void MM(uint a, uint b) { Op(14, a, b); }

    public void EP(uint mode, uint ep, string name, uint[] iface) {
        var list = new List<uint> { mode, ep };
        list.AddRange(Str(name));
        list.AddRange(iface);
        Op(15, list.ToArray());
    }

    public void N(uint t, string n) {
        var list = new List<uint> { t };
        list.AddRange(Str(n));
        Op(5, list.ToArray());
    }

    public void D(uint t, uint d) { Op(71, t, d); }
    public void D(uint t, uint d, uint v) { Op(71, t, d, v); }
    public void MD(uint t, uint m, uint d) { Op(72, t, m, d); }
    public void MD(uint t, uint m, uint d, uint v) { Op(72, t, m, d, v); }
    public void TV(uint r) { Op(19, r); }
    public void TFloat(uint r, uint w) { Op(22, r, w); }
    public void TVec(uint r, uint c, uint n) { Op(23, r, c, n); }
    public void TMat(uint r, uint ct, uint cc) { Op(24, r, ct, cc); }
    public void TPtr(uint r, uint sc, uint t) { Op(32, r, sc, t); }
    public void TF(uint r, uint rt) { Op(33, r, rt); }
    public void TInt(uint r, uint w, uint sg) { Op(21, r, w, sg); }
    public void TStr(uint r, uint[] members) {
        var list = new List<uint> { r };
        list.AddRange(members);
        Op(29, list.ToArray());
    }
    public void Const(uint r, uint t, uint v) { Op(43, r, t, v); }
    public void Var(uint r, uint t, uint sc) { Op(59, r, t, sc); }
    public void Ld(uint r, uint t, uint p) { Op(61, r, t, p); }
    public void St(uint p, uint v) { Op(62, p, v); }
    public void AC(uint r, uint t, uint b, uint i) { Op(65, r, t, b, i); }
    public void CC(uint r, uint t, uint[] cs) {
        var list = new List<uint> { r, t };
        list.AddRange(cs);
        Op(160, list.ToArray());
    }
    public void MTV(uint r, uint t, uint m, uint v) { Op(191, r, t, m, v); }
    public void Func(uint r, uint rt, uint fc, uint ft) { Op(54, r, rt, fc, ft); }
    public void FE() { Op(56); }
    public void Lab(uint r) { Op(248, r); }
    public void Ret() { Op(253); }

    private void Op(uint opc, params uint[] ops) {
        _w.Add((uint)((1 + ops.Length) << 16) | opc);
        _w.AddRange(ops);
    }

    private uint[] Str(string s) {
        s += '\0';
        while (s.Length % 4 != 0) s += '\0';
        var r = new uint[s.Length / 4];
        for (int i = 0; i < r.Length; i++)
            r[i] = (uint)(s[i*4] | (s[i*4+1]<<8) | (s[i*4+2]<<16) | (s[i*4+3]<<24));
        return r;
    }

    public byte[] B() {
        var b = new byte[_w.Count * 4];
        for (int i = 0; i < _w.Count; i++) {
            b[i*4] = (byte)(_w[i] & 0xFF);
            b[i*4+1] = (byte)((_w[i]>>8) & 0xFF);
            b[i*4+2] = (byte)((_w[i]>>16) & 0xFF);
            b[i*4+3] = (byte)((_w[i]>>24) & 0xFF);
        }
        return b;
    }

    public void Verify() {
        Console.WriteLine($"  Words: {_w.Count}, Bytes: {_w.Count * 4}");
    }
}
